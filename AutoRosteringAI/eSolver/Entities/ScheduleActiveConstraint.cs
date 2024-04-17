using eSolver.Entities.Constraints;
using eSolver.Entities.Constraints.Untiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eSolver.Entities
{
    public class ScheduleActiveConstraint
    {
        public int ConstraintRuleID { get; set; }
        public int ScheduleID { get; set; }
        public string ConstraintName { get; set; }
        public bool IsConstraintActive { get; set; }
        public Schedule Schedule { get; set; }

        public object ConstraintRule { get; set; }

        public ConstraintEMWWAEs ConstraintEMWWAEs { get; set; }
        public ConstraintEMNWWAEs ConstraintEMNWWAEs { get; set; }
        public ConstraintMNODOTW ConstraintMNODOTW { get; set; }
        public ConstraintMNOH ConstraintMNOH { get; set; }
        public ConstraintMNOJT ConstraintMNOJT { get; set; }
        public ConstraintSS ConstraintSS { get; set; }
        public ConstraintDOWP ConstraintDOWP { get; set; }
        public ConstraintMNOJTC ConstraintMNOJTC { get; set; }

        [JsonProperty("ConstraintMNOJTIME")]
        public ConstraintMNOJTI ConstraintMNOJTI { get; set; }

        public ConstraintSetValues ConstraintSetValues { get; set; }
        public List<string> ComparisonValues { get; set; }
        public int Id { get; set; }

    }
}
