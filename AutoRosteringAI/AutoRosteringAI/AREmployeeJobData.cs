namespace AutoRosteringAI
{
    public class Employee
    {
        public int Id { get; set; }
        public List<JobType> JobType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class JobType
    {
        public int Id { get; set; }
        public string JobTypeName { get; set; }
    }

    public class AREmployeeJobData
    {
        public List<Employee> Employees { get; set; }
        public List<ScheduleJob> ScheduleJobs { get; set; }
    }

    public class ScheduleJob
    {
        public int Id { get; set; }
        public string JobStartDateTime { get; set; }
        public string JobEndDateTime { get; set; }
        public int NoOfEmployeesRequired { get; set; }
        public List<JobType> JobType { get; set; }
    }


}
