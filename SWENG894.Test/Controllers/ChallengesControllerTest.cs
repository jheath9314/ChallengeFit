using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Org.BouncyCastle.Crypto.Digests;
using SWENG894.Areas.User.Controllers;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System.Linq;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SWENG894.ViewModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SWENG894.Test.Controllers
{
    public class ChallengesControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ChallengesController _cut;
        public ChallengesControllerTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryChallengeControllerTest");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _unitOfWork = new UnitOfWork(_context);
            _cut = new ChallengesController(_unitOfWork);
        }
        
        [Fact]
        public async void CreateChallengeTest()
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

            var wk = new Workout()
            {
                Id = 1,
                Published = true,
                Name = "test",
                ScoringType = Workout.Scoring.Time,
                Notes = "notes",
                User = usr1,
                Time = 20
            };

            var ch = new Challenge()
            {
                Id = 1,
                Challenger = usr1,
                Contender = usr2,
                ChallengeProgress = SWENG894.Models.Challenge.ChallengeStatus.Accepted,
                CreateDate = DateTime.Now,
                Workout = wk
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Workouts.Add(wk);
            _context.Challenges.Add(ch);
            _context.SaveChangesAsync().GetAwaiter();

            //var unit = new UnitOfWork(_context);

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
                }, "mock"));


            _cut.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            //var WorkoutController = new WorkoutsController(unit);

            //WorkoutController.ControllerContext = new ControllerContext()
            //{
            //    HttpContext = new DefaultHttpContext() { User = loggedInUser }
            //}; 
            //Workout workout = new Workout();
            //workout.Name = "Test";
            //workout.ScoringType = Workout.Scoring.Time;
            //workout.UserId = usr1.Id;
            //var res = await WorkoutController.Create(workout, 22);
            //_context.SaveChanges();

            //var data = await _context.Workouts.FirstOrDefaultAsync();

            var Challenge = new ChallengeViewModel
            {
                Challenger = usr1,
                Contender = usr2,
                WorkoutId = wk.Id
            };

            await _cut.Create(Challenge);

            var ChallengeData = await _context.Challenges.FirstOrDefaultAsync();
            Assert.True(ChallengeData.ChallengerId == usr1.Id);
            Assert.True(ChallengeData.ContenderId == usr2.Id);
            Assert.True(ChallengeData.WorkoutId == wk.Id);

            var response = _cut.Details(null, SWENG894.Models.Challenge.ChallengeStatus.New).Result;
            Assert.IsAssignableFrom<NotFoundResult>(response);

            response = _cut.Details(10, SWENG894.Models.Challenge.ChallengeStatus.New).Result;
            Assert.IsAssignableFrom<NotFoundResult>(response);

            response = _cut.Details(1, SWENG894.Models.Challenge.ChallengeStatus.Accepted).Result;
            var data = await _context.Challenges.FirstOrDefaultAsync();
            Assert.Equal(SWENG894.Models.Challenge.ChallengeStatus.Accepted, data.ChallengeProgress);

            data.ChallengeProgress = SWENG894.Models.Challenge.ChallengeStatus.New;
            data.Challenger = usr2;
            data.Contender = usr1;
            _context.Challenges.Update(data);
            _context.SaveChangesAsync().GetAwaiter();

            response = _cut.Details(1, SWENG894.Models.Challenge.ChallengeStatus.Rejected).Result;
            data = await _context.Challenges.FirstOrDefaultAsync();
            Assert.Equal(SWENG894.Models.Challenge.ChallengeStatus.Rejected, data.ChallengeProgress);

            data.ChallengeProgress = SWENG894.Models.Challenge.ChallengeStatus.New;
            data.Challenger = usr2;
            data.Contender = usr1;
            _context.Challenges.Update(data);
            _context.SaveChangesAsync().GetAwaiter();

            response = _cut.Details(1, SWENG894.Models.Challenge.ChallengeStatus.Accepted).Result;
            data = await _context.Challenges.FirstOrDefaultAsync();
            Assert.Equal(SWENG894.Models.Challenge.ChallengeStatus.Accepted, data.ChallengeProgress);

            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(wk);
            _context.Remove(ch);
            _context.Remove(ChallengeData);
            _context.SaveChangesAsync().GetAwaiter();
        }
    }
}
