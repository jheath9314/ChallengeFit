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
            builder.UseInMemoryDatabase(databaseName: "DbInMemory");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new MessageRepository(_context);
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
            var result = await _cut.GetFirstOrDefaultAsync(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Equal("From user1 to user2 msg1", result.Subject);
            Assert.Equal(1, _context.Messages.Count());

            result = await _cut.GetFirstOrDefaultAsync(x => x.Id == 2);
            Assert.Null(result);
            Assert.Equal(1, _context.Messages.Count());

            //Reset
            _context.Remove(msg1);
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void GetAllAsyncTest()
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
            var result = await _cut.GetAllAsync();
            Assert.Equal(3, result.Count());
            Assert.Equal(3, _context.Messages.Count());

            result = await _cut.GetAllAsync(x => x.SentById == "user1");
            Assert.Equal(2, result.Count());
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
                SentById = "user1",
                SentToId = "user2",
                Subject = "From user1 to user2 msg3",
                Body = "From user1 to user2 msg3",
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            var msg4 = new Message
            {
                Id = 4,
                SentById = "user1",
                SentToId = "user3",
                Subject = "From user1 to user3 msg1",
                Body = "From user1 to user3 msg1",
                SentTime = DateTime.Now,
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
                SentTime = DateTime.Now,
                SendStatus = Message.MessageSendStatud.New,
                ReadStatus = Message.MessageReadStatud.New,
                MessageType = Message.MessageTypes.Correspondence,
                DeletedBySender = false,
                DeletedByReceiver = false
            };

            //Test

            //Reset
        }

        [Fact]
        public void CreateNewMesage()
        {
            //Setup

            //Test

            //Reset
        }

        #endregion
    }
}