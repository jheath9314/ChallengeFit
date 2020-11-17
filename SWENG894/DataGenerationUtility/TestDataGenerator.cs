using Microsoft.AspNetCore.Identity;
using SWENG894.Data;
using SWENG894.Data.Initializer;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.DataGenerationUtility
{
    [ExcludeFromCodeCoverage]
    public class TestDataGenerator : ITestDataGenerator
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TestDataGenerator(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void GenerateTestData()
        {
            string[] lines = System.IO.File.ReadAllLines(@"DataGenerationUtility/names.txt");

            for(int i = 0; i < 1000; i++)
            {
                var firstName = lines[i];
                var lastName = lines[lines.Length - i - 1];
                Random r = new Random();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = firstName + "." + lastName + i,
                    Email = firstName + lastName + "@psufakeemail.edu",
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    ZipCode = "11111",
                    Rating = r.Next(0, 3000)
                }).GetAwaiter().GetResult();
            }

            IList<ApplicationUser> users = _context.ApplicationUsers.Where(x => x.Email.Contains("@psufakeemail.edu")).ToList();

            foreach (ApplicationUser user in users)
            {
                _userManager.AddToRoleAsync(user, "User").GetAwaiter().GetResult();
            }
        }

        public void RemoveTestData()
        {
            IList<ApplicationUser> users = _context.ApplicationUsers.Where(x => x.Email.Contains("@psufakeemail.edu")).ToList();
            _context.RemoveRange(users);
            _context.SaveChanges();
        }       
    }


}
