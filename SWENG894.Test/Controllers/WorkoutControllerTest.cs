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
using System.Threading.Tasks;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SWENG894.Test.Controllers
{
    public class WorkoutControllerTest
    {
        private readonly ApplicationDbContext _context;
        public WorkoutControllerTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryWorkoutController");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async void WorkoutControllerOpTest()
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
            _context.ApplicationUsers.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            var unit = new UnitOfWork(_context);
            var cont = new WorkoutsController(unit);

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
            }, "mock"));

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            
            Workout workout = new Workout();
            workout.Name = "Test";
            workout.ScoringType = Workout.Scoring.Time;


            var res = await cont.Create(workout, 22);
            var data = await _context.Workouts.FirstOrDefaultAsync();

            Assert.True(data != null);

            data.Name = "Updated";

            //
            //  edit with invalid id
            res = await cont.Edit(-22, data);
            Assert.IsType<NotFoundResult>(res);

            //
            //  edit with invalid userid
            var userId = data.UserId;
            data.UserId = "wrong";
            res = await cont.Edit(data.Id, data);
            Assert.IsType<ForbidResult>(res);

            //
            //  edit the workout
            data.UserId = userId;
            await cont.Edit(data.Id, data);

            data = await _context.Workouts.FirstOrDefaultAsync();

            Assert.True(data.Name == "Updated");
            Assert.True(data.Published == false);

            //
            //  publish with no exerices
            res = await cont.Publish(data.Id);
            Assert.IsType<ForbidResult>(res);
            Assert.True(data.Published == false);

            //
            //  add an exercise to the workout and publish
            Exercise ex = new Exercise();
            ex.Id = 0;
            ex.Exer = Exercise.Exercises.AirSquat;

            var exCont = new ExercisesController(unit);
            await exCont.Create(data.Id, ex);
            await _context.SaveChangesAsync();
            res = await cont.Publish(data.Id);
            Assert.True(data.Published == true);

            //
            //  try to edit after publishing
            res = await cont.Edit(data.Id, data);
            Assert.IsType<ForbidResult>(res);

            //
            //  delete workout
            await cont.DeleteConfirmed(data.Id);

            data = await _context.Workouts.FirstOrDefaultAsync();

            Assert.True(data == null);
        }
    }
}
