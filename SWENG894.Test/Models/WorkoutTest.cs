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
    public class WorkoutTest
    {

        public WorkoutTest()
        {

        }

        [Fact]
        public void getTimeDisplayTest()
        {
            
            Workout myWorkout = new Workout();
            myWorkout.ScoringType = Workout.Scoring.Time;
            myWorkout.Time = 120;
            Assert.True(myWorkout.getTimeDisplayString() == "2:00");

            myWorkout.Time = 0;
            Assert.True(myWorkout.getTimeDisplayString() == "0:00");

            myWorkout.Time = 297;
            Assert.True(myWorkout.getTimeDisplayString() == "4:57");

            myWorkout.Time = 660;
            Assert.True(myWorkout.getTimeDisplayString() == "11:00");
            
        
        }

        [Fact]
        public void getRepsDisplayTest()
        {
            Workout myWorkout = new Workout();
            myWorkout.ScoringType = Workout.Scoring.Reps;
            myWorkout.Time = 123;

            Assert.True(myWorkout.getTimeDisplayString() == "123");

            myWorkout.Time = 0;
            Assert.True(myWorkout.getTimeDisplayString() == "0");

            myWorkout.Time = 300000;
            Assert.True(myWorkout.getTimeDisplayString() == "300000");


        }


        [Fact]
        public void createWorkoutTest()
        {

            Workout myWorkout = new Workout();

            string workoutName = "Git it";
            string workoutScalingOptions = "Don't scale";
            string workoutNotes = "note test";

            myWorkout.Notes = workoutNotes;
            myWorkout.ScoringType = Workout.Scoring.Reps;
            myWorkout.Time = 123;
            myWorkout.Name = workoutName;
            myWorkout.ScalingOptions = workoutScalingOptions;
            myWorkout.Id = 1;

            Exercise ex = new Exercise();
            ex.Exer = Exercise.Exercises.AirSquat;
            ex.Reps = 5;

            Exercise ex_2 = new Exercise();
            ex.Exer = Exercise.Exercises.PullUp;
            ex.Reps = 10;

            myWorkout.Exercises.Add(ex);
            myWorkout.Exercises.Add(ex_2);

            Assert.True(myWorkout.Exercises.Count == 2);
            Assert.True(myWorkout.ScoringType == Workout.Scoring.Reps);
            Assert.True(myWorkout.Name == workoutName);
            Assert.True(myWorkout.ScalingOptions == workoutScalingOptions);
            Assert.True(myWorkout.Notes == workoutNotes);
            Assert.True(myWorkout.Id == 1);
            


        }
    }
}
