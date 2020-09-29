using Microsoft.EntityFrameworkCore;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
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
            builder.UseInMemoryDatabase(databaseName: "DbInMemory");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new FriendRequestRepository(_context);
        }

        #region Methods Inherited From Repository

        [Fact]
        public async void GetAsyncTest()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void GetFirstOrDefaultAsyncTest()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void GetAllAsyncTest()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void AddAsyncTests()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void RemoveAsyncIdTest()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void RemoveAsyncObjTest()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void RemoverangeAsyncTest()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void ObjectExistsTest()
        {
            //Setup

            //Test

            //Reset
        }

        #endregion

        #region Methods Specific To Interface

        [Fact]
        public async void UpdateAsyncTest()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void GetAllUserFriendRequestsTest()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void FindNewFriendsTest()
        {
            //Setup

            //Test

            //Reset
        }

        [Fact]
        public async void GetPersonToFriendTest()
        {
            //Setup

            //Test

            //Reset
        }

        #endregion
    }
}
