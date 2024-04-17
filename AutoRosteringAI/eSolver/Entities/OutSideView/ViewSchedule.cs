using System;
using eSolver.Entities;
using System.Collections.Generic;
using System.Text;

namespace eSolver.Entities.OutSideView
{
    public class ViewSchedule
    {
        public List<ScheduleCustomData> ScheduleCustomData { get; set; }
        public List<Assigment> Assigments { get; set; }
    }
}
