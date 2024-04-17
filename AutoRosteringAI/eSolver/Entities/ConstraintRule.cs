namespace eSolver.Entities
{
    public class ConstraintRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool DateRange { get; set; }
        public bool Offset { get; set; }
        public bool BaseDate { get; set; }
        public bool BaseDateRange { get; set; }
        public bool HoursType { get; set; }
        public bool SetValue { get; set; }
        public bool MaxHours { get; set; }
        public bool IsNightWorker { get; set; }
        public bool ComparationMode { get; set; }
        public bool ComparationValue { get; set; }
        public bool Operator { get; set; }
        public bool EmployeeField { get; set; }
        public bool Amount { get; set; }
        public bool MaxCount { get; set; }
        public bool DayOfWeek { get; set; }
        public bool TimePeriod { get; set; }
        public bool AmountOfDays { get; set; }
        public bool AmountOfHours { get; set; }
        public bool SkipBlankPeriods { get; set; }
        public bool ReferencePeriod { get; set; }
        public bool MustBeConsecutive { get; set; }
        public bool MinimumRestPeriod { get; set; }
        public bool MinimumGap { get; set; }
        public bool Age { get; set; }
        public bool BetweenTimeA { get; set; }
        public bool BetweenTimeB { get; set; }
        public bool RosterRule { get; set; }
        public bool CountOvernights { get; set; }
        public bool EmployeeAID { get; set; }
        public bool EmployeeBID { get; set; }
        public bool JobTypeID { get; set; }
        public bool CustomDataField { get; set; }
    }
}
