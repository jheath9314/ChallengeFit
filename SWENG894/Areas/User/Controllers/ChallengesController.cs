﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using SWENG894.Utility;
using SWENG894.ViewModels;

namespace SWENG894.Areas.User.Controllers
{
    [Area("User")]
    public class ChallengesController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize;

        public ChallengesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _pageSize = 5;
        }

        // GET: User/Challenges
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

            var challenges = await _unitOfWork.Challenge.GetAllAsync(x => x.ChallengerId == User.FindFirstValue(ClaimTypes.NameIdentifier) || x.ContenderId == User.FindFirstValue(ClaimTypes.NameIdentifier), includeProperties: "Challenger,Contender");
            var challengeList = await PaginatedList<Challenge>.Create(challenges.ToList(), page ?? 1, _pageSize);

            return View(challengeList);
        }

        // GET: User/Challenges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var challenge = await _context.Challenges
            //    .Include(c => c.Challenger)
            //    .Include(c => c.ChallengerResult)
            //    .Include(c => c.Contender)
            //    .Include(c => c.ContenderResult)
            //    .Include(c => c.Workout)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var challenge = await _unitOfWork.Challenge.GetFirstOrDefaultAsync(c => c.Id == id, includeProperties: "Challenger,ChallengerResult,Contender,ContenderResult,Workout");
            if (challenge == null)
            {
                return NotFound();
            }

            return View(challenge);
        }

        // GET: User/Challenges/Create
        public async Task<IActionResult> Create(string id)
        {
            var challengerUser = await _unitOfWork.ApplicationUser.GetUserWithWorkouts(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var contenderUser = await _unitOfWork.ApplicationUser.GetAsync(id);
            
            //if(challengerUser == null || contenderUser == null)
            //{
            //    return NotFound();
            //}

            var challengerFriends = _unitOfWork.FriendRequest.GetUserFriends("", "", challengerUser.Id, false);
            if(challengerFriends.Where(x => x.Id == id) == null)
            {
                return NotFound();
            }

            var challenge = new ChallengeViewModel()
            {
                Challenger = challengerUser,
                Contender = contenderUser,
                Friends = challengerFriends,
                WorkoutFavorites = challengerUser.WorkoutFavorites.ToList()
            };
            return View(challenge);
        }

        // POST: User/Challenges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChallengeViewModel challenge)
        {
            if (ModelState.IsValid)
            {
                var challengerUser = await _unitOfWork.ApplicationUser.GetUserWithWorkouts(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var contenderUser = await _unitOfWork.ApplicationUser.GetAsync(challenge.ContenderId);

                if (challengerUser == null || contenderUser == null)
                {
                    return NotFound();
                }

                var challengerFriends = _unitOfWork.FriendRequest.GetUserFriends("", "", challengerUser.Id, false);
                if (challengerFriends.Where(x => x.Id == challenge.ContenderId) == null)
                {
                    return NotFound();
                }

                var selectedWorkout = await _unitOfWork.Workout.GetFirstOrDefaultAsync(x => x.Id == challenge.WorkoutId);
                if(selectedWorkout == null)
                {
                    return NotFound();
                }

                var newChallenge = new Challenge()
                {
                    Challenger = challengerUser,
                    Contender = contenderUser,
                    Workout = selectedWorkout,
                    ChallengeProgress = Models.Challenge.ChallengeStatus.New
                };

                await _unitOfWork.Challenge.AddAsync(newChallenge);
                await _unitOfWork.Save();

                var msg = new Message()
                {
                    SentById = challengerUser.Id,
                    SentToId = contenderUser.Id,
                    Subject = "You've been challenged!",
                    Body = challengerUser.FullName + " sent you a challenge!",
                    SentTime = DateTime.Now,
                    MessageType = Message.MessageTypes.Challenge,
                    SendStatus = Message.MessageSendStatud.New,
                    ReadStatus = Message.MessageReadStatud.New,
                    DeletedByReceiver = false,
                    DeletedBySender = false
                };

                var feed = new NewsFeed()
                {
                    User = challengerUser,
                    RelatedUser = contenderUser,
                    RelatedChallenge = newChallenge,
                    RelatedWorkout = selectedWorkout,
                    CreateDate = DateTime.Now,
                    FeedType = NewsFeed.FeedTypes.SentChallenge,
                    Description = challengerUser.FullName + " sent a challenge to " + contenderUser.FullName
                };

                await _unitOfWork.Message.AddAsync(msg);
                await _unitOfWork.NewsFeed.AddAsync(feed);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(challenge);
        }

        private bool ChallengeExists(int id)
        {
            return _unitOfWork.Challenge.ObjectExists(id);
        }
    }
}