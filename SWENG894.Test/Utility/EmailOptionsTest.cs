using SWENG894.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SWENG894.Test.Utility
{
    public class EmailOptionsTest
    {
        [Fact]
        public void testEmailOptions()
        {
            string sender = "A Prince";
            string smptServer = "spamServerGalore";
            int port = 1234;
            string username = "student@psu.edu";
            string password = "unencrypted";

            EmailOptions opt = new EmailOptions();

            opt.Sender = sender;
            opt.SmtpServer = smptServer;
            opt.Port = port;
            opt.Username = username;
            opt.Password = password;

            Assert.True(opt.Sender == sender);
            Assert.True(opt.SmtpServer == smptServer);
            Assert.True(opt.Port == port);
            Assert.True(opt.Username == username);
            Assert.True(opt.Password == password);
        }
    }
}
