using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        public void UpdateAsync(ApplicationUser user);

        public Task<ApplicationUser> GetAsync(string id);

        public Task RemoveAsync(string id);

        public bool ObjectExists(string id);

        public Task<ApplicationUser> GetUserWithWorkouts(string id);

        public string GetUserByUsername(string userName);

        public IEnumerable<ApplicationUser> GetLeaderboard(string userId);

        public IEnumerable<Challenge> GetRecord(string userId1, string userId2);

        public List<int> GetAllUserRatings();

        public void UpdateRating(double newRating, string userId);

        public void Save();
    }
}
