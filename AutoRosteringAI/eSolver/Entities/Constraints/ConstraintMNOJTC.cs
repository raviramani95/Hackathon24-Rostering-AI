using eSolver.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace eSolver.Entities.Constraints
{
    public class ConstraintMNOJTC : ITmpConstraint
    {
        public int JobTypeID { get; set; }
        public int MaximalCount { get; set; } //MaximalCount
        public int RestDays { get; set; }
    }

    public class InfoDay 
    {
        public bool WorkAtAnyJob { get; set; }
        public bool WorkAtSpecificJobTypeJob { get; set; }
        public DateTime Date { get; set; }
    }
}
