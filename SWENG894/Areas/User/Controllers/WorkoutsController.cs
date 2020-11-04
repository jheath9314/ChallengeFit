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
using SWENG894.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public async Task<IActionResult> Index(string sort, string search, string filter, string list, string currentList, int? page)
        {
            ViewData["CurrentSort"] = sort;
            ViewData["SortOrder"] = String.IsNullOrEmpty(sort) ? "desc" : "";

            if (list == null)
            {
                list = currentList;
            }
            ViewData["CurrentList"] = list;

            if (search == null)
            {
                search = filter;
            }
            else
            {
                search = search.ToLower();
            }

            ViewData["CurrentFilter"] = search;

            var workouts = _unitOfWork.Workout.GetUserWorkouts(sort, search, User.FindFirstValue(ClaimTypes.NameIdentifier), string.IsNullOrEmpty(list));

            var workoutList = await PaginatedList<Workout>.Create(workouts.ToList(), page ?? 1, _pageSize);

            return View(workoutList);

        }

        // GET: User/Workouts/Find
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Find(string sort, string search, string filter, int? page, int? fave)
        {
            if(fave != null)
            {
                var favorite = await _unitOfWork.WorkoutFavorite.GetFirstOrDefaultAsync(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.WorkoutId == fave);

                if(favorite != null)
                {
                    await _unitOfWork.WorkoutFavorite.RemoveAsync(favorite);
                }
                else
                {
                    var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(x => x.Id == fave);

                    if(user != null && workout != null)
                    {
                        var newFave = new WorkoutFavorite
                        {
                            User = user,
                            Workout = workout
                        };
                        await _unitOfWork.WorkoutFavorite.AddAsync(newFave);
                    }                   
                }
                await _unitOfWork.Save();
            }

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

            var resultList = await PaginatedList<Workout>.Create(_unitOfWork.Workout.FindNewWorkouts(sort, search, User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), page ?? 1, _pageSize);

            return View(resultList);
        }

        // GET: User/Workouts/Details/5
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Details(int? id, string fave)
        {
            if (id == null)
            {
                //count the number of records
                int total = _unitOfWork.Workout.GetAllAsync(x => x.Published).Result.Count();

                //select a random number from the count, that becomes the "id"
                var random = new Random();
                int num = random.Next(1, total);
                
                //search for the details of the randomly selected 
                var randomWorkout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(m => m.Id == num, includeProperties: "Exercises");

                //because records may be deleted over time, id numbers will be deleted as well. So, loop through the list for an existing record
                while (randomWorkout == null)
                {
                    num = random.Next(1, total);
                    randomWorkout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(m => m.Id == num, includeProperties: "Exercises");
                }

                var fave1 = await _unitOfWork.WorkoutFavorite.GetFirstOrDefaultAsync(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.WorkoutId == randomWorkout.Id);
                randomWorkout.IsFavorite = fave1 != null;

                if(!string.IsNullOrEmpty(fave))
                {
                    if(randomWorkout.IsFavorite)
                    {
                        await _unitOfWork.WorkoutFavorite.RemoveAsync(fave1);
                    }
                    else
                    {
                        var newFave = new WorkoutFavorite() 
                        {
                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            WorkoutId = randomWorkout.Id
                        };
                        await _unitOfWork.WorkoutFavorite.AddAsync(newFave);
                    }
                    randomWorkout.IsFavorite = !randomWorkout.IsFavorite;
                    await _unitOfWork.Save();
                }

                //return test results
                return View(randomWorkout);

            }

            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(m => m.Id == id, includeProperties: "Exercises");

            if (workout == null)
            {
                return NotFound();
            }

            var fave2 = await _unitOfWork.WorkoutFavorite.GetFirstOrDefaultAsync(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.WorkoutId == workout.Id);
            workout.IsFavorite = fave2 != null;

            if (!string.IsNullOrEmpty(fave))
            {
                if (workout.IsFavorite)
                {
                    await _unitOfWork.WorkoutFavorite.RemoveAsync(fave2);                    
                }
                else
                {
                    var newFave = new WorkoutFavorite()
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        WorkoutId = workout.Id
                    };
                    await _unitOfWork.WorkoutFavorite.AddAsync(newFave);
                }
                workout.IsFavorite = !workout.IsFavorite;
                await _unitOfWork.Save();
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
                var user = await _unitOfWork.ApplicationUser.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                workout.User = user;

                // Add to favorites when the exercise is published. This block is just for testing.

                await _unitOfWork.Workout.AddAsync(workout);
                await _unitOfWork.Save();

                var fave = new WorkoutFavorite()
                {
                    User = user,
                    Workout = workout
                };
                await _unitOfWork.WorkoutFavorite.AddAsync(fave);
                await _unitOfWork.Save();

                // End test

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

            if (workout.Published)
            {
                return Forbid();
            }

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

            if (workout.Published) 
            {
                return Forbid();
            }

            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (workout.UserId != user.Id)
            {
                return Forbid();
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

            if(workout.Published)
            {
                return Forbid();
            }

            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (workout.UserId != user.Id)
            {
                return Forbid();
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

        public async Task<IActionResult> Publish(int id)
        {
            var workout = await _unitOfWork.Workout.GetAsync(id);
            var exs = await _unitOfWork.Exercise.GetAllAsync(e => e.WorkoutId == workout.Id);
            workout.Exercises = exs.ToList();

            if(workout.Exercises.Count < 1)
            {
                return Forbid();
            }
            workout.Published = true;
            _unitOfWork.Workout.UpdateAsync(workout);
            await _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }
    }
}
