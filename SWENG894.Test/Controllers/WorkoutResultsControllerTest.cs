using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWENG894.Areas.User.Controllers;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Models;
using System.Linq;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace SWENG894.Test.Controllers
{
    public class WorkoutResultsControllerTest
    {
        private readonly ApplicationDbContext _context;
        public WorkoutResultsControllerTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryWorkoutResults" +
                "Controller");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async void WorkoutResultsContollerTestOp()
        {
            var usr1 = new ApplicationUser
            {
                Id = "guid-user1",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "guid-user2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var w = new Workout()
            {
                Id = 1,
                Name = "Name",
                Notes = "Notes",
                ScoringType = Workout.Scoring.Time,
                Time = 20,
                Published = true
            };

            var ch = new Challenge()
            {
                Id = 1,
                Challenger = usr1,
                Contender = usr2,
                Workout = w,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Workouts.Add(w);
            _context.Challenges.Add(ch);
            _context.SaveChangesAsync().GetAwaiter();

            var httpContext = new DefaultHttpContext();
            var tempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            tempData["Response"] = "";
            var unit = new UnitOfWork(_context);
            var cont = new WorkoutResultsController(unit)
            {
                TempData = tempData
            };

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
            }, "mock"));

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            int workoutId = _context.Workouts.FirstOrDefault().Id;

            var res = new WorkoutResult()
            {
                Id = 0,
                UserId = "guid-user1",
                WorkoutId = workoutId,
                Score = 600,
                ScoringType = Workout.Scoring.Time,
                RelatedChallenge = (int)ch.Id
            };

            await cont.Create(workoutId, res, 0);
            _context.WorkoutResults.Add(res);

            var data = await _context.WorkoutResults.FirstOrDefaultAsync();
            Assert.Single(_context.WorkoutResults);
            Assert.Equal(600, data.Score);

            data = await _context.WorkoutResults.FirstOrDefaultAsync();
            data.Score = 900;

            await cont.Edit(data.Id, data, 0);
            data = _context.WorkoutResults.FirstOrDefault(x => x.Id == data.Id);
            Assert.Equal(900, data.Score);

            w = _context.Workouts.FirstOrDefault(w => w.Id == workoutId);
            w.ScoringType = Workout.Scoring.Rounds;
            _context.Workouts.Update(w);
            data.ScoringType = Workout.Scoring.Rounds;
            data.Score = 2;
            await cont.Edit(data.Id, data, 2);
            data = _context.WorkoutResults.FirstOrDefault(x => x.Id == data.Id);
            Assert.True(data.Score == 122);

            await cont.DeleteConfirmed(1);
            data = _context.WorkoutResults.FirstOrDefault(x => x.Id == data.Id);
            Assert.Null(data);

            _context.Database.EnsureDeleted();
        }

        [Fact]
        public async void WorkoutResultControllerCreateChallengeAcceptedTest()
        {
            var usr1 = new ApplicationUser
            {
                Id = "guid-user1",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "guid-user2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var w = new Workout()
            {
                Id = 1,
                Name = "Name",
                Notes = "Notes",
                ScoringType = Workout.Scoring.Time,
                Time = 20,
                Published = true
            };

            var ch = new Challenge()
            {
                Id = 1,
                Challenger = usr1,
                Contender = usr2,
                Workout = w,
                ChallengeProgress = Challenge.ChallengeStatus.Accepted,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Workouts.Add(w);
            _context.Challenges.Add(ch);
            _context.SaveChangesAsync().GetAwaiter();

            var httpContext = new DefaultHttpContext();
            var tempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            tempData["Response"] = "";
            var unit = new UnitOfWork(_context);
            var cont = new WorkoutResultsController(unit)
            {
                TempData = tempData
            };

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
            }, "mock"));

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            int workoutId = _context.Workouts.FirstOrDefault().Id;

            var res = new WorkoutResult()
            {
                Id = 0,
                UserId = "guid-user1",
                WorkoutId = workoutId,
                Score = 600,
                ScoringType = Workout.Scoring.Time,
                RelatedChallenge = (int)ch.Id
            };

            await cont.Create(workoutId, res, 0);
            Assert.True(_context.NewsFeed.Count() == 1);
            var feedList = _context.NewsFeed.ToList();
            var feed = feedList.ElementAt(0);
            Assert.True(feed.UserId == usr1.Id);
            Assert.True(feed.RelatedUserId == usr2.Id);
            Assert.True(feed.FeedType == NewsFeed.FeedTypes.CompletedChallenge);


            _context.Database.EnsureDeleted();
        }

        [Fact]
        public async void WorkoutResultControllerCreateChallengeCompletedByChallengerTest()
        {
            var usr1 = new ApplicationUser
            {
                Id = "guid-user2",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "guid-user1",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var w = new Workout()
            {
                Id = 1,
                Name = "Name",
                Notes = "Notes",
                ScoringType = Workout.Scoring.Time,
                Time = 20,
                Published = true
            };

            var ch = new Challenge()
            {
                Id = 1,
                Challenger = usr1,
                Contender = usr2,
                Workout = w,
                ChallengeProgress = Challenge.ChallengeStatus.CompletedByChallenger,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Workouts.Add(w);
            _context.Challenges.Add(ch);
            _context.SaveChangesAsync().GetAwaiter();

            var httpContext = new DefaultHttpContext();
            var tempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            tempData["Response"] = "";
            var unit = new UnitOfWork(_context);
            var cont = new WorkoutResultsController(unit)
            {
                TempData = tempData
            };

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
            }, "mock"));

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            int workoutId = _context.Workouts.FirstOrDefault().Id;

            var res = new WorkoutResult()
            {
                Id = 0,
                UserId = "guid-user1",
                WorkoutId = workoutId,
                Score = 800,
                ScoringType = Workout.Scoring.Time,
                RelatedChallenge = (int)ch.Id
            };

            ch.ChallengerResult = res;
            _context.Challenges.Update(ch);
            _context.SaveChangesAsync().GetAwaiter();

            await cont.Create(workoutId, res, 0);
            Assert.True(_context.NewsFeed.Count() == 1);
            var feedList = _context.NewsFeed.ToList();
            var feed = feedList.ElementAt(0);
            Assert.True(feed.UserId == usr2.Id);
            Assert.True(feed.RelatedUserId == usr1.Id);
            Assert.True(feed.FeedType == NewsFeed.FeedTypes.CompletedChallenge);

            _context.Database.EnsureDeleted();
        }

        [Fact]
        public async void WorkoutResultControllerCreateChallengeCompletedByContenderTest()
        {
            var usr1 = new ApplicationUser
            {
                Id = "guid-user1",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "guid-user2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var w = new Workout()
            {
                Id = 1,
                Name = "Name",
                Notes = "Notes",
                ScoringType = Workout.Scoring.Time,
                Time = 20,
                Published = true
            };

            var ch = new Challenge()
            {
                Id = 1,
                Challenger = usr1,
                Contender = usr2,
                Workout = w,
                ChallengeProgress = Challenge.ChallengeStatus.CompletedByContender,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Workouts.Add(w);
            _context.Challenges.Add(ch);
            _context.SaveChangesAsync().GetAwaiter();

            var httpContext = new DefaultHttpContext();
            var tempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            tempData["Response"] = "";
            var unit = new UnitOfWork(_context);
            var cont = new WorkoutResultsController(unit)
            {
                TempData = tempData
            };

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
            }, "mock"));

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            int workoutId = _context.Workouts.FirstOrDefault().Id;

            var res = new WorkoutResult()
            {
                Id = 0,
                UserId = "guid-user1",
                WorkoutId = workoutId,
                Score = 800,
                ScoringType = Workout.Scoring.Time,
                RelatedChallenge = (int)ch.Id
            };

            ch.ContenderResult = res;
            _context.Challenges.Update(ch);
            _context.SaveChangesAsync().GetAwaiter();

            await cont.Create(workoutId, res, 0);
            Assert.True(_context.NewsFeed.Count() == 1);
            var feedList = _context.NewsFeed.ToList();
            var feed = feedList.ElementAt(0);
            Assert.True(feed.UserId == usr1.Id);
            Assert.True(feed.RelatedUserId == usr2.Id);
            Assert.True(feed.FeedType == NewsFeed.FeedTypes.CompletedChallenge);


            _context.Database.EnsureDeleted();
        }

        [Fact]
        public async void WorkoutResultControllerCreateNullChallengeTest()
        {
            var usr1 = new ApplicationUser
            {
                Id = "guid-user1",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "guid-user2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var w = new Workout()
            {
                Id = 1,
                Name = "Name",
                Notes = "Notes",
                ScoringType = Workout.Scoring.Rounds,
                Time = 20,
                Published = true
            };

            var ch = new Challenge()
            {
                Id = 1,
                Challenger = usr1,
                Contender = usr2,
                Workout = w,
                ChallengeProgress = Challenge.ChallengeStatus.CompletedByContender,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Workouts.Add(w);
            _context.Challenges.Add(ch);
            _context.SaveChangesAsync().GetAwaiter();

            var httpContext = new DefaultHttpContext();
            var tempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            tempData["Response"] = "";
            var unit = new UnitOfWork(_context);
            var cont = new WorkoutResultsController(unit)
            {
                TempData = tempData
            };

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
            }, "mock"));

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            int workoutId = _context.Workouts.FirstOrDefault().Id;

            var res = new WorkoutResult()
            {
                Id = 0,
                UserId = "guid-user1",
                WorkoutId = workoutId,
                Score = 600,
                ScoringType = Workout.Scoring.Rounds,
                RelatedChallenge = null,
            };

            await cont.Create(workoutId, res, 0);
            Assert.True(_context.NewsFeed.Count() == 1);
            var feedList = _context.NewsFeed.ToList();
            var feed = feedList.ElementAt(0);
            Assert.True(feed.UserId == usr1.Id);
            Assert.True(feed.RelatedUserId == null);
            Assert.True(feed.FeedType == NewsFeed.FeedTypes.CompletedWorkout);


            _context.Database.EnsureDeleted();
        }

    }
}
