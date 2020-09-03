using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Utility
{
    public class EmailOptions
    {
        public string Sender { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
