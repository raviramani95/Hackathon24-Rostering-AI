using System;

namespace eSolver.Entities
{

    public class Schedule2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public object OnCallSlots { get; set; }
        public object ScheduleCustomData { get; set; }
    }

}
