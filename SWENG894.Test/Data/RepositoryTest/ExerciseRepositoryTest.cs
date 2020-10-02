using SWENG894.Data;
using SWENG894.Data.Repository;
using Microsoft.EntityFrameworkCore;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading;

namespace SWENG894.Test.Data.RepositoryTest
{
    public class ExerciseRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IExerciseRepository _cut;
        public ExerciseRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryExercises");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new ExerciseRepository(_context);
        }


        [Fact]
        public async void AddTest()
        {
            var ex1 = new Exercise();
            ex1.Id = 0;
            ex1.Reps = 1;
            ex1.Exer = Exercise.Exercises.AirSquat;

             await _cut.AddAsync(ex1);
            _context.SaveChangesAsync().GetAwaiter();

            var exs = await _cut.GetAllAsync();

            var exList = exs.ToList();

            Assert.True(exList.Count == 1);

            _context.Remove(exList.First());
            _context.SaveChangesAsync().GetAwaiter();
        }

        [Fact]
        public async void UpdateTest()
        {
            var ex1 = new Exercise();
            ex1.Id = 0;
            ex1.Reps = 1;
            ex1.Exer = Exercise.Exercises.AirSquat;

            await _cut.AddAsync(ex1);
            _context.SaveChangesAsync().GetAwaiter();

            var exs = await _cut.GetAllAsync();
            var exList = exs.ToList();

            ex1 = exs.First();

            ex1.Reps = 2;

            _cut.UpdateAsync(ex1);

            Thread.Sleep(100);

           exs = await _cut.GetAllAsync();
           exList = exs.ToList();
           Assert.True(exList.First().Reps == 2);

            //clean up
            _context.Remove(exList.First());
            _context.SaveChangesAsync().GetAwaiter();

        }

    }
}
