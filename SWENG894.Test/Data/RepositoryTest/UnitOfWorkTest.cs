using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Org.BouncyCastle.Crypto.Digests;
using SWENG894.Areas.User.Controllers;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SWENG894.Test.Data.RepositoryTest
{
    public class UnitOfWorkTest
    {
        private readonly ApplicationDbContext _context;
        private readonly UnitOfWork _cut;
        
        public UnitOfWorkTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryUnitOfWork");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new UnitOfWork(_context);
        }

        [Fact]
        public void TestUnitOfWorkDatabases()
        {
            Assert.True(_cut.ApplicationUser != null);
            Assert.True(_cut.Message != null);
            Assert.True(_cut.FriendRequest != null);
            Assert.True(_cut.Workout != null);
            Assert.True(_cut.Exercise != null);
            Assert.True(_cut.WorkoutResult != null);
        }


        [Fact]
        public async void TestUnitOfWorkSaving()
        {
            var ex = new Exercise();
            ex.Id = 0;
            ex.Reps = 1;
            ex.Exer = Exercise.Exercises.BackSquat;

            await _cut.Exercise.AddAsync(ex);

            await _cut.Save();

            var exs = await _cut.Exercise.GetAllAsync();
            var exList = exs.ToList();

            Assert.True(exList.Count > 0);


        }


    }
}
