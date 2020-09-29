using Microsoft.EntityFrameworkCore.Metadata;
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
    }
}
