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
    }
}
