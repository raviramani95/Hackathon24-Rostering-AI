namespace eSolver.Entities.Constraints
{
    public class ConstraintEMNWWAEs
    {
        public int ConstraintID { get; set; }
        public int EmployeeAID { get; set; }
        public int EmployeeBID { get; set; }
        public string ReferenceArea { get; set; }
        public Employee EmployeeA { get; set; }
        public Employee EmployeeB { get; set; }
        public int Id { get; set; }

    }
}
