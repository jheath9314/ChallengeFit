﻿using Microsoft.EntityFrameworkCore;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Repository
{
    public class FriendRequestRepository : Repository<FriendRequest>, IFriendRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public FriendRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<ApplicationUser> FindNewFriends(string sort, string search, string userId)
        {
            var user = _context.ApplicationUsers
                    .Include(u => u.SentFriendRequests)
                    .ThenInclude(u => u.RequestedFor)
                    .Include(u => u.ReceievedFriendRequests)
                    .ThenInclude(u => u.RequestedFor)
                    .FirstOrDefaultAsync(u => u.Id == userId).Result;

            var matchingUsers = _context.ApplicationUsers.Where(u => u.EmailConfirmed == true).ToList();
            matchingUsers.Remove(user);

            if (!String.IsNullOrEmpty(search))
            {
                matchingUsers = matchingUsers.Where(u => u.LastName.ToLower().Contains(search) ||
                    u.FirstName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search)).ToList();
            }

            matchingUsers = sort switch
            {
                "desc" => matchingUsers.OrderByDescending(s => s.LastName).ToList(),
                _ => matchingUsers.OrderBy(s => s.LastName).ToList(),
            };

            foreach (var request in user.SentFriendRequests)
            {
                matchingUsers.Remove(request.RequestedFor);
            }

            foreach (var request in user.ReceievedFriendRequests)
            {
                matchingUsers.Remove(request.RequestedBy);
            }

            return matchingUsers;
        }

        public IEnumerable<FriendRequest> GetAllUserFriendRequests(string sort, string search, string userId)
        {
            var user = _context.ApplicationUsers
                .Include(u => u.SentFriendRequests)
                .ThenInclude(u => u.RequestedFor)
                .Include(u => u.ReceievedFriendRequests)
                .ThenInclude(u => u.RequestedBy)
                .FirstOrDefaultAsync(u => u.Id == userId).Result;

            var matchingUsers = user.Friends;

            if (!String.IsNullOrEmpty(search))
            {
                matchingUsers = user.Friends.Where(u => u.RequestedFor.LastName.ToLower().Contains(search) ||
                    u.RequestedFor.FirstName.ToLower().Contains(search) ||
                    u.RequestedFor.Email.ToLower().Contains(search)).ToList();
            }

            matchingUsers = sort switch
            {
                "desc" => matchingUsers.OrderByDescending(s => s.RequestedFor.LastName).ToList(),
                _ => matchingUsers.OrderBy(s => s.RequestedFor.LastName).ToList(),
            };

            return matchingUsers.ToList();
        }

        public async Task<ApplicationUser> GetPersonToFriend(string id)
        {
            return await _context.ApplicationUsers
                .Include(u => u.ReceievedFriendRequests)
                .ThenInclude(u => u.RequestedBy)
                .FirstOrDefaultAsync(m => m.Id == id && m.EmailConfirmed == true);
        }

        public void UpdateAsync(FriendRequest request)
        {
            throw new NotImplementedException();
        }

        public override Task<FriendRequest> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public override bool ObjectExists(int id)
        {
            throw new NotImplementedException();
        }
    }
}
