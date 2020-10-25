using Microsoft.EntityFrameworkCore;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class ChallengeRepository : Repository<Challenge>, IChallengeRepository
    {
        private readonly ApplicationDbContext _context;

        public ChallengeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Challenge> GetUserChallenges(string sort, string search, string userId, bool getOld)
        {
            var challenges = _context.Challenges.Where(x => x.ChallengerId == userId || x.ContenderId == userId).Include(x => x.Challenger).Include(x => x.Contender).ToList();

            if (!getOld)
            {
                challenges = challenges.Where(x => x.ChallengeProgress < Challenge.ChallengeStatus.Rejected).ToList();
            }
            else
            {
                challenges = challenges.Where(x => x.ChallengeProgress >= Challenge.ChallengeStatus.Rejected).ToList();
            }

            if (!String.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                challenges = challenges.Where(u => 
                    u.Contender.FullName.ToLower().Contains(search) ||
                    u.Contender.Email.ToLower().Contains(search) || 
                    u.Challenger.FullName.ToLower().Contains(search) ||
                    u.Challenger.Email.ToLower().Contains(search)).ToList();
            }

            challenges = sort switch
            {
                "desc" => challenges.OrderByDescending(s => s.CreateDate).ToList(),
                _ => challenges.OrderBy(s => s.CreateDate).ToList(),
            };

            return challenges;
        }
    }
}
