using eSolver.Entities.Interfaces;

namespace eSolver.Entities.Constraints
{
    public class ConstraintMNOH : ITmpConstraint
    {

        public bool? IsNightWorker { get; set; }
        public double? SetValue { get; set; }
        public string MaxHoursField { get; set; }
        public int Id { get; set; }


        // From interface
        /*
        public int ConstraintID { get; set; }
        public int? CustomDataID { get; set; }
        public int? ConstraintCustomRangeID { get; set; }
        public int? ConstraintBaseDateRangeID { get; set; }
        public int? ConstraintSetValueID { get; set; }
        public int? ConstraintCustomDataID { get; set; }
        public bool IsPayPeriod { get; set; }
        public bool IsMonth { get; set; }
        public bool IsWeek { get; set; }
        public List<ScheduleCustomData> CustomData { get; set; }
        public ConstraintSetValues ConstraintSetValue { get; set; }
        public ConstraintCustomData ConstraintCustomData { get; set; }
        public ConstraintCustomRange ConstraintCustomRange { get; set; }
        public ConstraintBaseDateRange ConstraintBaseDateRange { get; set; }*/

    }
}
