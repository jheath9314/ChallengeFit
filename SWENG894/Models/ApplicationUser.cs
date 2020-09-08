using Microsoft.AspNetCore.Identity;
using SWENG894.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            SentFriendRequests = new List<FriendRequest>();
            ReceievedFriendRequests = new List<FriendRequest>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ZipCode { get; set; }

        public ICollection<FriendRequest> SentFriendRequests { get; set; }
        public ICollection<FriendRequest> ReceievedFriendRequests { get; set; }

        [NotMapped]
        public ICollection<FriendRequest> Friends
        {
            get
            {
                var friends = SentFriendRequests.Where(x => x.Approved).ToList();
                friends.AddRange(ReceievedFriendRequests.Where(x => x.Approved));
                return friends;
            }
        }

        [NotMapped]
        public string Role { get; set; }

        public bool HasRequestFrom(string uid)
        {
            if (ReceievedFriendRequests.Where(r => r.RequestedById == uid).Count() <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
