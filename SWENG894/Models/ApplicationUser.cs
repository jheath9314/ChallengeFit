using Microsoft.AspNetCore.Identity;
using SWENG894.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Constructors
        public ApplicationUser()
        {
            SentFriendRequests = new List<FriendRequest>();
            ReceievedFriendRequests = new List<FriendRequest>();
        }

        // Additional properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ZipCode { get; set; }

        [NotMapped]
        [Display(Name = "Name")]
        public string FullName => FirstName + " " + LastName;

        [NotMapped]
        public string Role { get; set; }

        // Friend requests
        public ICollection<FriendRequest> SentFriendRequests { get; set; }
        public ICollection<FriendRequest> ReceievedFriendRequests { get; set; }

        [NotMapped]
        public ICollection<FriendRequest> Friends
        {
            get
            {
                var friends = SentFriendRequests.Where(x => x.RequestApproved).ToList();
                friends.AddRange(ReceievedFriendRequests.Where(x => x.RequestApproved));
                return friends;
            }
        }

        [NotMapped]
        public ICollection<FriendRequest> BlockedFriends
        {
            get
            {
                var friends = SentFriendRequests.Where(x => !x.RequestApproved).ToList();
                friends.AddRange(ReceievedFriendRequests.Where(x => !x.RequestApproved));
                return friends;
            }
        }

        public bool HasRequestFrom(string uid)
        {
            if (ReceievedFriendRequests.Where(r => r.RequestedById == uid).Count() <= 0)
            {
                return false;
            }
            return true;
        }

        // Messages
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceievedMessages { get; set; }

    }
}
