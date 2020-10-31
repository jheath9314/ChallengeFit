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
        public ChallengesControllerTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryChallengesControllerTest");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
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

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            await _context.SaveChangesAsync();

            var unit = new UnitOfWork(_context);

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
                }, "mock"));

            var cont = new ChallengesController(unit);

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            var WorkoutController = new WorkoutsController(unit);

            WorkoutController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            }; 
            Workout workout = new Workout();
            workout.Name = "Test";
            workout.ScoringType = Workout.Scoring.Time;
            workout.UserId = usr1.Id;
            var res = await WorkoutController.Create(workout, 22);
            _context.SaveChanges();
            var data = await _context.Workouts.FirstOrDefaultAsync();

            var Challenge = new ChallengeViewModel
            {
                ChallengerId = usr1.Id,
                Challenger = usr1,
                ContenderId = usr2.Id,
                Contender = usr2,
                WorkoutId = data.Id,
            };


            await cont.Create(Challenge);

            var ChallengeData = await _context.Challenges.FirstOrDefaultAsync();
            Assert.True(ChallengeData.ChallengerId == usr1.Id);
            Assert.True(ChallengeData.ContenderId == usr2.Id);
            Assert.True(ChallengeData.WorkoutId == data.Id);


        }
    }
}
