using System;
using System.Collections.Generic;
using System.Text;
using SWENG894.Data;
using SWENG894.Data.Repository;
using Microsoft.EntityFrameworkCore;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace SWENG894.Test.Data.RepositoryTest
{
    public class WorkoutRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkoutRepository _cut;
        public WorkoutRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryWorkoutRepository");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new WorkoutRepository(_context);
        }

        [Fact]
        public async void workoutUpdateTest()
        {
            var work = new Workout();
            work.Id = 0;
            work.Name = "First";

            await _cut.AddAsync(work);
            await _context.SaveChangesAsync();
            work = await _cut.GetFirstOrDefaultAsync();

            work.Name = "Updated";
            _cut.UpdateAsync(work);

            Thread.Sleep(100);

            var updatedWorkout = await _cut.GetFirstOrDefaultAsync();

            Assert.True(updatedWorkout.Name == "Updated");

        }
        
        [Fact]
        public async void WorkoutSearchTest()
        {
            var work = new Workout();
            work.Id = 0;
            work.Name = "First";
            work.Published = true;

            var work_2 = new Workout();
            work_2.Id = 0;
            work_2.Name = "Second";
            work_2.Published = true;

            await _cut.AddAsync(work);
            await _cut.AddAsync(work_2);
            await _context.SaveChangesAsync();

            var workouts =  _cut.GetAllWorkouts("desc", "");
            var workoutList = workouts.ToList();

            Assert.True(workoutList.Count > 1);
            Assert.True(workoutList[0].Name == "Second");
            Assert.True(workoutList[1].Name == "First");


            workouts = _cut.GetAllWorkouts("asc", "");
            workoutList = workouts.ToList();
            Assert.True(workoutList[0].Name == "First");
            Assert.True(workoutList[1].Name == "Second");

            workouts = _cut.GetAllWorkouts("desc", "First");
            workoutList = workouts.ToList();

            Assert.True(workoutList.Count == 1);

        }

    }
}
