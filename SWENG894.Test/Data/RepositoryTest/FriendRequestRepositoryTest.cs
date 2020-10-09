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

namespace SWENG894.Test.RepositoryTest
{
    public class FriendRequestRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IFriendRequestRepository _cut;

        public FriendRequestRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryFriendReq");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new FriendRequestRepository(_context);
        }

        #region Methods Inherited From Repository

        [Fact]
        public async System.Threading.Tasks.Task GetAsyncTest()
        {
            //Setup
            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            _context.FriendRequests.Add(fr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            //GetAsync not impelemented.
            await Assert.ThrowsAsync<NotImplementedException>(() => _cut.GetAsync(1));
            Assert.Equal(1, _context.FriendRequests.Count());

            //Reset
            _context.Remove(fr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Reset
            _context.Remove(fr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetFirstOrDefaultAsyncTest()
        {
            //Setup
            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            _context.FriendRequests.Add(fr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetFirstOrDefaultAsync(x => x.RequestedById == "guid-user1" && x.RequestedForId == "guid-user2");
            Assert.NotNull(result);
            Assert.Equal("guid-user1", result.RequestedById);
            Assert.Equal("guid-user2", result.RequestedForId);
            Assert.Equal(1, _context.FriendRequests.Count());

            result = await _cut.GetFirstOrDefaultAsync(x => x.RequestedById == "guid-user3" && x.RequestedForId == "guid-user2");
            Assert.Null(result);
            Assert.Equal(1, _context.FriendRequests.Count());

            //Reset
            _context.Remove(fr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetAllAsyncTest()
        {
            //Setup
            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            var fr2 = new FriendRequest
            {
                RequestedById = "guid-user2",
                RequestedForId = "guid-user3",
                RequestStatus = FriendRequest.FriendRequestStatus.Rejected
            };

            var fr3 = new FriendRequest
            {
                RequestedById = "guid-user3",
                RequestedForId = "guid-user4",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            var fr4 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user4",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            _context.FriendRequests.Add(fr1);
            _context.FriendRequests.Add(fr2);
            _context.FriendRequests.Add(fr3);
            _context.FriendRequests.Add(fr4);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAllAsync();
            Assert.Equal(4, result.Count());
            Assert.Equal(4, _context.FriendRequests.Count());

            result = await _cut.GetAllAsync(x => x.RequestedById == "guid-user1");
            Assert.Equal(2, result.Count());
            Assert.Equal(4, _context.FriendRequests.Count());

            result = await _cut.GetAllAsync(orderBy: x => x.OrderBy(x => x.RequestedById));
            Assert.Equal(4, result.Count());
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal("guid-user1", result.ElementAt(0).RequestedById);

            result = await _cut.GetAllAsync(orderBy: x => x.OrderByDescending(x => x.RequestedById));
            Assert.Equal(4, result.Count());
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal("guid-user3", result.ElementAt(0).RequestedById);

            //Reset
            _context.Remove(fr1);
            _context.Remove(fr2);
            _context.Remove(fr3);
            _context.Remove(fr4);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void AddAsyncTests()
        {
            //Setup
            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            //Test
            await _cut.AddAsync(fr1);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.FriendRequests.FirstOrDefault(x => x.RequestedById == "guid-user1");
            Assert.NotNull(result);
            Assert.Equal(FriendRequest.FriendRequestStatus.New, result.RequestStatus);
            Assert.Equal(1, _context.FriendRequests.Count());

            //Reset
            _context.Remove(fr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoveAsyncIdTest()
        {
            //Setup
            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            _context.FriendRequests.Add(fr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            //GetAsync not impelemented.
            await Assert.ThrowsAsync<NotImplementedException>(() => _cut.RemoveAsync(1));
            Assert.Equal(1, _context.FriendRequests.Count());

            //Reset
            _context.Remove(fr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Reset
            _context.Remove(fr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoveAsyncObjTest()
        {
            //Setup
            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            var fr2 = new FriendRequest
            {
                RequestedById = "guid-user2",
                RequestedForId = "guid-user3",
                RequestStatus = FriendRequest.FriendRequestStatus.Rejected
            };

            var fr3 = new FriendRequest
            {
                RequestedById = "guid-user3",
                RequestedForId = "guid-user4",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            _context.FriendRequests.Add(fr1);
            _context.FriendRequests.Add(fr2);
            _context.FriendRequests.Add(fr3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync(fr2);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.FriendRequests.FirstOrDefault(x => x.RequestedById == "guid-user2" && x.RequestedForId == "guid-user3");
            Assert.Null(result);
            Assert.Equal(2, _context.FriendRequests.Count());

            //Reset
            _context.Remove(fr1);
            _context.Remove(fr3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoverangeAsyncTest()
        {
            //Setup
            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            var fr2 = new FriendRequest
            {
                RequestedById = "guid-user2",
                RequestedForId = "guid-user3",
                RequestStatus = FriendRequest.FriendRequestStatus.Rejected
            };

            var fr3 = new FriendRequest
            {
                RequestedById = "guid-user3",
                RequestedForId = "guid-user4",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            _context.FriendRequests.Add(fr1);
            _context.FriendRequests.Add(fr2);
            _context.FriendRequests.Add(fr3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            List<FriendRequest> list = new List<FriendRequest>() { fr1, fr2 };
            await _cut.RemoveRangeAsync(list);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.FriendRequests.FirstOrDefault(x => x.RequestedById == "guid-user1" && x.RequestedForId == "guid-user2");
            Assert.Null(result);
            result = _context.FriendRequests.FirstOrDefault(x => x.RequestedById == "guid-user2" && x.RequestedForId == "guid-user3");
            Assert.Null(result);
            Assert.Equal(1, _context.FriendRequests.Count());

            //Reset
            _context.Remove(fr3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void ObjectExistsTest()
        {
            //Setup
            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            _context.FriendRequests.Add(fr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            //GetAsync not impelemented.
             Assert.Throws<NotImplementedException>(() => _cut.ObjectExists(1));
            Assert.Equal(1, _context.FriendRequests.Count());

            //Reset
            _context.Remove(fr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion

        #region Methods Specific To Interface

        [Fact]
        public async void UpdateAsyncTest()
        {
            //Setup
            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            _context.FriendRequests.Add(fr1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            //GetAsync not impelemented.
            Assert.Throws<NotImplementedException>(() => _cut.UpdateAsync(fr1));
            Assert.Equal(1, _context.FriendRequests.Count());

            //Reset
            _context.Remove(fr1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetAllUserFriendRequestsTest()
        {
            //Setup
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

            var usr3 = new ApplicationUser
            {
                Id = "guid-user3",
                UserName = "user3@psu.edu",
                Email = "user3@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Three",
                ZipCode = "22222"
            };

            var usr4 = new ApplicationUser
            {
                Id = "guid-user4",
                UserName = "user4@psu.edu",
                Email = "user4@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Four",
                ZipCode = "44444"
            };

            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            var fr2 = new FriendRequest
            {
                RequestedById = "guid-user2",
                RequestedForId = "guid-user3",
                RequestStatus = FriendRequest.FriendRequestStatus.Rejected
            };

            var fr3 = new FriendRequest
            {
                RequestedById = "guid-user3",
                RequestedForId = "guid-user1",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            var fr4 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user4",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.ApplicationUsers.Add(usr4);
            _context.FriendRequests.Add(fr1);
            _context.FriendRequests.Add(fr2);
            _context.FriendRequests.Add(fr3);
            _context.FriendRequests.Add(fr4);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = _cut.GetUserFriends("", "", "guid-user1", false);
            //Returns only 2 approved requests fr3 and fr4. fr1 is not returned as it's still New.
            Assert.Equal(2, result.Count());
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal(4, _context.Users.Count());
            Assert.Equal("Four", result.ElementAt(0).LastName);

            result = _cut.GetUserFriends("desc", "", "guid-user1", false);
            //Returns only 2 approved requests fr3 and fr4. fr1 is not returned as it's still New.
            Assert.Equal(2, result.Count());
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal(4, _context.Users.Count());
            Assert.Equal("Three", result.ElementAt(0).LastName);

            result = _cut.GetUserFriends("", "Three", "guid-user1", false);
            Assert.Single(result);
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal(4, _context.Users.Count());
            Assert.Equal("Three", result.ElementAt(0).LastName);

            //Reset
            _context.Remove(fr1);
            _context.Remove(fr2);
            _context.Remove(fr3);
            _context.Remove(fr4);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
            _context.Remove(usr4);           
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void FindNewFriendsTest()
        {
            //Setup
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

            var usr3 = new ApplicationUser
            {
                Id = "guid-user3",
                UserName = "user3@psu.edu",
                Email = "user3@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Three",
                ZipCode = "22222"
            };

            var usr4 = new ApplicationUser
            {
                Id = "guid-user4",
                UserName = "user4@psu.edu",
                Email = "user4@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Four",
                ZipCode = "44444"
            };

            var usr5 = new ApplicationUser
            {
                Id = "guid-user5",
                UserName = "user5@psu.edu",
                Email = "user5@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Five",
                ZipCode = "55555"
            };

            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            var fr2 = new FriendRequest
            {
                RequestedById = "guid-user2",
                RequestedForId = "guid-user3",
                RequestStatus = FriendRequest.FriendRequestStatus.Rejected
            };

            var fr3 = new FriendRequest
            {
                RequestedById = "guid-user3",
                RequestedForId = "guid-user1",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            var fr4 = new FriendRequest
            {
                RequestedById = "guid-user2",
                RequestedForId = "guid-user4",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.ApplicationUsers.Add(usr4);
            _context.ApplicationUsers.Add(usr5);
            _context.FriendRequests.Add(fr1);
            _context.FriendRequests.Add(fr2);
            _context.FriendRequests.Add(fr3);
            _context.FriendRequests.Add(fr4);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = _cut.FindNewFriends("", "", "guid-user1");
            //Should return usr4 and usr5
            Assert.Equal(2, result.Count());
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal(5, _context.Users.Count());
            Assert.Equal("Five", result.ElementAt(0).LastName);

            result = _cut.FindNewFriends("desc", "", "guid-user1");
            Assert.Equal(2, result.Count());
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal(5, _context.Users.Count());
            Assert.Equal("Four", result.ElementAt(0).LastName);

            result = _cut.FindNewFriends("", "Three", "guid-user1");
            Assert.Empty(result);
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal(5, _context.Users.Count());

            result = _cut.FindNewFriends("", "Five", "guid-user1");
            Assert.Single(result);
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal(5, _context.Users.Count());
            Assert.Equal("Five", result.ElementAt(0).LastName);

            //Reset
            _context.Remove(fr1);
            _context.Remove(fr2);
            _context.Remove(fr3);
            _context.Remove(fr4);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
            _context.Remove(usr4);
            _context.Remove(usr5);          
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetPersonToFriendTest()
        {
            //Setup
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

            var usr3 = new ApplicationUser
            {
                Id = "guid-user3",
                UserName = "user3@psu.edu",
                Email = "user3@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Three",
                ZipCode = "22222"
            };

            var usr4 = new ApplicationUser
            {
                Id = "guid-user4",
                UserName = "user4@psu.edu",
                Email = "user4@psu.edu",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "Four",
                ZipCode = "44444"
            };

            var fr1 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New
            };

            var fr2 = new FriendRequest
            {
                RequestedById = "guid-user2",
                RequestedForId = "guid-user3",
                RequestStatus = FriendRequest.FriendRequestStatus.Rejected
            };

            var fr3 = new FriendRequest
            {
                RequestedById = "guid-user3",
                RequestedForId = "guid-user1",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            var fr4 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user4",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.ApplicationUsers.Add(usr3);
            _context.ApplicationUsers.Add(usr4);
            _context.FriendRequests.Add(fr1);
            _context.FriendRequests.Add(fr2);
            _context.FriendRequests.Add(fr3);
            _context.FriendRequests.Add(fr4);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetPersonToFriend("guid-user1");
            Assert.Equal(usr1, result);
            Assert.Equal(4, _context.FriendRequests.Count());
            Assert.Equal(4, _context.Users.Count());

            //Reset
            _context.Remove(fr1);
            _context.Remove(fr2);
            _context.Remove(fr3);
            _context.Remove(fr4);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.Remove(usr3);
            _context.Remove(usr4);           
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion
    }
}
