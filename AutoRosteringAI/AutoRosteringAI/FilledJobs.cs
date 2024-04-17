using System.Text.Json.Serialization;

namespace AutoRosteringAI
{
    public class FilledJobs
    {
        public long JobID { get; set; }
        public List<long> EmployeesID { get; set; }

        [JsonIgnore]
        public int AfterSortingJobID { get; set; }

    }
}
