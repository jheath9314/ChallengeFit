﻿using System;
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

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin, User")]
    public class WorkoutsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly int _pageSize;

        public WorkoutsController(ApplicationDbContext context)
        {
            _context = context;
            _pageSize = 5;
        }

        // GET: User/Workouts
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

            var workouts = _context.Workouts.Where(w => w.Name != null);

            if (!String.IsNullOrEmpty(search))
            {
                workouts = workouts.Where(s => s.Name.Contains(search));
            }

            workouts = sort switch
            {
                "desc" => workouts.OrderByDescending(s => s.Name),
                _ => workouts.OrderBy(s => s.Name),
            };

            var workoutList = await PaginatedList<Workout>.CreateAsync(workouts.AsNoTracking(), page ?? 1, _pageSize);

            return View(workoutList);

            //return View(await _context.Workouts.ToListAsync());
        }

        // GET: User/Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts.Include("Exercises")
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // GET: User/Workouts/Create
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
                _context.Add(workout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        // GET: User/Workouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts.FindAsync(id);
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
                    _context.Update(workout);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var workout = await _context.Workouts.FindAsync(id);
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutExists(int id)
        {
            return _context.Workouts.Any(e => e.Id == id);
        }
    }
}
