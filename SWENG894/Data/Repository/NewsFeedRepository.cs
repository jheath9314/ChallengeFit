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
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == userId);

            if(user == null)
            {
                return feedList;
            }

            foreach(var friend in user.Friends)
            {
                if(friend.RequestedById == user.Id)
                {
                    feedList.AddRange(_context.NewFeed.Where(x => x.UserId == friend.RequestedForId && x.CreateDate > DateTime.Now.AddDays(-1)).ToList());
                }
                else
                {
                    feedList.AddRange(_context.NewFeed.Where(x => x.UserId == friend.RequestedById && x.CreateDate > DateTime.Now.AddDays(-1)).ToList());
                }
            }

            return feedList;
        }
    }
}
