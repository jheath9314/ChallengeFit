using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWENG894.Areas.User.Controllers;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace SWENG894.Test.Controllers
{
    public class LeaderboardControllerTest
    {

        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LeaderboardController _cut;

        public LeaderboardControllerTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryLeaderboardController");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _unitOfWork = new UnitOfWork(_context);
            _cut = new LeaderboardController(_unitOfWork);
        }

        [Fact]
        public async void FriendsControllerOpTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user1",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "user2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var usr3 = new ApplicationUser
            {
                Id = "user3",
                UserName = "user3@psu.edu",
                Email = "user3@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Three",
                ZipCode = "33333"
            };

            var wk = new Workout()
            {
                Id = 1,
                Name = "First",
                Published = true,
                ScoringType = Workout.Scoring.Rounds,
                User = usr1
            };

            var res = new WorkoutResult()
            {
                Id = 1,
                User = usr1,
                Workout = wk,
                Score = 10
            };

            var res2 = new WorkoutResult()
            {
                Id = 2,
                User = usr1,
                Workout = wk,
                Score = 20
            };

            var res3 = new WorkoutResult()
            {
                Id = 3,
                User = usr2,
                Workout = wk,
                Score = 30
            };

            var res4 = new WorkoutResult()
            {
                Id = 4,
                User = usr2,
                Workout = wk,
                Score = 40
            };

            var chlg = new Challenge
            {
                Id = 1,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            var chlg2 = new Challenge
            {
                Id = 2,
                ChallengeProgress = Challenge.ChallengeStatus.CompletedByChallenger,
                Challenger = usr1,
                Contender = usr3,
                CreateDate = DateTime.Now
            };

            var chlg3 = new Challenge
            {
                Id = 3,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                Challenger = usr2,
                Contender = usr3,
                CreateDate = DateTime.Now
            };

            var chlg4 = new Challenge
            {
                Id = 4,
                ChallengeProgress = Challenge.ChallengeStatus.Completed,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now,
                Workout = wk,
                ChallengerResult = res,
                ContenderResult = res3
            };

            var chlg5 = new Challenge
            {
                Id = 5,
                ChallengeProgress = Challenge.ChallengeStatus.Rejected,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            var chlg6 = new Challenge
            {
                Id = 6,
                ChallengeProgress = Challenge.ChallengeStatus.Completed,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now,
                Workout = wk,
                ContenderResult = res2,
                ChallengerResult = res4
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.Workouts.Add(wk);
            _context.WorkoutResults.Add(res);
            _context.WorkoutResults.Add(res2);
            _context.WorkoutResults.Add(res3);
            _context.WorkoutResults.Add(res4);
            _context.Challenges.Add(chlg);
            _context.Challenges.Add(chlg2);
            _context.Challenges.Add(chlg3);
            _context.Challenges.Add(chlg4);
            _context.Challenges.Add(chlg5);
            _context.Challenges.Add(chlg6);

            _context.SaveChangesAsync().GetAwaiter();

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
             }, "mockId"));

            _cut.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            //Test
            var result = await _cut.IndexAsync(null, null, null, null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<ApplicationUser>>(
                viewResult.ViewData.Model);

            result = await _cut.Record(null, null, null, null, null, null, usr2.Id);

            var viewResult2 = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Challenge>>(
                viewResult.ViewData.Model);

            //Reset
            _context.Remove(chlg);
            _context.Remove(chlg2);
            _context.Remove(chlg3);
            _context.Remove(chlg4);
            _context.Remove(chlg5);
            _context.Remove(chlg6);
            _context.Remove(res);
            _context.Remove(res2);
            _context.Remove(res3);
            _context.Remove(res4);
            _context.Remove(wk);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
            _context.SaveChangesAsync().GetAwaiter();



           

            //var res = await cont.SendRequestPost("guid-user2");
            ////
            ////  invalid send request
            //res = await cont.SendRequestPost(null);
            //Assert.IsType<NotFoundResult>(res);
            //res = await cont.SendRequestPost("FakeUser");
            //Assert.IsType<NotFoundResult>(res);

            //var data = await _context.FriendRequests.FirstOrDefaultAsync();

            //Assert.True(data != null);

            //var res2 = await cont.Details("guid-user1");
            ////
            ////  invalid details request
            //res2 = await cont.Details(null);
            //Assert.IsType<NotFoundResult>(res2);
            //res2 = await cont.Details("FakeUser");
            //Assert.IsType<NotFoundResult>(res2);

            //data = await _context.FriendRequests.FirstOrDefaultAsync(x => x.RequestedById == "guid-user1");

            //Assert.True(data != null);

            //var res3 = await cont.ViewRequest("guid-user1", "guid-user2");
            ////
            ////  invalid view request
            //res3 = await cont.ViewRequest(null, "guid-user2");
            //Assert.IsType<NotFoundResult>(res3);
            //res3 = await cont.ViewRequest("guid-user1", null);
            //Assert.IsType<NotFoundResult>(res3);
            //res3 = await cont.ViewRequest("BozoTheClown", "FakeUser");
            //Assert.IsType<NotFoundResult>(res3);

            //data = await _context.FriendRequests.FirstOrDefaultAsync(x => x.RequestedById == "guid-user1" && x.RequestedForId == "guid-user2");

            //Assert.True(data != null);

            //var res4 = await cont.Profile("guid-user2");
            ////
            ////  invalid profile request
            //res4 = await cont.Profile(null);
            //Assert.IsType<NotFoundResult>(res4);
            //res4 = await cont.Profile("FakeUser");
            //Assert.IsType<NotFoundResult>(res4);

            //data = await _context.FriendRequests.FirstOrDefaultAsync(x => x.RequestedForId == "guid-user2");

            ////
            ////  delete
            //var res5 = await cont.DeleteConfirmed("BozoTheClown", "FakeUser");
            //Assert.IsType<NotFoundResult>(res5);
            //res5 = await cont.DeleteConfirmed("guid-user1", "FakeUser");
            //Assert.IsType<NotFoundResult>(res5);
            //res5 = await cont.DeleteConfirmed("FakeUser", "guid-user2");
            //Assert.IsType<NotFoundResult>(res5);

            //res5 = await cont.DeleteConfirmed("guid-user1", "guid-user2");
            //data = await _context.FriendRequests.FirstOrDefaultAsync(x => x.RequestedById == "guid-user1");
            //Assert.True(data == null);

            //_context.Database.EnsureDeleted();
        }
    }
}
