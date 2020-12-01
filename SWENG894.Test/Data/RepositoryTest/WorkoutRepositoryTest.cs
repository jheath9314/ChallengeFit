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
using System.Security.Claims;
using System.Security.Principal;

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

        [Fact]
        public async void GetUserWorkoutsTest()
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

            var work = new Workout();
            work.Id = 0;
            work.Name = "First";
            work.Published = true;
            work.UserId = "guid-user1";

            var work_2 = new Workout();
            work_2.Id = 0;
            work_2.Name = "Second";
            work_2.Published = true;
            work_2.UserId = "2";

            var work_3 = new Workout();
            work_3.Id = 0;
            work_3.Name = "Second1";
            work_3.Published = true;
            work_3.UserId = "guid-user1";


            await _cut.AddAsync(work);
            await _context.SaveChangesAsync();
            var tempWorkout = await _context.Workouts.FirstOrDefaultAsync();
            var workoutId = tempWorkout.Id;

            await _cut.AddAsync(work_2);
            await _cut.AddAsync(work_3);
            await _context.SaveChangesAsync();

            var workouts = _cut.GetUserWorkouts("desc", "Second", "guid-user1", true);
            var workoutList = workouts.ToList();
            Assert.True(workoutList.Count == 1);

            workouts = _cut.GetUserWorkouts("", "", "guid-user1", true);
            workoutList = workouts.ToList();
            Assert.True(workoutList.Count == 2);

            workouts = _cut.GetUserWorkouts("desc", "First", "2", true);
            workoutList = workouts.ToList();
            Assert.True(workoutList.Count == 0);

            var fav = new WorkoutFavorite
            {
                UserId = "guid-user1",
                WorkoutId = 2,
            };

            _context.WorkoutFavorites.Add(fav);
            await _context.SaveChangesAsync();

            workouts = _cut.GetUserWorkouts("", "", "guid-user1", true);
            workoutList = workouts.ToList();
            Assert.True(workoutList.Count == 3);
        }

        [Fact]
        public async void FindNewWorkoutsTest()
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



            var work = new Workout();
            work.Id = 0;
            work.Name = "First";
            work.Published = true;
            work.UserId = "guid-user1";

            var work_2 = new Workout();
            work_2.Id = 0;
            work_2.Name = "Second";
            work_2.Published = true;
            work_2.UserId = "2";

            var work_3 = new Workout();
            work_2.Id = 0;
            work_2.Name = "Second1";
            work_2.Published = true;
            work_2.UserId = "guid-user1";


            await _cut.AddAsync(work);
            await _context.SaveChangesAsync();
            var tempWorkout = await _context.Workouts.FirstOrDefaultAsync();
            var workoutId = tempWorkout.Id;
            await _cut.AddAsync(work_2);
            await _cut.AddAsync(work_3);
            await _context.SaveChangesAsync();

            var workouts = _cut.FindNewWorkouts("desc", "First", "guid-user1");
            var workoutList = workouts.ToList();

            Assert.True(workoutList.Count == 1);
            Assert.False(workoutList.ElementAt(0).IsFavorite);

            var fav = new WorkoutFavorite
            {
                UserId = "guid-user1",
                WorkoutId = workoutId,
            };

            _context.WorkoutFavorites.Add(fav);
            await _context.SaveChangesAsync();

            workouts = _cut.FindNewWorkouts("", "", "guid-user1");
            workoutList = workouts.ToList();

            Assert.True(workoutList.Count == 2);
            Assert.True(workoutList.ElementAt(0).IsFavorite);
        }
    }
}
