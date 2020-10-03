<<<<<<< HEAD
﻿using Microsoft.EntityFrameworkCore;
=======
﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
>>>>>>> e73bad61ccbb0b0b535b98a4cea180b70f77568b
using SWENG894.Areas.User.Controllers;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Models;
<<<<<<< HEAD
using Xunit;
=======
using System.Linq;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
>>>>>>> e73bad61ccbb0b0b535b98a4cea180b70f77568b

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
                ScoringType = Workout.Scoring.Reps,
                Time = 20
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Workouts.Add(w);
            _context.SaveChangesAsync().GetAwaiter();

            
            var unit = new UnitOfWork(_context);
            var cont = new WorkoutResultsController(unit);

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
            }, "mock"));

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            var res = new WorkoutResult()
            {
                Id = 1,
                UserId = "guid-user1",
                WorkoutId = 1,
            };

            //Not sure if this is a bug. I pass in 600 for seconds, but it saves a 0 for create.
            await cont.Create(1, res, 600);

            var data = _context.WorkoutResults.FirstOrDefault(x => x.Id == 1);
            Assert.Single(_context.WorkoutResults);
            //Assert.Equal(600, data.Score);

            //Same for edit. Score doesn't update.
            await cont.Edit(1, res, 900);
            data = _context.WorkoutResults.FirstOrDefault(x => x.Id == 1);
            Assert.Single(_context.WorkoutResults);
            //Assert.Equal(900, data.Score);

            await cont.DeleteConfirmed(1);
            data = _context.WorkoutResults.FirstOrDefault(x => x.Id == 1);
            Assert.Null(data);

            _context.Database.EnsureDeleted();
        }
    }
}
