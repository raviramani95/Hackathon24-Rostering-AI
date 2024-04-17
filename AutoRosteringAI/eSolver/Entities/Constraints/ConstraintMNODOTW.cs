using eSolver.Entities.Interfaces;

namespace eSolver.Entities.Constraints
{
    public class ConstraintMNODOTW : ITmpConstraint
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; }
        public int MaxCount { get; set; }
        public bool CountOvernights { get; set; }


    }
}
