using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace SWENG894.Areas.User.Controllers
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
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Index()
        {
            //We should consider filtering by username here or this could get very slow
            //var loggedInUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            //You can skip the first query and filter directly by ClaimTypes.NameIdentifier
            var workoutResults = await _unitOfWork.WorkoutResult.GetAllAsync(w => w.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier), includeProperties: "Workout,User");
            var workoutResultList = workoutResults.ToList();
            //Workout results already include Workout and User
            for(int i = 0; i < workoutResults.Count(); i++)
            {
                var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == workoutResultList[i].UserId);
                var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(w => w.Id == workoutResultList[i].WorkoutId);

                workoutResultList[i].Username = user.FullName;
                workoutResultList[i].WorkoutName = workout.Name;
                workoutResultList[i].ScoringType = workout.ScoringType;
            }

            return View(workoutResultList);
        }

        // GET: User/WorkoutResults/Details/5
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutResults = await _unitOfWork.WorkoutResult.GetFirstOrDefaultAsync(m => m.Id == id, includeProperties: "Workout,User");

            //Same as above we have User and Workout properties in WorkoutResult
            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);
            workoutResults.Username = user.FullName;
            workoutResults.WorkoutName = workout.Name;
            workoutResults.ScoringType = workout.ScoringType;

            if (workoutResults == null)
            {
                return NotFound();
            }

            return View(workoutResults);
        }

        // GET: User/WorkoutResults/Create
        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> CreateAsync(int id, int? challenge)
        {
            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(w => w.Id == id);

            if(workout == null)
            {
                return NotFound();
            }

            if(!workout.Published)
            {
                return Forbid();
            }

            var workoutResults = new WorkoutResult()
            {
                Workout = workout
            };

            workoutResults.ScoringType = workout.ScoringType;

            if(challenge != null)
            {
                workoutResults.RelatedChallenge = challenge;
            }

            return View(workoutResults);
        }

        [ExcludeFromCodeCoverage]
        public async Task<IActionResult> ViewWorkoutResults(int? id)
        {
            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(m => m.Id == id);

            if (workout == null)
            {
                return NotFound();
            }
            var user = await _unitOfWork.ApplicationUser.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var workoutResults =  _unitOfWork.WorkoutResult.GetWorkoutResults(user.Id, workout.Id);


            return View(workoutResults);

        }

        // POST: User/WorkoutResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkoutId,UserId,Score")] int Id, WorkoutResult workoutResults, int seconds)
        {
            ModelState.Remove("seconds");
            if (ModelState.IsValid)
            {
                var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(w => w.Id == Id);

                if(user == null || workout == null)
                {
                    return NotFound();
                }

                if(!workout.Published)
                {
                    return Forbid();
                }


                var newResult = new WorkoutResult() 
                { 
                    User = user,
                    Workout = workout,
                    Score = workoutResults.Score,
                    ResultNotes = workoutResults.ResultNotes,
                    ScoringType = workoutResults.ScoringType
                };

                //Reps means that the workout type is a "Reps" workout. Scoring for a "Reps" workout is by time.
                if (workout.ScoringType == Workout.Scoring.Reps)
                {
                    newResult.Score = workoutResults.Score * 60 + seconds;
                }

            
                await _unitOfWork.WorkoutResult.AddAsync(newResult);

                if(workoutResults.RelatedChallenge != null)
                {
                    var clg = await  _unitOfWork.Challenge.GetFirstOrDefaultAsync(x => x.Id == (int)workoutResults.RelatedChallenge, includeProperties: "Challenger,Contender,Workout");

                    if(clg != null)
                    {
                        if(clg.ChallengerId == User.FindFirstValue(ClaimTypes.NameIdentifier) || clg.ContenderId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        {
                            var feed = new NewsFeed()
                            {
                                RelatedChallenge = clg,
                                RelatedWorkout = clg.Workout,
                                CreateDate = DateTime.Now,
                                FeedType = NewsFeed.FeedTypes.CompletedChallenge,
                                Dismissed = false
                            };

                            switch (clg.ChallengeProgress)
                            {
                                case Models.Challenge.ChallengeStatus.Accepted:
                                    if(clg.ChallengerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                    {
                                        clg.ChallengeProgress = Models.Challenge.ChallengeStatus.CompletedByChallenger;
                                        clg.ChallengerResult = newResult;
                                        feed.User = clg.Challenger;
                                        feed.RelatedUser = clg.Contender;
                                        feed.Description = clg.Challenger.FullName + " completed a challenge!";
                                    }
                                    else if(clg.ContenderId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                    {
                                        clg.ChallengeProgress = Models.Challenge.ChallengeStatus.CompletedByContender;
                                        clg.ContenderResult = newResult;
                                        feed.User = clg.Contender;
                                        feed.RelatedUser = clg.Challenger;
                                        feed.Description = clg.Contender.FullName + " completed a challenge!";
                                    }
                                    break;

                                case Models.Challenge.ChallengeStatus.CompletedByChallenger:
                                    if (clg.ContenderId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                    {
                                        clg.ChallengeProgress = Models.Challenge.ChallengeStatus.Completed;
                                        clg.ContenderResult = newResult;
                                        feed.User = clg.Contender;
                                        feed.RelatedUser = clg.Challenger;
                                        feed.Description = clg.Contender.FullName + " completed a challenge!";
                                    }                                      
                                    break;

                                case Models.Challenge.ChallengeStatus.CompletedByContender:
                                    if (clg.ChallengerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                    {
                                        clg.ChallengeProgress = Models.Challenge.ChallengeStatus.Completed;
                                        clg.ChallengerResult = newResult;
                                        feed.User = clg.Challenger;
                                        feed.RelatedUser = clg.Contender;
                                        feed.Description = clg.Challenger.FullName + " completed a challenge!";
                                    }                                      
                                    break;
                            }
                            if(feed.User != null && feed.RelatedUser != null)
                            {
                                await _unitOfWork.NewsFeed.AddAsync(feed);
                            }
                        }
                        _unitOfWork.Challenge.UpdateAsync(clg);
                    }
                }

                await _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
         
            return View(workoutResults);
        }

        // GET: User/WorkoutResults/Edit/5
        [ExcludeFromCodeCoverage]
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkoutId,UserId,Score, ResultNotes")] WorkoutResult workoutResults, int seconds)
        {
            if (id != workoutResults.Id)
            {
                return NotFound();
            }

            //get the real workout results
            var workout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(w => w.Id == workoutResults.WorkoutId);

            workoutResults.ScoringType = workout.ScoringType;

            if (workoutResults.ScoringType == Workout.Scoring.Reps)
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
        [ExcludeFromCodeCoverage]
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
        //Vladimir, need help with this one
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
