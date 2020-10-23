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
    public class TestDataGenerator
    {
        private IUnitOfWork unitOfWork;

        private List<ApplicationUser> applicationUserList = new List<ApplicationUser>();

        public TestDataGenerator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async void GenerateTestData()
        {
            string[] lines = System.IO.File.ReadAllLines(@"DataGenerationUtility/names.txt");

            for(int i = 0; i < lines.Length; i++)
            {
                var firstName = lines[i];
                var lastName = lines[lines.Length - i - 1];
                Random r = new Random();


                var user = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    ZipCode = "11111",
                    UserName = firstName + lastName + "@psufakeemail.edu",
                    Email = firstName + lastName + "@psufakeemail.edu",
                    Rating = r.Next(0, 3000),
                };

                await unitOfWork.ApplicationUser.AddAsync(user);
            }

            unitOfWork.ApplicationUser.Save();


        }

        public async void RemoveTestData()
        {
            
            string[] lines = System.IO.File.ReadAllLines(@"DataGenerationUtility/names.txt");
            var userList = new List<ApplicationUser>();

            for (int i = 0; i < lines.Length; i++)
            {
                var firstName = lines[i];
                var lastName = lines[lines.Length - i - 1];
                string UserName = firstName + lastName + "@psufakeemail.edu";
                var appUser = await unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.UserName == UserName);
                userList.Add(appUser);
            }
            

            await unitOfWork.ApplicationUser.RemoveRangeAsync(userList);
            unitOfWork.ApplicationUser.Save();
        }
        
    }


}
