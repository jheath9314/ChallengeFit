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
    public class ChallengeRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IChallengeRepository _cut;

        public ChallengeRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryChallenges");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new ChallengeRepository(_context);
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

            var chlg = new Challenge
            {
                Id = 1,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Challenges.Add(chlg);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAsync(1);
            Assert.NotNull(result);
            Assert.Equal("user2", result.ContenderId);
            Assert.Equal(1, _context.Challenges.Count());

            result = await _cut.GetAsync(2);
            Assert.Null(result);
            Assert.Equal(1, _context.Challenges.Count());

            //Reset
            _context.Remove(chlg);
            _context.Remove(usr1);
            _context.Remove(usr2);
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

            var chlg = new Challenge
            {
                Id = 1,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Challenges.Add(chlg);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetFirstOrDefaultAsync(x => x.Id == 1, includeProperties: "Challenger,Contender");
            Assert.NotNull(result);
            Assert.Equal("user1", result.Challenger.Id);
            Assert.Equal("user2", result.Contender.Id);
            Assert.Equal(1, _context.Challenges.Count());

            result = await _cut.GetFirstOrDefaultAsync(x => x.Id == 2, includeProperties: "Challenger,Contender");
            Assert.Null(result);
            Assert.Equal(1, _context.Challenges.Count());

            //Reset
            _context.Remove(chlg);
            _context.Remove(usr1);
            _context.Remove(usr2);
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
                ChallengeProgress = Challenge.ChallengeStatus.New,
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

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.Challenges.Add(chlg);
            _context.Challenges.Add(chlg2);
            _context.Challenges.Add(chlg3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAllAsync();
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Challenges.Count());

            result = await _cut.GetAllAsync(x => x.ChallengerId == "user1", includeProperties: "Challenger,Contender");
            Assert.Equal(2, result.Count());
            Assert.Equal("One", result.ElementAt(0).Challenger.LastName);
            Assert.Equal(3, _context.Challenges.Count());

            result = await _cut.GetAllAsync(orderBy: x => x.OrderBy(x => x.Id));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Challenges.Count());
            Assert.Equal(1, result.ElementAt(0).Id);

            result = await _cut.GetAllAsync(orderBy: x => x.OrderByDescending(x => x.Id));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Challenges.Count());
            Assert.Equal(3, result.ElementAt(0).Id);

            //Reset
            _context.Remove(chlg);
            _context.Remove(chlg2);
            _context.Remove(chlg3);
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

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var chlg = new Challenge
            {
                Id = 1,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            await _cut.AddAsync(chlg);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Challenges.FirstOrDefault(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Equal("user2", result.ContenderId);
            Assert.Equal(1, _context.Challenges.Count());

            //Reset
            _context.Remove(chlg);
            _context.Remove(usr1);
            _context.Remove(usr2);
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
                ChallengeProgress = Challenge.ChallengeStatus.New,
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

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.Challenges.Add(chlg);
            _context.Challenges.Add(chlg2);
            _context.Challenges.Add(chlg3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync(2);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Challenges.FirstOrDefault(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(2, _context.Challenges.Count());

            //Reset
            _context.Remove(chlg);
            _context.Remove(chlg3);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
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
                ChallengeProgress = Challenge.ChallengeStatus.New,
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

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.Challenges.Add(chlg);
            _context.Challenges.Add(chlg2);
            _context.Challenges.Add(chlg3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync(chlg2);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Challenges.FirstOrDefault(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(2, _context.Challenges.Count());

            //Reset
            _context.Remove(chlg);
            _context.Remove(chlg3);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
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
                ChallengeProgress = Challenge.ChallengeStatus.New,
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

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.Challenges.Add(chlg);
            _context.Challenges.Add(chlg2);
            _context.Challenges.Add(chlg3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            List<Challenge> list = new List<Challenge>() { chlg, chlg2 };
            await _cut.RemoveRangeAsync(list);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Challenges.FirstOrDefault(x => x.Id == 1);
            Assert.Null(result);
            result = _context.Challenges.FirstOrDefault(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(1, _context.Challenges.Count());

            //Reset
            _context.Remove(chlg3);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
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

            var chlg = new Challenge
            {
                Id = 1,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Challenges.Add(chlg);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            Assert.True(_cut.ObjectExists(1));
            Assert.False(_cut.ObjectExists(2));

            //Reset
            _context.Remove(chlg);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion

        #region Methods Specific To Interface

        [Fact]
        public async void UpdateTest()
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

            var chlg = new Challenge
            {
                Id = 1,
                ChallengeProgress = Challenge.ChallengeStatus.New,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Challenges.Add(chlg);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            chlg.ChallengeProgress = Challenge.ChallengeStatus.Accepted;
            _cut.UpdateAsync(chlg);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Challenges.FirstOrDefault(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Equal(Challenge.ChallengeStatus.Accepted, result.ChallengeProgress);
            Assert.Equal(1, _context.Challenges.Count());

            //Reset
            _context.Remove(chlg);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetUserChallengesTest()
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
                ChallengeProgress = Challenge.ChallengeStatus.Canceled,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
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
                ChallengeProgress = Challenge.ChallengeStatus.CompletedByContender,
                Challenger = usr1,
                Contender = usr2,
                CreateDate = DateTime.Now
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.Challenges.Add(chlg);
            _context.Challenges.Add(chlg2);
            _context.Challenges.Add(chlg3);
            _context.Challenges.Add(chlg4);
            _context.Challenges.Add(chlg5);
            _context.Challenges.Add(chlg6);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var request = _cut.GetUserChallenges("", "", "user1", false);
            Assert.Equal(3, request.Count());

            request = _cut.GetUserChallenges("", "", "user1", true);
            Assert.Equal(2, request.Count());

            //  add search term
            request = _cut.GetUserChallenges("", "User", "user1", false);
            Assert.Equal(3, request.Count());

            request = _cut.GetUserChallenges("", "Three", "user1", false);
            Assert.Equal(1, request.Count());


            //Reset
            _context.Remove(chlg);
            _context.Remove(chlg2);
            _context.Remove(chlg3);
            _context.Remove(chlg4);
            _context.Remove(chlg5);
            _context.Remove(chlg6);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion
    }
}
