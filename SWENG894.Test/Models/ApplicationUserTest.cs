using System;
using Xunit;
using SWENG894.Models;

namespace SWENG894.Test
{
    public class ApplicationUserTest
    {
        [Fact]
        public void Test1()
        {
            ApplicationUser appUser = new ApplicationUser();
            appUser.FirstName = "John";
            appUser.LastName = "Doe";
            appUser.ZipCode = "53202";
            appUser.Role = "User";

            Assert.Equal("John",  appUser.FirstName);
            Assert.Equal("Doe",   appUser.LastName);
            Assert.Equal("53202", appUser.ZipCode);
            Assert.Equal("User",  appUser.Role);
        }
        
        //FullName
        //tests FullName assignent is valid
        [Fact]
        public void FullNameIsValid()
        {
            //arrange
            ApplicationUser appUser = new ApplicationUser();
            appUser.FirstName = "John";
            appUser.LastName = "Doe";
            appUser.ZipCode = "53202";
            appUser.Role = "User";

            //act
            var FullName = appUser.FullName;

            //assert
            Assert.Equal("John Doe", FullName);
            
        }
        
        //tests FullName assignent is NOT null
        [Fact]
        public void FullNameIsNotNull()
        {
            //arrange
            ApplicationUser appUser = new ApplicationUser();
            appUser.FirstName = "John";
            appUser.LastName = "Doe";
            appUser.ZipCode = "53202";
            appUser.Role = "User";

            //act
            var FullName = appUser.FullName;

            //assert
            Assert.NotNull(FullName);
        }
        
        
        
    }
}
