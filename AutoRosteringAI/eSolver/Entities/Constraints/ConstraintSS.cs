using eSolver.Entities.Interfaces;

namespace eSolver.Entities.Constraints
{
    public class ConstraintSS : ITmpConstraint
    {
        public double MinimumRestPeriod { get; set; }
        public double MaximumGap { get; set; }
        public int Id { get; set; }
    }
}
