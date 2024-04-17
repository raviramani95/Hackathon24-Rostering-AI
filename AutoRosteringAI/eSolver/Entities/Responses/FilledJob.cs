using Newtonsoft.Json;
using System.Collections.Generic;

namespace eSolver.Entities.Responses
{
    public class FilledJob
    {
        public long JobID { get; set; }
        public List<long> EmployeesID { get; set; }

        [JsonIgnore]
        public int AfterSortingJobID { get; set; }
    }

}
