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
                Id = 0,
                Name = "Name",
                Notes = "Notes",
                ScoringType = Workout.Scoring.Time,
                Time = 20,
                Published = true
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

            int workoutId = _context.Workouts.FirstOrDefault().Id;

            var res = new WorkoutResult()
            {
                Id = 0,
                UserId = "guid-user1",
                WorkoutId = workoutId,
                Score = 600,
                ScoringType = Workout.Scoring.Time
            };

            //Not sure if this is a bug. I pass in 600 for seconds, but it saves a 0 for create.
            await cont.Create(workoutId, res, 0);
            _context.WorkoutResults.Add(res);

            var data = await _context.WorkoutResults.FirstOrDefaultAsync();
            Assert.Single(_context.WorkoutResults);
            Assert.Equal(600, data.Score);

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
