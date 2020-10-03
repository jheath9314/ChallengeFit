using Microsoft.EntityFrameworkCore;
using SWENG894.Areas.User.Controllers;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Models;
using Xunit;

namespace SWENG894.Test.Controllers
{
    public class WorkoutResultsControllerTest
    {
        private readonly ApplicationDbContext _context;
        public WorkoutResultsControllerTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryWorkoutResults" +
                "Controller");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async void WorkoutResultsContollerTestOp()
        {
            WorkoutResult res = new WorkoutResult();
            var unit = new UnitOfWork(_context);
            var cont = new WorkoutResultsController(unit);

            Workout w = new Workout();
            w.Id = 0;
            ApplicationUser u = new ApplicationUser();
            u.Id = "";

            _context.Workouts.Add(w);
            _context.ApplicationUsers.Add(u);
            await _context.SaveChangesAsync();

            res.Id = 0;
            res.UserId = "1";
            res.WorkoutId = 1;

            //ClaimTypes

            //await cont.Create(1, res, 10);

            //var data = _context.WorkoutResults.FirstOrDefaultAsync();

            //Assert.True(data != null);

            
        }
    }
}
