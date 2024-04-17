using eSolver.Entities.Interfaces;
using Newtonsoft.Json;

namespace eSolver.Entities.Constraints
{
    public class ConstraintMNOJT : ITmpConstraint
    {
        public int JobTypeID { get; set; }

        [JsonProperty("MaxCount")]
        public int MaximalCount { get; set; }
        
    }
}
