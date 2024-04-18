using System.Text;

namespace AutoRosteringAI
{

    public class AllConstraintRule

    {

        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class ConstraintRule

    {

        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class Employees

    {

        public int Id { get; set; }

        public int JobTypeID { get; set; }

        public DateTime StartDate { get; set; }

        public object LeaveDate { get; set; }

        public object DateOfBirth { get; set; }

        public string EmployeeNumber { get; set; }

        public string Firstname { get; set; }

        public string Surname { get; set; }

        public object Address2 { get; set; }

        public object Address1 { get; set; }

        public object Address3 { get; set; }

        public object CostCode { get; set; }

        public object Address4 { get; set; }

        public object TelephoneNumber { get; set; }

        public object MobileNumber { get; set; }

        public object TargetRuleGroup { get; set; }

        public object WorkRules { get; set; }

        public object Email { get; set; }

        public object Username { get; set; }

        public object EmployeeProfile { get; set; }

        public object Gender { get; set; }

        public object Department { get; set; }

        public object Division { get; set; }

        public object JobTitle { get; set; }

        public object Team { get; set; }

        public object Class { get; set; }

        public object Notification { get; set; }

        public object Manager { get; set; }

        public object JobType { get; set; }

        public List<object> Locations { get; set; }

        public int PayRate { get; set; }

        public int MaxHours1 { get; set; }

        public object MaxHours2 { get; set; }

        public object MaxHours3 { get; set; }

        public object MaxHours4 { get; set; }

        public object MaxHours5 { get; set; }

        public object WeeklyHours { get; set; }

        public bool AllowedTrades { get; set; }

        public bool Availability { get; set; }

        public object EmployeeAvailabilities { get; set; }

    }

    public class NonEssentialSkill

    {

        public int SkillCodeID { get; set; }

        public int JobTypeID { get; set; }

        public string SkillGroup { get; set; }

        public object SkillCode { get; set; }

        public object JobType { get; set; }

    }

    public class PayPeriodDTO

    {

        public string PayPeriodStartDate { get; set; }

        public object PayPeriodNumberOfDays { get; set; }

        public bool IsMonthlyPayPeriod { get; set; }

    }

    public class RankingType

    {

        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class Result

    {

        public int MaxNumOfJobsAtSameTime { get; set; }

        public double MaxTime { get; set; }

        public string StartDayOfTheWeek { get; set; }

        public bool Maximize { get; set; }

        public bool FinalConsoleOutput { get; set; }

        public bool PrintSolutions { get; set; }

        public List<Employees> Employees { get; set; }

        public List<object> JobOutsides { get; set; }

        public List<object> ScheduleRankings { get; set; }

        public List<object> RankingRules { get; set; }

        public List<AllConstraintRule> AllConstraintRules { get; set; }

        public List<RankingType> RankingTypes { get; set; }

        public Schedule Schedule { get; set; }

        public PayPeriodDTO PayPeriodDTO { get; set; }

        public object canChangeEmployeeOnTemplateJobs { get; set; }

        public List<NonEssentialSkill> NonEssentialSkills { get; set; }

        public List<SkillMatrixList> SkillMatrixList { get; set; }

    }

    public class Schedule

    {

        public int ScheduleID { get; set; }

        public List<ScheduleActiveConstraint> ScheduleActiveConstraints { get; set; }

        public List<ScheduleJobs> ScheduleJobs { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public object ApplyConstraintFromScheduleId { get; set; }

        public object InsertedTemplates { get; set; }

    }

    public class ScheduleActiveConstraint

    {

        public int Id { get; set; }

        public string ConstraintName { get; set; }

        public bool IsConstraintActive { get; set; }

        public object ComparisonValues { get; set; }

        public int ConstraintRuleID { get; set; }

        public int ScheduleID { get; set; }

        public Schedule Schedule { get; set; }

        public ConstraintRule ConstraintRule { get; set; }

        public object ConstraintEMWWAEs { get; set; }

        public object ConstraintEMNWWAEs { get; set; }

        public object ConstraintSetValues { get; set; }

        public object ConstraintMNODOTW { get; set; }

        public object ConstraintMNOH { get; set; }

        public object ConstraintMNOJTC { get; set; }

        public object ConstraintDOWP { get; set; }

        public object ConstraintSS { get; set; }

        public object ConstraintMNOJT { get; set; }

        public object ConstraintMNOJTIME { get; set; }

        public List<object> BrokenRules { get; set; }

    }

    public class ScheduleJobs

    {

        public int Id { get; set; }

        public DateTime JobStartDateTime { get; set; }

        public DateTime JobEndDateTime { get; set; }

        public int Hours { get; set; }

        public int Hours1 { get; set; }

        public int Hours2 { get; set; }

        public int Hours3 { get; set; }

        public int Hours4 { get; set; }

        public int Hours5 { get; set; }

        public int Hours6 { get; set; }

        public int NoOfEmployeesRequired { get; set; }

        public int SubgroupID { get; set; }

        public int JobtypeID { get; set; }

        public List<object> JobCustomData { get; set; }

        public List<object> UnavailableEmployeesForJob { get; set; }

        public List<object> AlreadyAssignedEmployeeOnJob { get; set; }

        public bool isTemplateJob { get; set; }

    }

    public class SkillMatrixList

    {

        public int Id { get; set; }

        public bool Value { get; set; }

        public int EmployeeID { get; set; }

        public int SkillCodeID { get; set; }

        public object Employee { get; set; }

        public object SkillCode { get; set; }

    }

}
