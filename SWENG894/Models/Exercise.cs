using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public enum exercises
        {
            Pushup,
            Benchpress,
            Burpee,
            Dip,
            MuscleUp,
            PullUp,
            AirSquat,
            FrontSquat,
            BackSquat,
            OverheadSquare,
            Deadlift,
            SitUp,
            JumpRope,
            Run,
            Swim,
            Clean,
            Snatch,
            Jerk
        }

        public exercises exercise;

        public int reps { get; set; }
    }
}
