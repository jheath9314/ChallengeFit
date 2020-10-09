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
    }
}
