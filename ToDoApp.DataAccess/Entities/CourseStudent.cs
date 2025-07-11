﻿using System.ComponentModel.DataAnnotations.Schema;
using ToDoApp.DataAccess.Entities;

namespace ToDoApp.DataAccess.Entities
{
    public class CourseStudent
    {
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
        public double AssignmentScore { get; set; }
        public double PracticalScore { get; set; }
        public double FinalScore { get; set; }
    }
}
