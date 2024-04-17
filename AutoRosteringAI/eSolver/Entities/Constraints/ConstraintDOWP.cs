using eSolver.Entities.Interfaces;

namespace eSolver.Entities.Constraints
{
    public class ConstraintDOWP : ITmpConstraint
    {
        public int? NumberOfRestDays { get; set; }
        public bool MustBeConsecutive { get; set; }
        public string StartDay { get; set; }
        public int Id { get; set; }
    }
}
