using System;

namespace eSolver.Entities
{
    public class EmployeeAssigment
    {
        public int EmployeeID { get; set; }
        public int JobID { get; set; }
        public DateTime JobStartDate { get; set; }
        public DateTime JobEndDate { get; set; }
        public TimeSpan JobStartTime { get; set; }
        public TimeSpan JobEndTime { get; set; }
    }

}
