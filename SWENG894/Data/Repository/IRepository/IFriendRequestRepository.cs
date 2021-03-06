using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository.IRepository
{
    public interface IFriendRequestRepository : IRepository<FriendRequest>
    {
        public void UpdateAsync(FriendRequest request);

        public IEnumerable<ApplicationUser> GetUserFriends(string sort, string search, string userId, bool getBlocked);

        public IEnumerable<ApplicationUser> FindNewFriends(string sort, string search, string userId);

        public Task<ApplicationUser> GetPersonToFriend(string id);
    }
}
