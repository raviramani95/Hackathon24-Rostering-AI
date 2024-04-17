using System;
using System.Collections.Generic;

namespace eSolver.Entities
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int? OnCallSlots { get; set; }
        public List<ScheduleActiveConstraint> ScheduleActiveConstraints { get; set; }
        public List<ScheduleJob> ScheduleJobs { get; set; }
    }

}
