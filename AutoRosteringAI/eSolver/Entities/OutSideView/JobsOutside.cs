
using eSolver.Entities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eSolver.Entities.OutSideView
{ 
    public class JobFromOutside : IJobBase
    {
        #region JobBaseImplementation

        [JsonProperty("Id")]
        public long JobID { get; set; }
        [JsonProperty("JobStartDateTime")]
        public DateTime JobStartDate{ get; set; }
        [JsonProperty("JobEndDateTime")]
        public DateTime JobEndDate { get; set; }
        public TimeSpan HoursT { get; set; }
        public TimeSpan HoursT1 { get; set; }
        public TimeSpan HoursT2 { get; set; }
        public TimeSpan HoursT3 { get; set; }
        public TimeSpan HoursT4 { get; set; }
        public TimeSpan HoursT5 { get; set; }
        public TimeSpan HoursT6 { get; set; }
        public long Hours { get; set; }
        public long Hours1 { get; set; }
        public long Hours2 { get; set; }
        public long Hours3 { get; set; }
        public long Hours4 { get; set; }
        public long Hours5 { get; set; }
        public long Hours6 { get; set; }
        public int COUNT_OF_MILISECONDS { get; } = 10000000;
        public List<ScheduleCustomData> JobCustomData { get; set; }
        public long SubGroupID { get; set; }
        public long JobTypeID { get; set; }

        #endregion

        #region Properties
        /// <summary>
        /// Ids of employees which are assugned to this Job
        /// </summary>
        public List<int> EmployeeAssigments { get; set; }
        /// <summary>
        /// Schedule where Job belongs
        /// </summary>
        public long ScheduleID { get; set; }

        #endregion
    }
}
