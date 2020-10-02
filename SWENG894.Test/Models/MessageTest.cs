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

namespace SWENG894.Test.Models
{
    public class MessageTest
    {

        [Fact]
        public void testMessages()
        {
            Message m = new Message();
            string messageBody = "1234567";
            string subject = "12345678";
            m.Body = messageBody;
            m.DeletedByReceiver = true;
            m.DeletedBySender = true;
            m.Id = 1;
            m.ReadStatus = Message.MessageReadStatud.New;
            m.SendStatus = Message.MessageSendStatud.New;
            m.SentBy = new ApplicationUser();
            m.SentById = "1";
            m.SentTime = new DateTime();
            m.SentTo = new ApplicationUser();
            m.SentToId = "1";
            m.Subject = subject;

            Assert.True(m.Body == messageBody);
            Assert.True(m.Subject == subject);
            Assert.True(m.DeletedByReceiver);
            Assert.True(m.DeletedBySender);
            Assert.True(m.Id == 1);
            Assert.True(m.ReadStatus == Message.MessageReadStatud.New);
            Assert.True(m.SendStatus == Message.MessageSendStatud.New);
            Assert.True(m.SentBy != null);
            Assert.True(m.SentById == "1");
            Assert.True(m.SentTime != null);
            Assert.True(m.SentTo != null);
            Assert.True(m.SentToId == "1");
            Assert.True(m.Subject == subject);

            Console.WriteLine("Value: " + m.SubjectPreview);
            Assert.True(m.SubjectPreview == "1234567...");









        }

    }
}
