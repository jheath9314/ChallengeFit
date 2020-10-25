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
    public class NewsFeedRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly INewsFeedRepository _cut;

        public NewsFeedRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryNewsFeed");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new NewsFeedRepository(_context);
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

            var news = new NewsFeed
            {
                Id = 1,
                CreateDate = DateTime.Now,
                Description = "News",
                FeedType = NewsFeed.FeedTypes.PublishedWorkout,
                User = usr1,
                RelatedUser = usr2
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.NewsFeed.Add(news);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAsync(1);
            Assert.NotNull(result);
            Assert.Equal("user1", result.UserId);
            Assert.Equal(1, _context.NewsFeed.Count());

            result = await _cut.GetAsync(2);
            Assert.Null(result);
            Assert.Equal(1, _context.NewsFeed.Count());

            //Reset
            _context.Remove(news);
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

            var news = new NewsFeed
            {
                Id = 1,
                CreateDate = DateTime.Now,
                Description = "News",
                FeedType = NewsFeed.FeedTypes.PublishedWorkout,
                User = usr1,
                RelatedUser = usr2
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.NewsFeed.Add(news);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetFirstOrDefaultAsync(x => x.Id == 1, includeProperties: "User,RelatedUser");
            Assert.NotNull(result);
            Assert.Equal("user1", result.User.Id);
            Assert.Equal("user2", result.RelatedUser.Id);
            Assert.Equal(1, _context.NewsFeed.Count());

            result = await _cut.GetFirstOrDefaultAsync(x => x.Id == 2, includeProperties: "User,RelatedUser");
            Assert.Null(result);
            Assert.Equal(1, _context.NewsFeed.Count());

            //Reset
            _context.Remove(news);
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

            var news = new NewsFeed
            {
                Id = 1,
                CreateDate = DateTime.Now,
                Description = "News",
                FeedType = NewsFeed.FeedTypes.PublishedWorkout,
                User = usr1,
                RelatedUser = usr2
            };

            var news2 = new NewsFeed
            {
                Id = 2,
                CreateDate = DateTime.Now,
                Description = "News2",
                FeedType = NewsFeed.FeedTypes.CompletedWorkout,
                User = usr2,
                RelatedUser = usr3
            };

            var news3 = new NewsFeed
            {
                Id = 3,
                CreateDate = DateTime.Now,
                Description = "News3",
                FeedType = NewsFeed.FeedTypes.AcceptedChallenge,
                User = usr3,
                RelatedUser = usr1
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.NewsFeed.Add(news);
            _context.NewsFeed.Add(news2);
            _context.NewsFeed.Add(news3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAllAsync();
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.NewsFeed.Count());

            result = await _cut.GetAllAsync(x => x.FeedType == NewsFeed.FeedTypes.AcceptedChallenge, includeProperties: "User,RelatedUser");
            Assert.Single(result);
            Assert.Equal("Three", result.ElementAt(0).User.LastName);
            Assert.Equal(3, _context.NewsFeed.Count());

            result = await _cut.GetAllAsync(orderBy: x => x.OrderBy(x => x.Id));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.NewsFeed.Count());
            Assert.Equal(1, result.ElementAt(0).Id);

            result = await _cut.GetAllAsync(orderBy: x => x.OrderByDescending(x => x.Id));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.NewsFeed.Count());
            Assert.Equal(3, result.ElementAt(0).Id);

            //Reset
            _context.Remove(news);
            _context.Remove(news2);
            _context.Remove(news3);
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
            var news = new NewsFeed
            {
                Id = 1,
                CreateDate = DateTime.Now,
                Description = "News",
                FeedType = NewsFeed.FeedTypes.PublishedWorkout,
                User = usr1,
                RelatedUser = usr2
            };

            await _cut.AddAsync(news);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.NewsFeed.FirstOrDefault(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Equal("user2", result.RelatedUserId);
            Assert.Equal(1, _context.NewsFeed.Count());

            //Reset
            _context.Remove(news);
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

            var news = new NewsFeed
            {
                Id = 1,
                CreateDate = DateTime.Now,
                Description = "News",
                FeedType = NewsFeed.FeedTypes.PublishedWorkout,
                User = usr1,
                RelatedUser = usr2
            };

            var news2 = new NewsFeed
            {
                Id = 2,
                CreateDate = DateTime.Now,
                Description = "News2",
                FeedType = NewsFeed.FeedTypes.CompletedWorkout,
                User = usr2,
                RelatedUser = usr3
            };

            var news3 = new NewsFeed
            {
                Id = 3,
                CreateDate = DateTime.Now,
                Description = "News3",
                FeedType = NewsFeed.FeedTypes.AcceptedChallenge,
                User = usr3,
                RelatedUser = usr1
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.NewsFeed.Add(news);
            _context.NewsFeed.Add(news2);
            _context.NewsFeed.Add(news3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync(2);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.NewsFeed.FirstOrDefault(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(2, _context.NewsFeed.Count());

            //Reset
            _context.Remove(news);
            _context.Remove(news3);
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

            var news = new NewsFeed
            {
                Id = 1,
                CreateDate = DateTime.Now,
                Description = "News",
                FeedType = NewsFeed.FeedTypes.PublishedWorkout,
                User = usr1,
                RelatedUser = usr2
            };

            var news2 = new NewsFeed
            {
                Id = 2,
                CreateDate = DateTime.Now,
                Description = "News2",
                FeedType = NewsFeed.FeedTypes.CompletedWorkout,
                User = usr2,
                RelatedUser = usr3
            };

            var news3 = new NewsFeed
            {
                Id = 3,
                CreateDate = DateTime.Now,
                Description = "News3",
                FeedType = NewsFeed.FeedTypes.AcceptedChallenge,
                User = usr3,
                RelatedUser = usr1
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.NewsFeed.Add(news);
            _context.NewsFeed.Add(news2);
            _context.NewsFeed.Add(news3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync(news2);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.NewsFeed.FirstOrDefault(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(2, _context.NewsFeed.Count());

            //Reset
            _context.Remove(news);
            _context.Remove(news3);
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

            var news = new NewsFeed
            {
                Id = 1,
                CreateDate = DateTime.Now,
                Description = "News",
                FeedType = NewsFeed.FeedTypes.PublishedWorkout,
                User = usr1,
                RelatedUser = usr2
            };

            var news2 = new NewsFeed
            {
                Id = 2,
                CreateDate = DateTime.Now,
                Description = "News2",
                FeedType = NewsFeed.FeedTypes.CompletedWorkout,
                User = usr2,
                RelatedUser = usr3
            };

            var news3 = new NewsFeed
            {
                Id = 3,
                CreateDate = DateTime.Now,
                Description = "News3",
                FeedType = NewsFeed.FeedTypes.AcceptedChallenge,
                User = usr3,
                RelatedUser = usr1
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.NewsFeed.Add(news);
            _context.NewsFeed.Add(news2);
            _context.NewsFeed.Add(news3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            List<NewsFeed> list = new List<NewsFeed>() { news, news2 };
            await _cut.RemoveRangeAsync(list);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.NewsFeed.FirstOrDefault(x => x.Id == 1);
            Assert.Null(result);
            result = _context.NewsFeed.FirstOrDefault(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(1, _context.NewsFeed.Count());

            //Reset
            _context.Remove(news3);
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

            var news = new NewsFeed
            {
                Id = 1,
                CreateDate = DateTime.Now,
                Description = "News",
                FeedType = NewsFeed.FeedTypes.PublishedWorkout,
                User = usr1,
                RelatedUser = usr2
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.NewsFeed.Add(news);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            Assert.True(_cut.ObjectExists(1));
            Assert.False(_cut.ObjectExists(2));

            //Reset
            _context.Remove(news);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion

        #region Methods Specific To Interface

        [Fact]
        public async void GetUserFeedTest()
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

            var news = new NewsFeed
            {
                Id = 1,
                CreateDate = DateTime.Now,
                Description = "News",
                FeedType = NewsFeed.FeedTypes.PublishedWorkout,
                User = usr1
            };

            var news2 = new NewsFeed
            {
                Id = 2,
                CreateDate = DateTime.Now,
                Description = "News2",
                FeedType = NewsFeed.FeedTypes.CompletedWorkout,
                User = usr2
            };

            var news3 = new NewsFeed
            {
                Id = 3,
                CreateDate = DateTime.Now,
                Description = "News3",
                FeedType = NewsFeed.FeedTypes.AcceptedChallenge,
                User = usr1,
                RelatedUser = usr3
            };

            var fr1 = new FriendRequest
            {
                RequestedById = "user1",
                RequestedForId = "user2",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            var fr2 = new FriendRequest
            {
                RequestedById = "user1",
                RequestedForId = "user3",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.FriendRequests.Add(fr1);
            _context.FriendRequests.Add(fr2);
            _context.NewsFeed.Add(news);
            _context.NewsFeed.Add(news2);
            _context.NewsFeed.Add(news3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = _cut.GetUserFeed("user3");
            Assert.Equal(2, result.Count());
            Assert.Equal(3, _context.NewsFeed.Count());

            //Reset
            _context.Remove(news);
            _context.Remove(news2);
            _context.Remove(news3);
            _context.Remove(fr1);
            _context.Remove(fr2);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion
    }
}
