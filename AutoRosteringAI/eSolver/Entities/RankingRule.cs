using System;
using System.Collections.Generic;

namespace eSolver.Entities
{
    public class RankingRule
    {
        public int Id { get; set; }
        public bool ReverseOrder { get; set; }
        public string DateRange { get; set; }
        public double? Offset { get; set; }
        public double? Amount { get; set; }
        public DateTime? BaseDate { get; set; }
        public int? BaseDateRange { get; set; }
        public string DayOfWeek { get; set; }
        public bool? CountOvernights { get; set; }
        public DateTime? JobStartFrom { get; set; }
        public DateTime? JobStartTo { get; set; }
        public DateTime? JobEndFrom { get; set; }
        public DateTime? JobEndTo { get; set; }
        public int? RestrictedToJobTypeID { get; set; }
        public Jobtype JobType { get; set; }
        public string ComparisonMode { get; set; }
        public string ComparisonValue { get; set; }
        public List<string> ComparisonValues { get; set; }
        public string Operator { get; set; }
        public string EmployeeField { get; set; }
        //public int CustomDataFieldID { get; set; }
        public int? CustomDataID { get; set; } // 28.04.2020
    }
}
