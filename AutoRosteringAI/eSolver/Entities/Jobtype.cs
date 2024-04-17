using System.Collections.Generic;

namespace eSolver.Entities
{
    public class Jobtype
    {
        public int JobtypeID { get; set; }
        public string TimeUnitName { get; set; }
        public int? TimeUnitValue { get; set; }
        public string SkillCodeFormula { get; set; }
        public List<ComparisonRule> ComparisonRules { get; set; }
    }

}
