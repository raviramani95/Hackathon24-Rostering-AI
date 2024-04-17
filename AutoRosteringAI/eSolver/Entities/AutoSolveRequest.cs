using eSolver.Entities;
using eSolver.Entities.OutSideView;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace eSolver.Entities
{
    /// <summary>
    /// Model which is passed to AutoSolve, this is entry point
    /// </summary>
    public class AutoSolveRequest
    {
        #region Properties
        /// <summary>
        /// Schedule which will be AutoSolved
        /// </summary>
        public Schedule Schedule { get; set; }
        /// <summary>
        /// All ConstrintRules Specified in System
        /// </summary>
        [JsonProperty("AllConstraintRules")]
        public List<AllConstraintRule> ConstraintRules { get; set; }
        /// <summary>
        /// All Employees that could be assigned on some job
        /// </summary>
        public List<Employee> Employees { get; set; }
        /// <summary>
        /// All RankingTypes specified in System
        /// </summary>
        public List<RankingType> RankingTypes { get; set; }
        /// <summary>
        /// All ScheduleRankings specified in System
        /// </summary>
        public List<ScheduleRanking> ScheduleRankings { get; set; }
        /// <summary>
        /// All RankingRules specified in System
        /// </summary>
        public List<RankingRule> RankingRules { get; set; }
        /// <summary>
        /// PAyPeriod object
        /// </summary>
        public PayPeriodDTO PayPeriodDTO { get; set; }
        /// <summary>
        /// Start Day of the week specified in system
        /// </summary>
        public string StartDayOfTheWeek { get; set; }
        /// <summary>
        /// Should application print solution
        /// </summary>
        public bool PrintSolutions { get; set; }
        /// <summary>
        ///  Max time that solver will spent in order to find solution
        /// </summary>
        public float MaxTime { get; set; }
        /// <summary>
        /// True : It will try to insert as many employees as he can. False : It will fynding solution one per one empl
        /// </summary>
        public bool Maximize { get; set; }
        /// <summary>
        /// Jobs outised of Solver range
        /// </summary>
        public List<JobFromOutside> JobOutsides { get; set; }
        /// <summary>
        /// Can employee be changed on already assigned template jobs
        /// </summary>
        public bool? CanChangeEmployeeOnTemplateJobs { get; set; }

        /// <summary>
        /// NonEssentialSkills
        /// </summary>
        public List<JobTypeNonEssentialSkill> NonEssentialSkills { get; set; }

        /// <summary>
        /// SkillMatrixList
        /// </summary>
        public List<SkillMatrix> SkillMatrixList { get; set; }

        #endregion
    }
}
