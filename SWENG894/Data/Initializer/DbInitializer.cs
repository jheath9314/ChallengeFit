using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Data.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch
            {

            }

            if (_context.Roles.Any(r => r.Name == "Admin")) return;

            _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("User")).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "stephen@psu.edu",
                Email = "stephen@psu.edu",
                EmailConfirmed = true,
                FirstName = "Stephen",
                LastName = "Cook",
                ZipCode = "11111",
                Rating = 1500.0
            }, "P@ssW0rd").GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "jarrod@psu.edu",
                Email = "jarrod@psu.edu",
                EmailConfirmed = true,
                FirstName = "Jarrod",
                LastName = "Follweiler",
                ZipCode = "22222",
                Rating = 1500.0
            }, "P@ssW0rd").GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "tina@psu.edu",
                Email = "tina@psu.edu",
                EmailConfirmed = true,
                FirstName = "Tina",
                LastName = "Hang",
                ZipCode = "33333",
                Rating = 1500.0
            }, "P@ssW0rd").GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "jeremy@psu.edu",
                Email = "jeremy@psu.edu",
                EmailConfirmed = true,
                FirstName = "Jeremy",
                LastName = "Heath",
                ZipCode = "44444",
                Rating = 1500.0
            }, "P@ssW0rd").GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "vladimir@psu.edu",
                Email = "vladimir@psu.edu",
                EmailConfirmed = true,
                FirstName = "Vladimir",
                LastName = "Novikov",
                ZipCode = "17222",
                Rating = 1500.0
            }, "P@ssW0rd").GetAwaiter().GetResult();

            IList<ApplicationUser> users = _context.ApplicationUsers.ToList();

            foreach (ApplicationUser user in users)
            {
                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }
        }
    }
}
