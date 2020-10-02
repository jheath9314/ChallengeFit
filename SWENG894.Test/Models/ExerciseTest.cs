using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Org.BouncyCastle.Crypto.Digests;
using SWENG894.Areas.User.Controllers;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SWENG894.Test.Models
{
    public class ExerciseTest
    {
        [Fact]
        public void createExerciseTest()
        {
            Exercise ex = new Exercise();
            ex.Exer = Exercise.Exercises.AirSquat;
            ex.Reps = 12;
            ex.Id = 1;
            ex.WorkoutId = 2;

            Assert.True(ex.Exer == Exercise.Exercises.AirSquat);
            Assert.True(ex.Reps == 12);
            Assert.True(ex.Id == 1);
            Assert.True(ex.WorkoutId == 2);

            ex.Workout = new Workout();
            Assert.True(ex.Workout != null);

        }
    }
}
