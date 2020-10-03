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
    public class ExercisesControllerTest
    {
        private readonly ApplicationDbContext _context;
        public ExercisesControllerTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryExercisesController");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async void ExerciseControllerTestOp()
        {
            var unit = new UnitOfWork(_context);
            var cont = new ExercisesController(unit);
            Workout workout = new Workout();
            workout.Id = 0;
            workout.Name = "Test";

            await unit.Workout.AddAsync(workout);
            await _context.SaveChangesAsync();

            var data = await _context.Workouts.FirstOrDefaultAsync();

            Exercise ex = new Exercise();
            ex.Id = 0;
            ex.Exer = Exercise.Exercises.AirSquat;

            await cont.Create(data.Id, ex);
            await _context.SaveChangesAsync();


            ex = await unit.Exercise.GetFirstOrDefaultAsync();

            Assert.True(ex != null);

            ex.Exer = Exercise.Exercises.Clean;
            await cont.Edit(ex.Id, ex);

            ex = await unit.Exercise.GetFirstOrDefaultAsync();
            Assert.True(ex.Exer == Exercise.Exercises.Clean);

            await cont.DeleteConfirmed(ex.Id);
            await _context.SaveChangesAsync();
            ex = await unit.Exercise.GetFirstOrDefaultAsync();

            Assert.True(ex == null);
        }
    }
}
