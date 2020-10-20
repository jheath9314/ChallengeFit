using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SWENG894.Data;
using SWENG894.Models;
using SWENG894.Utility;
using SWENG894.Data.Repository.IRepository;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class WorkoutsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;

        public WorkoutsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _pageSize = 5;
        }

        // GET: User/Workouts
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Index(string sort, string search, string filter, int? page)
        {
            ViewData["CurrentSort"] = sort;
            ViewData["SortOrder"] = String.IsNullOrEmpty(sort) ? "desc" : "";

            if (search == null)
            {
                search = filter;
            }
            else
            {
                search = search.ToLower();
            }

            ViewData["CurrentFilter"] = search;

            var workouts = _unitOfWork.Workout.GetAllWorkouts(sort, search);

            var workoutList = await PaginatedList<Workout>.Create(workouts.ToList(), page ?? 1, _pageSize);

            return View(workoutList);

        }

        // GET: User/Workouts/Details/5
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(m => m.Id == id, includeProperties: "Exercises");

            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // GET: User/Workouts/Create
        [ExcludeFromCodeCoverage]
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Workouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Workout workout, int seconds = 0)
        {
            //time is in minutes, store in seconds. Could be broken up and written as unit test
            if(seconds == 0)
            {
                seconds = 0;
            }
            
            if(workout.ScoringType == Workout.Scoring.Time)
            {
                workout.Time = workout.Time * 60 + seconds;
            }
            //This is needed since the "seconds" value in the function arguments can
            //cause an error if it's not set within the HTML form. We don't always need it
            //set and it's annoying to default it to 0
            ModelState.Remove("seconds");
            if (ModelState.IsValid)
            {
                await _unitOfWork.Workout.AddAsync(workout);
                await _unitOfWork.Save();

                var user = await _unitOfWork.ApplicationUser.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var fave = new WorkoutFavorite()
                {
                    User = user,
                    Workout = workout
                };
                await _unitOfWork.WorkoutFavorite.AddAsync(fave);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        // GET: User/Workouts/Edit/5
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _unitOfWork.Workout.GetAsync((int)id);
            if (workout == null)
            {
                return NotFound();
            }
            return View(workout);
        }

        // POST: User/Workouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,time,notes,reps")] Workout workout)
        {
            if (id != workout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Workout.UpdateAsync(workout);
                    await _unitOfWork.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExists(workout.Id))
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
            return View(workout);
        }

        // GET: User/Workouts/Delete/5
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(m => m.Id == id);

            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // POST: User/Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workout = await _unitOfWork.Workout.GetAsync(id);
            await _unitOfWork.Workout.RemoveAsync(workout);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutExists(int id)
        {
            return _unitOfWork.Workout.ObjectExists(id);
        }
    }
}
