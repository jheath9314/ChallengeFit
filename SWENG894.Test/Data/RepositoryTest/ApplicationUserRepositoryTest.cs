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
    public class ApplicationUserRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IApplicationUserRepository _cut;

        public ApplicationUserRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryAppUser");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new ApplicationUserRepository(_context);
        }

        #region Methods Inherited From Repository

        [Fact]
        public async void GetAsyncIntTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            _context.ApplicationUsers.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            //GetAsync(int id) not impelemented. AppUser key is a guid.
            await Assert.ThrowsAsync<NotImplementedException>(() => _cut.GetAsync(1));
            Assert.Equal(1, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetFirstOrDefaultAsyncTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            _context.ApplicationUsers.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetFirstOrDefaultAsync(x => x.Id == "user-guid");
            Assert.NotNull(result);
            Assert.Equal("user1@psu.edu", result.Email);
            Assert.Equal(1, _context.Users.Count());

            result = await _cut.GetFirstOrDefaultAsync(x => x.Id == "user-guid2");
            Assert.Null(result);
            Assert.Equal(1, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetAllAsyncTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "user-guid2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var usr3 = new ApplicationUser
            {
                Id = "user-guid3",
                UserName = "user3@psu.edu",
                Email = "user3@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Three",
                ZipCode = "22222"
            };

            _context.Users.Add(usr1);
            _context.Users.Add(usr2);
            _context.Users.Add(usr3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAllAsync();
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Users.Count());

            result = await _cut.GetAllAsync(x => x.ZipCode == "22222");
            Assert.Equal(2, result.Count());
            Assert.Equal(3, _context.Users.Count());

            result = await _cut.GetAllAsync(orderBy: x => x.OrderBy(x => x.Id));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Users.Count());
            Assert.Equal("user-guid", result.ElementAt(0).Id);

            result = await _cut.GetAllAsync(orderBy: x => x.OrderByDescending(x => x.Id));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Users.Count());
            Assert.Equal("user-guid3", result.ElementAt(0).Id);

            //Reset
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void AddAsyncTests()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            //Test
            await _cut.AddAsync(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Users.FirstOrDefault(x => x.Id == "user-guid");
            Assert.NotNull(result);
            Assert.Equal("user1@psu.edu", result.Email);
            Assert.Equal(1, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoveAsyncIntTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            _context.ApplicationUsers.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            //GetAsync(int id) not impelemented. AppUser key is a guid.
            await Assert.ThrowsAsync<NotImplementedException>(() => _cut.RemoveAsync(1));
            Assert.Equal(1, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoveAsyncObjTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "user-guid2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var usr3 = new ApplicationUser
            {
                Id = "user-guid3",
                UserName = "user3@psu.edu",
                Email = "user3@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Three",
                ZipCode = "22222"
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync(usr2);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Users.FirstOrDefault(x => x.Id == "user-guid2");
            Assert.Null(result);
            Assert.Equal(2, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.Remove(usr3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoverangeAsyncTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "user-guid2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var usr3 = new ApplicationUser
            {
                Id = "user-guid3",
                UserName = "user3@psu.edu",
                Email = "user3@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Three",
                ZipCode = "22222"
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            List<ApplicationUser> list = new List<ApplicationUser>() { usr1, usr2 };
            await _cut.RemoveRangeAsync(list);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Users.FirstOrDefault(x => x.Id == "user-guid");
            Assert.Null(result);
            result = _context.Users.FirstOrDefault(x => x.Id == "user-guid2");
            Assert.Null(result);
            Assert.Equal(1, _context.Users.Count());

            //Reset
            _context.Remove(usr3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void ObjectExistsIntTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            _context.ApplicationUsers.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            //ObjectExists(int id) not impelemented. AppUser key is a guid.
            Assert.Throws<NotImplementedException>(() => _cut.ObjectExists(1));
            Assert.Equal(1, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion

        #region Methods Specific To Interface

        [Fact]
        public async void GetAsyncStringTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            _context.ApplicationUsers.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAsync("user-guid");
            Assert.NotNull(result);
            Assert.Equal("user1@psu.edu", result.Email);
            Assert.Equal(1, _context.Users.Count());

            result = await _cut.GetAsync("user-guid2");
            Assert.Null(result);
            Assert.Equal(1, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoveAsyncStrIdTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            var usr2 = new ApplicationUser
            {
                Id = "user-guid2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var usr3 = new ApplicationUser
            {
                Id = "user-guid3",
                UserName = "user3@psu.edu",
                Email = "user3@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Three",
                ZipCode = "22222"
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync("user-guid2");
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Users.FirstOrDefault(x => x.Id == "user-guid2");
            Assert.Null(result);
            Assert.Equal(2, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.Remove(usr3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void ObjectExistsStrTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            _context.Users.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            Assert.True(_cut.ObjectExists("user-guid"));
            Assert.False(_cut.ObjectExists("user-guid2"));

            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void UpdateAsyncTest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111"
            };

            _context.ApplicationUsers.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            //ObjectExists(int id) not impelemented. AppUser key is a guid.
            Assert.Throws<NotImplementedException>(() => _cut.UpdateAsync(usr1));
            Assert.Equal(1, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public void GetAllUserRatingsTest()
        {
            var ratings = _cut.GetAllUserRatings();
            Assert.Empty(ratings);

            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111",
                Rating = 1500
            };

            _context.ApplicationUsers.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            ratings = _cut.GetAllUserRatings();
            Assert.Single(ratings);
            Assert.Equal(1500, ratings[0]);
 
            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public void GetRecordTest()
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

            var usr2 = new ApplicationUser
            {
                Id = "user2",
                UserName = "user2@psu.edu",
                Email = "user2@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Two",
                ZipCode = "22222"
            };

            var usr3 = new ApplicationUser
            {
                Id = "user3",
                UserName = "user3@psu.edu",
                Email = "user3@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Three",
                ZipCode = "33333"
            };

            var wk = new Workout()
            {
                Id = 1,
                Name = "First",
                Published = true,
                ScoringType = Workout.Scoring.Rounds,
                User = usr1
            };

            var res = new WorkoutResult()
            {
                Id = 1,
                User = usr1,
                Workout = wk,
                Score = 10
            };

            var res2 = new WorkoutResult()
            {
                Id = 2,
                User = usr1,
                Workout = wk,
                Score = 20
            };

            var res3 = new WorkoutResult()
            {
                Id = 3,
                User = usr2,
                Workout = wk,
                Score = 30
            };

            var res4 = new WorkoutResult()
            {
                Id = 4,
                User = usr2,
                Workout = wk,
                Score = 40
            };

            var chlg = new Challenge
            {
                Id = 1,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            var chlg2 = new Challenge
            {
                Id = 2,
                ChallengeProgress = Challenge.ChallengeStatus.CompletedByChallenger,
                Challenger = usr1,
                Contender = usr3,
                CreateDate = DateTime.Now
            };

            var chlg3 = new Challenge
            {
                Id = 3,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                Challenger = usr2,
                Contender = usr3,
                CreateDate = DateTime.Now
            };

            var chlg4 = new Challenge
            {
                Id = 4,
                ChallengeProgress = Challenge.ChallengeStatus.Completed,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now,
                Workout = wk,
                ChallengerResult = res,
                ContenderResult = res3
            };

            var chlg5 = new Challenge
            {
                Id = 5,
                ChallengeProgress = Challenge.ChallengeStatus.Rejected,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            var chlg6 = new Challenge
            {
                Id = 6,
                ChallengeProgress = Challenge.ChallengeStatus.Completed,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now,
                Workout = wk,
                ContenderResult = res2,
                ChallengerResult = res4
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.Workouts.Add(wk);
            _context.WorkoutResults.Add(res);
            _context.WorkoutResults.Add(res2);
            _context.WorkoutResults.Add(res3);
            _context.WorkoutResults.Add(res4);
            _context.Challenges.Add(chlg);
            _context.Challenges.Add(chlg2);
            _context.Challenges.Add(chlg3);
            _context.Challenges.Add(chlg4);
            _context.Challenges.Add(chlg5);
            _context.Challenges.Add(chlg6);
            
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var data = _cut.GetRecord(usr1.Id, usr2.Id);
            Assert.Equal(2, data.Count());

            data = _cut.GetRecord(usr1.Id, usr3.Id);
            Assert.Empty(data);

            //Reset
            _context.Remove(chlg);
            _context.Remove(chlg2);
            _context.Remove(chlg3);
            _context.Remove(chlg4);
            _context.Remove(chlg5);
            _context.Remove(chlg6);
            _context.Remove(res);
            _context.Remove(res2);
            _context.Remove(res3);
            _context.Remove(res4);
            _context.Remove(wk);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);           
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public void UpdateRatingest()
        {
            //Setup
            var usr1 = new ApplicationUser
            {
                Id = "user-guid",
                UserName = "user1@psu.edu",
                Email = "user1@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "One",
                ZipCode = "11111",
                Rating = 1500
            };

            _context.ApplicationUsers.Add(usr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            _cut.UpdateRating(1400, usr1.Id);
            _context.SaveChanges();

            var result = _context.ApplicationUsers.FirstOrDefault(x => x.Id == usr1.Id);
            Assert.NotNull(result);
            Assert.Equal(1400, result.Rating);
            Assert.Equal(1, _context.Users.Count());

            //Reset
            _context.Remove(usr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion
    }
}
