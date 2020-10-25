using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class NewsFeedRepository : Repository<NewsFeed>, INewsFeedRepository
    {
        private readonly ApplicationDbContext _context;

        public NewsFeedRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<NewsFeed> GetUserFeed(string userId)
        {
            var feedList = new List<NewsFeed>();
            var user = _context.ApplicationUsers
                .Include(x => x.SentFriendRequests)
                .ThenInclude(x => x.RequestedFor)
                .Include(x => x.ReceievedFriendRequests)
                .ThenInclude(x => x.RequestedBy)
                .FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return feedList;
            }

            foreach(var friend in user.Friends)
            {
                if(friend.RequestedById == user.Id)
                {
                    feedList.AddRange(_context.NewsFeed.Where(x => x.UserId == friend.RequestedForId && x.CreateDate > DateTime.Now.AddDays(-1))
                        .Include(x => x.User)
                        .Include(x => x.RelatedUser)
                        .Include(x => x.RelatedChallenge)
                        .Include(x => x.RelatedWorkout).ToList());
                }
                else
                {
                    feedList.AddRange(_context.NewsFeed.Where(x => x.UserId == friend.RequestedById && x.CreateDate > DateTime.Now.AddDays(-1))
                        .Include(x => x.User)
                        .Include(x => x.RelatedUser)
                        .Include(x => x.RelatedChallenge)
                        .Include(x => x.RelatedWorkout).ToList());
                }
            }

            return feedList;
        }
    }
}
