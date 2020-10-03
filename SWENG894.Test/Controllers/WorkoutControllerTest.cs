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
            var unit = new UnitOfWork(_context);
            var cont = new WorkoutsController(unit);
            Workout workout = new Workout();
            workout.Name = "Test";
            var res = await cont.Create(workout, 22);
            var data = await _context.Workouts.FirstOrDefaultAsync();

            Assert.True(data != null);

            data.Name = "Updated";

            await cont.Edit(data.Id, data);

            data = await _context.Workouts.FirstOrDefaultAsync();

            Assert.True(data.Name == "Updated");

            await cont.DeleteConfirmed(data.Id);

            data = await _context.Workouts.FirstOrDefaultAsync();

            Assert.True(data == null);
        }
    }
}
