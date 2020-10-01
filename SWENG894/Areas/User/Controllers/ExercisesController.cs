using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using SWENG894.Data;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class ExercisesController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;

        public ExercisesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: User/Exercises
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.Exercise.GetAllAsync());
        }

        // GET: User/Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _unitOfWork.Exercise.GetFirstOrDefaultAsync(m => m.Id == id);

            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        // GET: User/Exercises/Create
        public async Task<IActionResult> Create(int id)
        {
            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(x => x.Id == id);

            if (workout == null)
            {
                return NotFound();
            }

            return View();
        }

        // POST: User/Exercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, Exercise e)
        {
            if (ModelState.IsValid) {

                var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(x => x.Id == id, includeProperties: "Exercises");

                if (workout == null)
                {
                    return NotFound();
                }

                Exercise exToAdd = new Exercise
                {
                    Workout = workout,
                    Exer = e.Exer,
                    Reps = e.Reps
                };

                await _unitOfWork.Exercise.AddAsync(exToAdd);
                await _unitOfWork.Save();

                return RedirectToAction("Details", "Workouts", new { Id = exToAdd.WorkoutId });
            }
            return View(e);
        }

        // GET: User/Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _unitOfWork.Exercise.GetAsync((int)id);

            if (exercise == null)
            {
                return NotFound();
            }
            return View(exercise);
        }

        // POST: User/Exercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("exercise,reps")] Exercise exercise)
        {
            if (id != exercise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Exercise.UpdateAsync(exercise);
                    await _unitOfWork.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExists(exercise.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(exercise);
        }

        // GET: User/Exercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _unitOfWork.Exercise.GetFirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        // POST: User/Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercise = await _unitOfWork.Exercise.GetAsync(id);
            if(exercise != null)
            {
                await _unitOfWork.Exercise.RemoveAsync(exercise);
                await _unitOfWork.Save();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExists(int id)
        {
            return _unitOfWork.Exercise.ObjectExists(id);
        }
    }
}
