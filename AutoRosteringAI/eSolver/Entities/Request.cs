using System.Collections.Generic;

namespace eSolver.Entities
{
    public class Request
    {
        public List<SkillMatrix> SkillMatrices { get; set; }
        public List<Schedule> Schedules { get; set; }
        public List<AllConstraintRule> AllConstraintRules { get; set; }
        public List<Employee> Employees { get; set; }
        //public List<EmployeeAssigment> EmployeeAssigments { get; set; }
        public int DesiredJob { get; set; }
        public List<RankingType> RankingTypes { get; set; }
        public List<ScheduleRanking> ScheduleRankings { get; set; }
        public List<RankingRule> RankingRules { get; set; }

    }
}
