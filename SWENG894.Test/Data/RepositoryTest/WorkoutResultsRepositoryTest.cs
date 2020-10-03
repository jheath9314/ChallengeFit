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
using System.Threading;
using System.Threading.Tasks;
using Xunit;


namespace SWENG894.Test.Data.RepositoryTest
{
    public class WorkoutResultsRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkoutResultRepository _cut;

        public WorkoutResultsRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryWorkoutResultsRepository");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new WorkoutResultRepository(_context);
        }

        [Fact]
        public async void WorkoutRepositoryUpdateTest()
        {
            var workoutResult = new WorkoutResult();
            workoutResult.Id = 0;
            workoutResult.Score = 2;
            workoutResult.ScoringType = Workout.Scoring.Reps;

            await _cut.AddAsync(workoutResult);
            await _context.SaveChangesAsync();
            var w = await _cut.GetFirstOrDefaultAsync();
            w.Score = 3;

            _cut.UpdateAsync(w);

            Thread.Sleep(100);

           w = await _cut.GetFirstOrDefaultAsync();

           
            Assert.True(w.Score == 3);
        }

    }
}
