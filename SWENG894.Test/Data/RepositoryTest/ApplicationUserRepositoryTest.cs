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

        #endregion
    }
}
