using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Math.EC.Rfc7748;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChangesAsync();
        }

        public override async Task<ApplicationUser> GetAsync(int id)
        {
            // AppUser key is a guid. Raise exception if search by id uses an int.
            throw new NotImplementedException();
        }

        public override async Task RemoveAsync(int id)
        {
            // AppUser key is a guid. Raise exception if remove by id uses an int.
            throw new NotImplementedException();
        }

        public override bool ObjectExists(int id)
        {
            // AppUser key is a guid. Raise exception if exists by id uses an int.
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> GetAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task RemoveAsync(string id)
        {
            var entity = await _dbSet.FindAsync(id);
            await RemoveAsync(entity);
        }

        public bool ObjectExists(string id)
        {
            return GetAsync(id).Result != null;
        }

        public void UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> GetUserWithWorkouts(string id)
        {
            return _context.ApplicationUsers.Include(x => x.WorkoutFavorites).ThenInclude(x => x.Workout).SingleOrDefaultAsync(x => x.Id == id);
        }

        public string GetUserByUsername(string userName)
        {
            var temp = _dbSet.FirstOrDefault(u => u.UserName == userName);
            return temp.Id;

        }

        public IEnumerable<ApplicationUser> GetLeaderboard(string userId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                var list = new List<ApplicationUser>();
                var user = _context.ApplicationUsers
                    .Include(u => u.SentFriendRequests)
                    .ThenInclude(u => u.RequestedFor)
                    .Include(u => u.ReceievedFriendRequests)
                    .ThenInclude(u => u.RequestedFor)
                    .FirstOrDefaultAsync(u => u.Id == userId).Result;

                if (user == null)
                {
                    return list;
                }

                foreach (var req in user.Friends)
                {
                    if (req.RequestedForId == user.Id)
                    {
                        list.Add(req.RequestedBy);
                    }
                    else
                    {
                        list.Add(req.RequestedFor);
                    }
                }

                return list.OrderByDescending(x => x.Rating);
            }

            return _context.ApplicationUsers.Where(x => x.EmailConfirmed).OrderByDescending(x => x.Rating);
        }

        public IEnumerable<Challenge> GetRecord(string userId1, string userId2)
        {
            return _context.Challenges
                .Include(x => x.Challenger)
                .Include(x => x.Contender)
                .Include(x => x.ChallengerResult)
                .Include(x => x.ContenderResult)
                .Include(x => x.Workout)
                .Where(x => (x.ChallengerId == userId1 && x.ContenderId == userId2 && x.ChallengeProgress == Challenge.ChallengeStatus.Completed) || (x.ChallengerId == userId2 && x.ContenderId == userId1 && x.ChallengeProgress == Challenge.ChallengeStatus.Completed));
        }

        public List<int> GetAllUserRatings()
        {
            var ratings = new List<int>();
            var appUsers = _context.ApplicationUsers.ToList();
            foreach (var appUser in appUsers)
            {
                ratings.Add((int)appUser.Rating);
            }

            return ratings;
        }
    }
}
