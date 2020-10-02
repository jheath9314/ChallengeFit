using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Org.BouncyCastle.Crypto.Digests;
using SWENG894.Areas.User.Controllers;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SWENG894.Test.RepositoryTest
{
    public class MessageRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IMessageRepository _cut;

        public MessageRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryMessages");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new MessageRepository(_context);
        }

        [Fact]
        public async void CreateNewMessageTest()
        {
            var user = new ApplicationUser();
            var user_2 = new ApplicationUser();

            _context.ApplicationUsers.Add(user);
            _context.ApplicationUsers.Add(user_2);
            _context.SaveChanges();

            var userList = await _context.ApplicationUsers.ToListAsync();

            user = userList[0];
            user_2 = userList[1];

            var messageModel = _cut.CreateNewMesage(user.Id);

            Assert.True(messageModel != null);

            var nullMessageModel = _cut.CreateNewMesage("INVALID");

            Assert.True(nullMessageModel == null);

            var request = new FriendRequest();
            request.RequestedById = user.Id;
            request.RequestedForId = user_2.Id;
            request.Status = FriendRequest.FriendRequestStatus.Approved;

            await _context.FriendRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            request.RequestedById = user_2.Id;
            request.RequestedForId = user.Id;
            request.Status = FriendRequest.FriendRequestStatus.Approved;

            await _context.FriendRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            messageModel = _cut.CreateNewMesage(user.Id);

            Assert.True(messageModel.Friends.Count > 0);

        }

        #region Methods Inherited From Repository

        [Fact]
        public async void GetAsyncTest()
        {
            //Setup
            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            _context.Messages.Add(msg1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAsync(1);
            Assert.NotNull(result);
            Assert.Equal("From user1 to user2 msg1", result.Subject);
            Assert.Equal(1, _context.Messages.Count());

            result = await _cut.GetAsync(2);
            Assert.Null(result);
            Assert.Equal(1, _context.Messages.Count());

            //Reset
            _context.Remove(msg1);
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

            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Messages.Add(msg1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetFirstOrDefaultAsync(x => x.Id == 1, includeProperties: "SentBy");
            Assert.NotNull(result);
            Assert.Equal("From user1 to user2 msg1", result.Subject);
            Assert.Equal("user1", result.SentBy.Id);
            Assert.Equal(1, _context.Messages.Count());

            result = await _cut.GetFirstOrDefaultAsync(x => x.Id == 2, includeProperties: "SentTo");
            Assert.Null(result);
            Assert.Equal(1, _context.Messages.Count());

            //Reset
            _context.Remove(msg1);
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

            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg2 = new Message
            {
                Id = 2,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg2",
                Body = "From user1 to user2 msg2",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg3 = new Message
            {
                Id = 3,
                SentById = "user2",
                SentToId = "user1",
                Subject = "From user2 to user1 msg1",
                Body = "From user2 to user1 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            _context.ApplicationUsers.Add(usr1);
            _context.ApplicationUsers.Add(usr2);
            _context.Messages.Add(msg1);
            _context.Messages.Add(msg2);
            _context.Messages.Add(msg3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var result = await _cut.GetAllAsync();
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Messages.Count());

            result = await _cut.GetAllAsync(x => x.SentById == "user1", includeProperties: "SentBy");
            Assert.Equal(2, result.Count());
            Assert.Equal("One", result.ElementAt(0).SentBy.LastName);
            Assert.Equal(3, _context.Messages.Count());

            result = await _cut.GetAllAsync(orderBy: x => x.OrderBy(x => x.Id));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Messages.Count());
            Assert.Equal(1, result.ElementAt(0).Id);

            result = await _cut.GetAllAsync(orderBy: x => x.OrderByDescending(x => x.Id));
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Messages.Count());
            Assert.Equal(3, result.ElementAt(0).Id);

            //Reset
            _context.Remove(msg1);
            _context.Remove(msg2);
            _context.Remove(msg3);
            _context.Remove(usr1);
            _context.Remove(usr2);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void AddAsyncTests()
        {
            //Setup
            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            //Test
            await _cut.AddAsync(msg1);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Messages.FirstOrDefault(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Equal("From user1 to user2 msg1", result.Subject);
            Assert.Equal(1, _context.Messages.Count());

            //Reset
            _context.Remove(msg1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoveAsyncIdTest()
        {
            //Setup
            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg2 = new Message
            {
                Id = 2,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg2",
                Body = "From user1 to user2 msg2",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg3 = new Message
            {
                Id = 3,
                SentById = "user2",
                SentToId = "user1",
                Subject = "From user2 to user1 msg1",
                Body = "From user2 to user1 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };
            _context.Messages.Add(msg1);
            _context.Messages.Add(msg2);
            _context.Messages.Add(msg3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync(2);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Messages.FirstOrDefault(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(2, _context.Messages.Count());

            //Reset
            _context.Remove(msg1);
            _context.Remove(msg3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoveAsyncObjTest()
        {
            //Setup
            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg2 = new Message
            {
                Id = 2,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg2",
                Body = "From user1 to user2 msg2",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg3 = new Message
            {
                Id = 3,
                SentById = "user2",
                SentToId = "user1",
                Subject = "From user2 to user1 msg1",
                Body = "From user2 to user1 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };
            _context.Messages.Add(msg1);
            _context.Messages.Add(msg2);
            _context.Messages.Add(msg3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            await _cut.RemoveAsync(msg2);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Messages.FirstOrDefault(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(2, _context.Messages.Count());

            //Reset
            _context.Remove(msg1);
            _context.Remove(msg3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void RemoverangeAsyncTest()
        {
            //Setup
            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg2 = new Message
            {
                Id = 2,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg2",
                Body = "From user1 to user2 msg2",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg3 = new Message
            {
                Id = 3,
                SentById = "user2",
                SentToId = "user1",
                Subject = "From user2 to user1 msg1",
                Body = "From user2 to user1 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };
            _context.Messages.Add(msg1);
            _context.Messages.Add(msg2);
            _context.Messages.Add(msg3);
            _context.SaveChangesAsync().GetAwaiter();

            //Test

            List<Message> list = new List<Message>() { msg1, msg2 };
            await _cut.RemoveRangeAsync(list);
            _context.SaveChangesAsync().GetAwaiter();

            var result = _context.Messages.FirstOrDefault(x => x.Id == 1);
            Assert.Null(result);
            result = _context.Messages.FirstOrDefault(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(1, _context.Messages.Count());

            //Reset
            _context.Remove(msg3);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void ObjectExistsTest()
        {
            //Setup
            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            _context.Messages.Add(msg1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            Assert.True(_cut.ObjectExists(1));
            Assert.False(_cut.ObjectExists(2));

            //Reset
            _context.Remove(msg1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        #endregion

        #region Methods Specific To Interface

        [Fact]
        public async void UpdateAsyncTest()
        {
            //Setup
            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            _context.Messages.Add(msg1);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            //Update not impelemented.
            Assert.Throws<NotImplementedException>(() => _cut.UpdateAsync(msg1));
            Assert.Equal(1, _context.Messages.Count());

            //Reset
            _context.Remove(msg1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public void GetAllUserMessages()
        {
            //Setup
            var msg1 = new Message
            {
                Id = 1,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg1",
                Body = "From user1 to user2 msg1",
                SentTime = new DateTime(2020, 10, 1, 12, 0, 0),
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = true,
                DeletedByReceiver = false
            };

            var msg2 = new Message
            {
                Id = 2,
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg2",
                Body = "From user1 to user2 msg2",
                SentTime = new DateTime(2020, 10, 2, 12, 0, 0),
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg3 = new Message
            {
                Id = 3,
                SentById = "user4",
                SentToId = "user5",
                Subject = "From user4 to user5 msg1",
                Body = "From user4 to user5 msg1",
                SentTime = new DateTime(2020, 10, 3, 12, 0, 0),
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg4 = new Message
            {
                Id = 4,
                SentById = "user2",
                SentToId = "user5",
                Subject = "From user2 to user5 msg1",
                Body = "From user2 to user5 msg1",
                SentTime = new DateTime(2020, 10, 4, 12, 0, 0),
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg5 = new Message
            {
                Id = 5,
                SentById = "user2",
                SentToId = "user1",
                Subject = "From user2 to user1 msg1",
                Body = "From user2 to user1 msg1",
                SentTime = new DateTime(2020, 10, 5, 12, 0, 0),
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg6 = new Message
            {
                Id = 6,
                SentById = "user2",
                SentToId = "user1",
                Subject = "From user2 to user1 msg2",
                Body = "From user2 to user1 msg2",
                SentTime = new DateTime(2020, 10, 6, 12, 0, 0),
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = true
            };

            var msg7 = new Message
            {
                Id = 7,
                SentById = "user1",
                SentToId = "user6",
                Subject = "From user1 to user6 msg1",
                Body = "From user1 to user6 msg1",
                SentTime = new DateTime(2020, 10, 7, 12, 0, 0),
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg8 = new Message
            {
                Id = 8,
                SentById = "user7",
                SentToId = "user1",
                Subject = "From user7 to user1 msg1",
                Body = "From user7 to user1 msg1",
                SentTime = new DateTime(2020, 10, 8, 12, 0, 0),
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            _context.Messages.Add(msg1);
            _context.Messages.Add(msg2);
            _context.Messages.Add(msg3);
            _context.Messages.Add(msg4);
            _context.Messages.Add(msg5);
            _context.Messages.Add(msg6);
            _context.Messages.Add(msg7);
            _context.Messages.Add(msg8);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var userMessages = _cut.GetAllUserMessages("", "", "", "user1");
            //Only returns messages 5 and 8.
            Assert.Equal(2, userMessages.Count());
            Assert.Equal(5, userMessages.ElementAt(0).SentTime.Day);

            userMessages = _cut.GetAllUserMessages("", "From user7 to user1 msg1", "", "user1");
            Assert.Single(userMessages);
            Assert.Equal("From user7 to user1 msg1", userMessages.ElementAt(0).Subject);

            userMessages = _cut.GetAllUserMessages("desc", "", "", "user1");
            Assert.Equal(2, userMessages.Count());
            Assert.Equal(8, userMessages.ElementAt(0).SentTime.Day);

            //Only returns messages 2 and 7.
            userMessages = _cut.GetAllUserMessages("", "", "sent", "user1");
            Assert.Equal(2, userMessages.Count());
            Assert.Equal(2, userMessages.ElementAt(0).SentTime.Day);

            userMessages = _cut.GetAllUserMessages("", "From user1 to user6 msg1", "sent", "user1");
            Assert.Single(userMessages);
            Assert.Equal("From user1 to user6 msg1", userMessages.ElementAt(0).Subject);

            userMessages = _cut.GetAllUserMessages("desc", "", "sent", "user1");
            Assert.Equal(2, userMessages.Count());
            Assert.Equal(7, userMessages.ElementAt(0).SentTime.Day);

            //Reset
            _context.Remove(msg1);
            _context.Remove(msg2);
            _context.Remove(msg3);
            _context.Remove(msg4);
            _context.Remove(msg5);
            _context.Remove(msg6);
            _context.Remove(msg7);
            _context.Remove(msg8);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public void CreateNewMesage()
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
                Status = FriendRequest.FriendRequestStatus.New
            };

            var fr2 = new FriendRequest
            {
                RequestedById = "guid-user2",
                RequestedForId = "guid-user3",
                Status = FriendRequest.FriendRequestStatus.Rejected
            };

            var fr3 = new FriendRequest
            {
                RequestedById = "guid-user4",
                RequestedForId = "guid-user3",
                Status = FriendRequest.FriendRequestStatus.Approved
            };

            var fr4 = new FriendRequest
            {
                RequestedById = "guid-user1",
                RequestedForId = "guid-user4",
                Status = FriendRequest.FriendRequestStatus.Approved
            };

            _context.Users.Add(usr1);
            _context.Users.Add(usr2);
            _context.Users.Add(usr3);
            _context.Users.Add(usr4);
            _context.FriendRequests.Add(fr1);
            _context.FriendRequests.Add(fr2);
            _context.FriendRequests.Add(fr3);
            _context.FriendRequests.Add(fr4);
            _context.SaveChangesAsync().GetAwaiter();

            //Test
            var viewModel = _cut.CreateNewMesage("guid-user5");
            Assert.Null(viewModel);
            viewModel = _cut.CreateNewMesage("guid-user4");
            Assert.NotNull(viewModel);

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