namespace eSolver.Entities
{
    public class ComparisonRule
    {
        public int JobTypeID { get; set; }
        public int? CustomDataID { get; set; }
        public string ComparisonMode { get; set; }
        public string Operator { get; set; }
        public string EmployeeField { get; set; }
        public string EmployeeFieldType { get; set; }
        public string TextValue { get; set; }
        public int? NumberValue { get; set; }
    }
}
