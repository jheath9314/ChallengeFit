using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Models
{
    public class Workout
    {
        public int Id { get; set; }

        public String name { get; set; }

        public int time { get; set; }

        public String notes { get; set; }

        public int reps { get; set; }

        public List<Exercise> Exercises { get; set; }

    }
}
