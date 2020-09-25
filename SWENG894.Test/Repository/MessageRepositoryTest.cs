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

namespace SWENG894.Test.Repository
{
    public class MessageRepositoryTest
    {
        private readonly ApplicationDbContext _context;

        public MessageRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemory");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async void MessageTests()
        {
            var db = new MessageRepository(_context);
           
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

            await db.AddAsync(msg1);          
            _context.SaveChangesAsync().GetAwaiter();

            var result = db.GetAsync(1);
            Assert.NotNull(result.Result);
            Assert.Equal("From user1 to user2 msg1", result.Result.Subject);
            Assert.True(db.ObjectExists(1));

            var test = db.ObjectExists(5);
            var test2 = db.GetAsync(5);
            Assert.False(db.ObjectExists(5));

            result = db.GetAsync(3);
            Assert.Null(result.Result);

            await db.AddAsync(msg2);
            _context.SaveChangesAsync().GetAwaiter();

            result = db.GetFirstOrDefaultAsync(x => x.Id == 2);
            Assert.NotNull(result.Result);
            Assert.Equal("From user1 to user2 msg2", result.Result.Subject);

            var results = db.GetAllAsync();
            Assert.NotNull(results.Result);
            Assert.Equal(2, results.Result.Count());

            await db.RemoveAsync(1);
            _context.SaveChangesAsync().GetAwaiter();
            results = db.GetAllAsync();
            Assert.NotNull(results.Result);
            Assert.Single(results.Result);

            await db.RemoveAsync(msg2);
            _context.SaveChangesAsync().GetAwaiter();
            results = db.GetAllAsync();
            Assert.Empty(results.Result);

            await db.AddAsync(msg1);
            await db.AddAsync(msg2);
            _context.SaveChangesAsync().GetAwaiter();

            //Update not impelemented.
            //db.Update(msg1);

            var messages = new List<Message>() { msg1, msg2 };
            await db.RemoveRangeAsync(messages);
            _context.SaveChangesAsync().GetAwaiter();
            results = db.GetAllAsync();
            Assert.Empty(results.Result);

            await db.AddAsync(msg1);
            await db.AddAsync(msg2);
            await db.AddAsync(msg3);
            await db.AddAsync(msg4);
            await db.AddAsync(msg5);
            _context.SaveChangesAsync().GetAwaiter();

        }
    }
}
//Still need to test these 2 functions.
//IEnumerable<Message> GetAllUserMessages(string sort, string search, string box, string userId);
//MessageViewModel CreateNewMesage(string fromUserId);