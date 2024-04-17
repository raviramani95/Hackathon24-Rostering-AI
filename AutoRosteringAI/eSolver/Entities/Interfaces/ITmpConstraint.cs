using eSolver.Entities.Constraints.Untiles;
using System.Collections.Generic;

namespace eSolver.Entities.Interfaces
{
    public abstract class ITmpConstraint
    {
        public List<ScheduleCustomData> CustomData { get; set; }
        // public ScheduleCustomData CustomData { get; set; }

        public ConstraintCustomRange ConstraintCustomRange { get; set; }
        public ConstraintBaseDateRange ConstraintBaseDateRange { get; set; }
        public ConstraintSetValues ConstraintSetValue { get; set; }
        public ConstraintCustomData ConstraintCustomData { get; set; }
        public List<string> ComparisonValues { get; set; }

        public long ConstraintID { get; set; }
        public long? CustomDataID { get; set; }
        public long? ConstraintSetValueID { get; set; }
        public long? ConstraintCustomDataID { get; set; }
        public long? ConstraintCustomRangeID { get; set; }
        public long? ConstraintBaseDateRangeID { get; set; }

        public bool IsPayPeriod { get; set; }
        public bool IsMonth { get; set; }
        public bool IsWeek { get; set; }

        public string getTypeOfConstraintsPeriod()
        {
            if (IsWeek) return "w";
            if (IsMonth) return "m";
            if (IsPayPeriod) return "p";
            if (ConstraintCustomRange != null) return "c";
            if (ConstraintBaseDateRange != null) return "b";

            return "";
        }
    }
}
