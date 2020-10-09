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
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using SWENG894.ViewModels;

namespace SWENG894.Test.Controllers
{
    public class MessagesControllerTest
    {
        private readonly ApplicationDbContext _context;
        public MessagesControllerTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryMessagesController");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
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

            var fr = new FriendRequest()
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user2",
                RequestStatus = FriendRequest.FriendRequestStatus.Approved,
            };

            var msg = new Message()
            {
                Id = 1,
                SentById = "guid-user1",
                SentToId = "guid-user2",
                SentTime = DateTime.Now,
                Subject = "Test",
                Body = "Test body",
                SendStatus = Message.MessageSendStatud.New,
                DeletedByReceiver = false,
                DeletedBySender = false
                
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.FriendRequests.Add(fr);
            _context.Messages.Add(msg);
            _context.SaveChangesAsync().GetAwaiter();

            var unit = new UnitOfWork(_context);
            var cont = new MessagesController(unit);

            var loggedInUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@psu.edu"),
                new Claim(ClaimTypes.NameIdentifier, "guid-user1"),
            }, "mock"));

            cont.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = loggedInUser }
            };

            var res = (ViewResult)cont.Details(1, "sent").Result;
            Message message = (Message)res.Model;

            Assert.Equal("Test", message.Subject);

            var msg2 = new MessageViewModel()
            {
                SentById = "guid-user1",
                SentToId = "guid-user2",
                Subject = "Test2",
                Body = "Body2"
            };

            await cont.Create(msg2);

            var data = _context.Messages.Where(x => x.SentById == "guid-user1"); 

            Assert.Equal(2, data.Count());

            await cont.Delete(1);

            data = _context.Messages.Where(x => x.Id == 1);

            Assert.Single(data);

            _context.Database.EnsureDeleted();
        }
    }
}
