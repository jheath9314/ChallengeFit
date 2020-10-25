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

namespace SWENG894.Test.Data.RepositoryTest
{
    public class WorkoutFavoritesRepositoryTest
    {

        private readonly ApplicationDbContext _context;
        private readonly IWorkoutFavoriteRepository _cut;

        public WorkoutFavoritesRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryWorkoutFavorites");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new WorkoutFavoriteRepository(_context);
        }

        #region Methods Inherited From Repository

        [Fact]
        public async void GetAsyncTest()
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

            var wrk1 = new Workout
            {
                Id = 1,
                Name = "Workout",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 20
            };

            var fave1 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk1
            };

            _context.ApplicationUsers.Add(usr1);
            _context.Workouts.Add(wrk1);
            _context.WorkoutFavorites.Add(fave1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await Assert.ThrowsAsync<NotImplementedException>(() => _cut.GetAsync(1));

            //Reset
            _context.Remove(fave1);
            _context.Remove(wrk1);
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetFirstOrDefaultAsyncTest()
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

            var wrk1 = new Workout
            {
                Id = 1,
                Name = "Workout",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 20
            };

            var fave1 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk1
            };

            _context.ApplicationUsers.Add(usr1);
            _context.Workouts.Add(wrk1);
            _context.WorkoutFavorites.Add(fave1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetFirstOrDefaultAsync(x => x.UserId == "user1" && x.WorkoutId == 1, includeProperties: "Workout,User");
            Assert.NotNull(result);
            Assert.Equal("user1", result.User.Id);
            Assert.Equal(1, result.Workout.Id);
            Assert.Equal(1, _context.WorkoutFavorites.Count());

            result = await _cut.GetFirstOrDefaultAsync(x => x.UserId == "user1" && x.WorkoutId == 2, includeProperties: "Workout,User");
            Assert.Null(result);
            Assert.Equal(1, _context.WorkoutFavorites.Count());

            //Reset
            _context.Remove(fave1);
            _context.Remove(wrk1);
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetAllAsyncTest()
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

            var wrk1 = new Workout
            {
                Id = 1,
                Name = "Workout",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 20
            };

            var wrk2 = new Workout
            {
                Id = 2,
                Name = "Workout2",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 30
            };

            var wrk3 = new Workout
            {
                Id = 3,
                Name = "Workout3",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 50
            };

            var fave1 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk1
            };

            var fave2 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk2
            };

            var fave3 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk3
            };

            _context.ApplicationUsers.Add(usr1);
            _context.Workouts.Add(wrk1);
            _context.Workouts.Add(wrk2);
            _context.Workouts.Add(wrk3);
            _context.WorkoutFavorites.Add(fave1);
            _context.WorkoutFavorites.Add(fave2);
            _context.WorkoutFavorites.Add(fave3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAllAsync();
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.WorkoutFavorites.Count());

            result = await _cut.GetAllAsync(x => x.UserId == "user1" && x.WorkoutId == 1, includeProperties: "User,Workout");
            Assert.Single(result);
            Assert.Equal("One", result.ElementAt(0).User.LastName);
            Assert.Equal(3, _context.WorkoutFavorites.Count());

            result = await _cut.GetAllAsync(orderBy: x => x.OrderBy(x => x.WorkoutId));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.WorkoutFavorites.Count());
            Assert.Equal(1, result.ElementAt(0).WorkoutId);

            result = await _cut.GetAllAsync(orderBy: x => x.OrderByDescending(x => x.WorkoutId));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.WorkoutFavorites.Count());
            Assert.Equal(3, result.ElementAt(0).WorkoutId);

            //Reset
            _context.Remove(fave1);
            _context.Remove(fave2);
            _context.Remove(fave3);
            _context.Remove(wrk1);
            _context.Remove(wrk2);
            _context.Remove(wrk3);
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void AddAsyncTests()
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

            var wrk1 = new Workout
            {
                Id = 1,
                Name = "Workout",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 20
            };

            _context.ApplicationUsers.Add(usr1);
            _context.Workouts.Add(wrk1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var fave1 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk1
            };

            await _cut.AddAsync(fave1);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.WorkoutFavorites.FirstOrDefault(x => x.UserId == "user1" && x.WorkoutId == 1);
            Assert.NotNull(result);
            Assert.Equal("user1", result.UserId);
            Assert.Equal(1, _context.WorkoutFavorites.Count());

            //Reset
            _context.Remove(fave1);
            _context.Remove(usr1);
            _context.Remove(wrk1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoveAsyncIdTest()
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

            var wrk1 = new Workout
            {
                Id = 1,
                Name = "Workout",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 20
            };

            var wrk2 = new Workout
            {
                Id = 2,
                Name = "Workout2",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 30
            };

            var wrk3 = new Workout
            {
                Id = 3,
                Name = "Workout3",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 50
            };

            var fave1 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk1
            };

            var fave2 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk2
            };

            var fave3 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk3
            };

            _context.ApplicationUsers.Add(usr1);
            _context.Workouts.Add(wrk1);
            _context.Workouts.Add(wrk2);
            _context.Workouts.Add(wrk3);
            _context.WorkoutFavorites.Add(fave1);
            _context.WorkoutFavorites.Add(fave2);
            _context.WorkoutFavorites.Add(fave3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await Assert.ThrowsAsync<NotImplementedException>(() => _cut.RemoveAsync(1));

            //Reset
            _context.Remove(fave1);
            _context.Remove(fave2);
            _context.Remove(fave3);
            _context.Remove(usr1);
            _context.Remove(wrk1);
            _context.Remove(wrk2);
            _context.Remove(wrk3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoveAsyncObjTest()
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

            var wrk1 = new Workout
            {
                Id = 1,
                Name = "Workout",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 20
            };

            var wrk2 = new Workout
            {
                Id = 2,
                Name = "Workout2",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 30
            };

            var wrk3 = new Workout
            {
                Id = 3,
                Name = "Workout3",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 50
            };

            var fave1 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk1
            };

            var fave2 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk2
            };

            var fave3 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk3
            };

            _context.ApplicationUsers.Add(usr1);
            _context.Workouts.Add(wrk1);
            _context.Workouts.Add(wrk2);
            _context.Workouts.Add(wrk3);
            _context.WorkoutFavorites.Add(fave1);
            _context.WorkoutFavorites.Add(fave2);
            _context.WorkoutFavorites.Add(fave3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync(fave2);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.WorkoutFavorites.FirstOrDefault(x => x.UserId == "user1" && x.WorkoutId == 2);
            Assert.Null(result);
            Assert.Equal(2, _context.WorkoutFavorites.Count());

            //Reset
            _context.Remove(fave1);
            _context.Remove(fave3);
            _context.Remove(usr1);
            _context.Remove(wrk1);
            _context.Remove(wrk2);
            _context.Remove(wrk3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoverangeAsyncTest()
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

            var wrk1 = new Workout
            {
                Id = 1,
                Name = "Workout",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 20
            };

            var wrk2 = new Workout
            {
                Id = 2,
                Name = "Workout2",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 30
            };

            var wrk3 = new Workout
            {
                Id = 3,
                Name = "Workout3",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 50
            };

            var fave1 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk1
            };

            var fave2 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk2
            };

            var fave3 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk3
            };

            _context.ApplicationUsers.Add(usr1);
            _context.Workouts.Add(wrk1);
            _context.Workouts.Add(wrk2);
            _context.Workouts.Add(wrk3);
            _context.WorkoutFavorites.Add(fave1);
            _context.WorkoutFavorites.Add(fave2);
            _context.WorkoutFavorites.Add(fave3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            List<WorkoutFavorite> list = new List<WorkoutFavorite>() { fave1, fave2 };
            await _cut.RemoveRangeAsync(list);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.WorkoutFavorites.FirstOrDefault(x => x.UserId == "user1" && x.WorkoutId == 1);
            Assert.Null(result);
            result = _context.WorkoutFavorites.FirstOrDefault(x => x.UserId == "user1" && x.WorkoutId == 2);
            Assert.Null(result);
            Assert.Equal(1, _context.WorkoutFavorites.Count());

            //Reset
            _context.Remove(fave3);
            _context.Remove(usr1);
            _context.Remove(wrk1);
            _context.Remove(wrk2);
            _context.Remove(wrk3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void ObjectExistsTest()
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

            var wrk1 = new Workout
            {
                Id = 1,
                Name = "Workout",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 20
            };

            var wrk2 = new Workout
            {
                Id = 2,
                Name = "Workout2",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 30
            };

            var wrk3 = new Workout
            {
                Id = 3,
                Name = "Workout3",
                Published = true,
                ScoringType = Workout.Scoring.Reps,
                Time = 50
            };

            var fave1 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk1
            };

            var fave2 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk2
            };

            var fave3 = new WorkoutFavorite
            {
                User = usr1,
                Workout = wrk3
            };

            _context.ApplicationUsers.Add(usr1);
            _context.Workouts.Add(wrk1);
            _context.Workouts.Add(wrk2);
            _context.Workouts.Add(wrk3);
            _context.WorkoutFavorites.Add(fave1);
            _context.WorkoutFavorites.Add(fave2);
            _context.WorkoutFavorites.Add(fave3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            Assert.Throws<NotImplementedException>(() => _cut.ObjectExists(1));

            //Reset
            _context.Remove(fave1);
            _context.Remove(fave2);
            _context.Remove(fave3);
            _context.Remove(wrk1);
            _context.Remove(wrk2);
            _context.Remove(wrk3);
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion

        #region Methods Specific To Interface

        #endregion
    }
}
