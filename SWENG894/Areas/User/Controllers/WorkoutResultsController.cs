using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWENG894.Areas.User.Controllers;
using SWENG894.Data;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;

namespace SWENG894.Areas.User.Views
{
    [Area("User")]
    public class WorkoutResultsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;

        public WorkoutResultsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _pageSize = 5;
        }

        // GET: User/WorkoutResults
        public async Task<IActionResult> Index()
        {
            //We should consider filtering by username here or this could get very slow
            //var loggedInUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            //You can skip the first query and filter directly by ClaimTypes.NameIdentifier
            var workoutResults = await _unitOfWork.WorkoutResult.GetAllAsync(w => w.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier), includeProperties: "Workout,User");

            //Workout results alreay include Workout and User
            //for(int i = 0; i < workoutResults.Count(); i++)
            //{
            //    var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == workoutResults[i].UserId);
            //    var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == workoutResults[i].WorkoutId);

            //    workoutResults[i].Username = user.FullName;
            //    workoutResults[i].WorkoutName = workout.Name;
            //    workoutResults[i].ScoringType = workout.ScoringType;
            //}

            return View(workoutResults);
        }

        // GET: User/WorkoutResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutResults = await _unitOfWork.WorkoutResult.GetFirstOrDefaultAsync(m => m.Id == id, includeProperties: "Workout,User");

            //Same as above we have User and Workout properties in WorkoutResult
            //var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            //var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);
            //workoutResults.Username = user.FullName;
            //workoutResults.WorkoutName = workout.Name;
            //workoutResults.ScoringType = workout.ScoringType;

            if (workoutResults == null)
            {
                return NotFound();
            }

            return View(workoutResults);
        }

        // GET: User/WorkoutResults/Create
        public IActionResult Create(int Id)
        {
            //When you create a new result from Index view, there's no workout id and this errors out. Not sure what the desired action is.
            //get the model data needed for determining how to record results
            //var workout = _context.Workouts.FirstOrDefault(w => w.Id == Id);
            //var workoutResults = new WorkoutResult();
            //workoutResults.ScoringType = workout.ScoringType;
            //workoutResults.WorkoutName = workout.Name;
            //return View(workoutResults);

            return View();
        }

        // POST: User/WorkoutResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkoutId,UserId,Score")] int Id, WorkoutResult workoutResults, int seconds)
        {

            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            workoutResults.UserId = user.Id;
            workoutResults.WorkoutId = Id;
            workoutResults.Id = 0;

            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);
            workoutResults.ScoringType = workout.ScoringType;


            workoutResults.Username = user.FullName;
            workoutResults.WorkoutName = workout.Name;
            workoutResults.ScoringType = workout.ScoringType;

            if (workoutResults.ScoringType == Workout.Scoring.Time)
            {
                workoutResults.Score = workoutResults.Score * 60 + seconds;
            }

            ModelState.Remove("seconds");
            if (ModelState.IsValid)
            {
                await _unitOfWork.WorkoutResult.AddAsync(workoutResults);
                await _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            
            return View(workoutResults);
        }

        // GET: User/WorkoutResults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutResults = await _unitOfWork.WorkoutResult.GetAsync((int)id);
            if (workoutResults == null)
            {
                return NotFound();
            }

            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);
            workoutResults.Username = user.FullName;
            workoutResults.WorkoutName = workout.Name;
            workoutResults.ScoringType = workout.ScoringType;

            return View(workoutResults);
        }

        // POST: User/WorkoutResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkoutId,UserId,Score")] WorkoutResult workoutResults, int seconds)
        {
            if (id != workoutResults.Id)
            {
                return NotFound();
            }

            //get the real workout results
            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);

            workoutResults.ScoringType = workout.ScoringType;

            if (workoutResults.ScoringType == Workout.Scoring.Time)
            {
                workoutResults.Score = workoutResults.Score * 60 + seconds;
            }

            ModelState.Remove("seconds");
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.WorkoutResult.UpdateAsync(workoutResults);
                    await _unitOfWork.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutResultsExists(workoutResults.Id))
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
            return View(workoutResults);
        }

        // GET: User/WorkoutResults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutResults = await _unitOfWork.WorkoutResult.GetFirstOrDefaultAsync(m => m.Id == id);
            if (workoutResults == null)
            {
                return NotFound();
            }

            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);
            workoutResults.Username = user.FullName;
            workoutResults.WorkoutName = workout.Name;
            workoutResults.ScoringType = workout.ScoringType;

            return View(workoutResults);
        }

        // POST: User/WorkoutResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutResults = await _unitOfWork.WorkoutResult.GetAsync(id);
            await _unitOfWork.WorkoutResult.RemoveAsync(workoutResults);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutResultsExists(int id)
        {
            return _unitOfWork.WorkoutResult.ObjectExists(id);
        }
    }
}
