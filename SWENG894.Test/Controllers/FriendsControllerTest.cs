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
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace SWENG894.Test.Controllers
{
    public class FriendsControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly FriendsController _cut;

        public FriendsControllerTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryFriendsController");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _unitOfWork = new UnitOfWork(_context);
            _cut = new FriendsController(_unitOfWork);
        }

        [Fact]
        public async void FriendsControllerOpTest()
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

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.SaveChangesAsync().GetAwaiter();

            var unit = new UnitOfWork(_context);
            var cont = new FriendsController(unit);

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
            }, "mock"));

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            FriendRequest fr = new FriendRequest()
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.New,
            };

            var res = await cont.SendRequestPost("guid-user2");

            var data = await _context.FriendRequests.FirstOrDefaultAsync();

            Assert.True(data != null);

            var res2 = await cont.Details("guid-user1");

            data = await _context.FriendRequests.FirstOrDefaultAsync(x => x.RequestedById == "guid-user1");

            Assert.True(data != null);

            var res3 = await cont.ViewRequest("guid-user1", "guid-user2");

            data = await _context.FriendRequests.FirstOrDefaultAsync(x => x.RequestedById == "guid-user1" && x.RequestedForId == "guid-user2");

            Assert.True(data != null);

            var res4 = await cont.Profile("guid-user2");

            data = await _context.FriendRequests.FirstOrDefaultAsync(x =>  x.RequestedForId == "guid-user2");

            _context.Database.EnsureDeleted();
        }

        //[Fact]
        //public void IndexTest()
        //{
        //    //Setup
        //    var usr1 = new ApplicationUser
        //    {
        //        Id = "guid-user1",
        //        UserName = "user1@psu.edu",
        //        Email = "user1@psu.edu",
        //        EmailConfirmed = true,
        //        FirstName = "User",
        //        LastName = "One",
        //        ZipCode = "11111"
        //    };

        //    var usr2 = new ApplicationUser
        //    {
        //        Id = "guid-user2",
        //        UserName = "user2@psu.edu",
        //        Email = "user2@psu.edu",
        //        EmailConfirmed = true,
        //        FirstName = "User",
        //        LastName = "Two",
        //        ZipCode = "22222"
        //    };

        //    var usr3 = new ApplicationUser
        //    {
        //        Id = "guid-user3",
        //        UserName = "user3@psu.edu",
        //        Email = "user3@psu.edu",
        //        EmailConfirmed = true,
        //        FirstName = "User",
        //        LastName = "Three",
        //        ZipCode = "22222"
        //    };

        //    var usr4 = new ApplicationUser
        //    {
        //        Id = "guid-user4",
        //        UserName = "user4@psu.edu",
        //        Email = "user4@psu.edu",
        //        EmailConfirmed = true,
        //        FirstName = "User",
        //        LastName = "Four",
        //        ZipCode = "44444"
        //    };

        //    var fr1 = new FriendRequest
        //    {
        //        RequestedById = "guid-user1",
        //        RequestedForId = "guid-user2",
        //        Status = FriendRequest.FriendRequestStatus.New
        //    };

        //    var fr2 = new FriendRequest
        //    {
        //        RequestedById = "guid-user2",
        //        RequestedForId = "guid-user3",
        //        Status = FriendRequest.FriendRequestStatus.Rejected
        //    };

        //    var fr3 = new FriendRequest
        //    {
        //        RequestedById = "guid-user3",
        //        RequestedForId = "guid-user1",
        //        Status = FriendRequest.FriendRequestStatus.Approved
        //    };

        //    var fr4 = new FriendRequest
        //    {
        //        RequestedById = "guid-user1",
        //        RequestedForId = "guid-user4",
        //        Status = FriendRequest.FriendRequestStatus.Approved
        //    };

        //    _context.ApplicationUsers.Add(usr1);
        //    _context.ApplicationUsers.Add(usr2);
        //    _context.ApplicationUsers.Add(usr3);
        //    _context.ApplicationUsers.Add(usr4);
        //    _context.FriendRequests.Add(fr1);
        //    _context.FriendRequests.Add(fr2);
        //    _context.FriendRequests.Add(fr3);
        //    _context.FriendRequests.Add(fr4);
        //    _context.SaveChangesAsync().GetAwaiter();

        //    var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Name, "user1@psu.edu"),
        //        new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
        //    }, "mock"));

        //    _cut.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext() { User = loggedInUser }
        //    };

        //    //Test
        //    var index = _cut.Index("", "", "", 1);
        //}
    }
}
