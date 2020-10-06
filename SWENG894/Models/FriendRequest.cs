using SWENG894.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class FriendRequest
    {
        public string RequestedById { get; set; }
        public ApplicationUser RequestedBy { get; set; }

        public string RequestedForId { get; set; }
        public ApplicationUser RequestedFor { get; set; }

        public DateTime? RequestTime { get; set; }

        public DateTime? BecameFriendsTime { get; set; }

        public FriendRequestStatus RequestStatus { get; set; }

        public FriendRequestStatus RequesterStatus { get; set; }

        public FriendRequestStatus ReceiverStatus { get; set; }

        [NotMapped]
        public bool RequestApproved => RequestStatus == FriendRequestStatus.Approved;

        [NotMapped]
        public bool RequesterApproved => RequesterStatus == FriendRequestStatus.Approved;

        [NotMapped]
        public bool ReceiverApproved => ReceiverStatus == FriendRequestStatus.Approved;

        public enum FriendRequestStatus
        {
            New,
            Approved,
            Rejected
        }
    }
}
