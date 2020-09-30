using FluentAssertions.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SWENG894.Data;
using SWENG894.Data.Initializer;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SWENG894.Test.Data.DbInitializerTest
{
    public class DbInitializerTest : WebApplicationFactory<Startup>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDbInitializer _cut;

            public DbInitializerTest()
            {

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryInit");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context),
                           new RoleValidator<IdentityRole>[0],
                           new UpperInvariantLookupNormalizer(),//new Mock<ILookupNormalizer>().Object,
                           new Mock<IdentityErrorDescriber>().Object,
                           new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

            _userManager = new UserManager<IdentityUser>(
                            new UserStore<IdentityUser>(_context),
                            new Mock<IOptions<IdentityOptions>>().Object,
                            new Mock<IPasswordHasher<IdentityUser>>().Object,
                            new IUserValidator<IdentityUser>[0],
                            new IPasswordValidator<IdentityUser>[0],
                            new UpperInvariantLookupNormalizer(),
                            new Mock<IdentityErrorDescriber>().Object,
                            new Mock<IServiceProvider>().Object,
                            new Mock<ILogger<UserManager<IdentityUser>>>().Object);

            _cut = new DbInitializer(_context, _userManager, _roleManager);
        }

        #region Methods Inherited From Repository

        [Fact]
        public void InitializeTest()
        {
            //Setup

            //Test
            _cut.Initialize();
            Assert.Equal(5, _context.ApplicationUsers.Count());
            Assert.Equal(2, _context.Roles.Count());

            //Verify another call doesn't duplicate users and roles.
            _cut.Initialize();
            Assert.Equal(5, _context.ApplicationUsers.Count());
            Assert.Equal(2, _context.Roles.Count());

            //Reset
        }

        #endregion
    }
}
