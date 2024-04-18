using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eSolver.Entities.Responses
{
    public class FilledJob
    {
        public long JobID { get; set; }
        public List<long> EmployeesID { get; set; }
        public string Color { get; set; }
        public List<string> AssignedEmployees { get; set; }      
        public string JobTypeName { get; set; }
        public string JobStartDateTime { get; set; }
        public string JobEndDateTime { get; set; }

        [JsonIgnore]
        public int AfterSortingJobID { get; set; }
    }

}
