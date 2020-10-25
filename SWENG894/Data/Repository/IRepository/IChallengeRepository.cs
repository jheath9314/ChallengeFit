using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository.IRepository
{
    public interface IChallengeRepository : IRepository<Challenge>
    {
        public void UpdateAsync(Challenge challenge);
        public IEnumerable<Challenge> GetUserChallenges(string sort, string search, string userId, bool getOld);
    }
}
