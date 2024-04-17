using eSolver.Entities;
using System.Collections.Generic;

namespace eSolver.Entities.OutSideView
{
    public class Assigment
    {
        public List<int> EmployeeIDs { get; set; }
        public ScheduleJob Job { get; set; }
    }
}