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
    public class WorkoutResultTest
    {

        [Fact]
        public void createWorkoutResultTest()
        {

            WorkoutResult result = new WorkoutResult();

            result.User = new ApplicationUser();
            result.Score = 55;
            result.ScoringType = Workout.Scoring.Reps;
            result.Username = "Joe";
            result.Workout = new Workout();
            result.WorkoutId = 1;
            result.UserId = "12";
            result.WorkoutName = "TestName";
            result.WorkoutId = 1;
            result.Id = 1;

            Assert.True(result.User != null);
            Assert.True(result.Score == 55);
            Assert.True(result.ScoringType == Workout.Scoring.Reps);
            Assert.True(result.Username == "Joe");
            Assert.True(result.Workout != null);
            Assert.True(result.WorkoutId == 1);
            Assert.True(result.UserId == "12");
            Assert.True(result.WorkoutName == "TestName");
            Assert.True(result.WorkoutId == 1);
            Assert.True(result.Id == 1);

        }

        [Fact]
        public void getResultScoreTest()
        {
            WorkoutResult result = new WorkoutResult();

            result.ScoringType = Workout.Scoring.Reps;
            result.Score = 123;

            Assert.True(result.getTimeDisplayString() == "123");

            result.Score = 0;
            Assert.True(result.getTimeDisplayString() == "0");

            result.Score = 300000;
            Assert.True(result.getTimeDisplayString() == "300000");
        }

        [Fact]
        public void getTimeDisplayTest()
        {

            Workout myWorkout = new Workout();
            myWorkout.ScoringType = Workout.Scoring.Time;
            myWorkout.Time = 120;
            Assert.True(myWorkout.GetTimeDisplayString() == "2:00");

            myWorkout.Time = 0;
            Assert.True(myWorkout.GetTimeDisplayString() == "0:00");

            myWorkout.Time = 297;
            Assert.True(myWorkout.GetTimeDisplayString() == "4:57");

            myWorkout.Time = 660;
            Assert.True(myWorkout.GetTimeDisplayString() == "11:00");


        }
    }
}
