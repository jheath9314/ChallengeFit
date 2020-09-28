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
using SWENG894.Models;

namespace SWENG894.Areas.User.Views
{
    [Area("User")]
    public class WorkoutResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkoutResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/WorkoutResults
        public async Task<IActionResult> Index()
        {
            //We should consider filtering by username here or this could get very slow
            var loggedInUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            var workoutResults = await _context.WorkoutResults.Where(w => w.UserId == loggedInUser.Id).ToListAsync();

            for(int i = 0; i < workoutResults.Count; i++)
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == workoutResults[i].UserId);
                var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == workoutResults[i].WorkoutId);

                workoutResults[i].username = user.FullName;
                workoutResults[i].workoutName = workout.Name;
                workoutResults[i].ScoringType = workout.ScoringType;
            }

            return View(workoutResults);
        }

        // GET: User/WorkoutResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutResults = await _context.WorkoutResults
                .FirstOrDefaultAsync(m => m.Id == id);

            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);
            workoutResults.username = user.FullName;
            workoutResults.workoutName = workout.Name;
            workoutResults.ScoringType = workout.ScoringType;

            if (workoutResults == null)
            {
                return NotFound();
            }

            return View(workoutResults);
        }

        // GET: User/WorkoutResults/Create
        public IActionResult Create(int Id)
        {
            //get the model data needed for determining how to record results
            var workout = _context.Workouts.FirstOrDefault(w => w.Id == Id);
            var workoutResults = new WorkoutResults();
            workoutResults.ScoringType = workout.ScoringType;
            workoutResults.workoutName = workout.Name;
            return View(workoutResults);
        }

        // POST: User/WorkoutResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkoutId,UserId,Score")] int Id, WorkoutResults workoutResults, int seconds)
        {

            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            workoutResults.UserId = user.Id;
            workoutResults.WorkoutId = Id;
            workoutResults.Id = 0;

            if (workoutResults.ScoringType == Workout.Scoring.Time)
            {
                workoutResults.Score = workoutResults.Score * 60 + seconds;
            }

            var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);

            workoutResults.username = user.FullName;
            workoutResults.workoutName = workout.Name;
            workoutResults.ScoringType = workout.ScoringType;

            ModelState.Remove("seconds");
            if (ModelState.IsValid)
            {
                _context.Add(workoutResults);
                await _context.SaveChangesAsync();
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

            var workoutResults = await _context.WorkoutResults.FindAsync(id);
            if (workoutResults == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);
            workoutResults.username = user.FullName;
            workoutResults.workoutName = workout.Name;
            workoutResults.ScoringType = workout.ScoringType;

            return View(workoutResults);
        }

        // POST: User/WorkoutResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkoutId,UserId,Score")] WorkoutResults workoutResults, int seconds)
        {
            if (id != workoutResults.Id)
            {
                return NotFound();
            }

            //get the real workout results
            var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);

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
                    _context.Update(workoutResults);
                    await _context.SaveChangesAsync();
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

            var workoutResults = await _context.WorkoutResults
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workoutResults == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);
            workoutResults.username = user.FullName;
            workoutResults.workoutName = workout.Name;
            workoutResults.ScoringType = workout.ScoringType;

            return View(workoutResults);
        }

        // POST: User/WorkoutResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutResults = await _context.WorkoutResults.FindAsync(id);
            _context.WorkoutResults.Remove(workoutResults);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutResultsExists(int id)
        {
            return _context.WorkoutResults.Any(e => e.Id == id);
        }
    }
}
