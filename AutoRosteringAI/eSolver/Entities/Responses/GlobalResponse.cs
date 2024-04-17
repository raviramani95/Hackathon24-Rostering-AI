namespace eSolver.Entities.Responses
{
    public class GlobalResponse
    {
        #region Properties

        /// <summary>
        /// Represents Job Primary Key
        /// </summary>
        public long JobID { get; set; }
        /// <summary>
        /// Represents Employee Primary Key
        /// </summary>
        public long EmployeeID { get; set; }
        /// <summary>
        /// Represents Is Employee Suitable For Job
        /// </summary>
        public bool Value { get; set; }

        #endregion

        #region Constructor's

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GlobalResponse() { Value = true; }
        /// <summary>
        /// Constructor which initialize all properties
        /// </summary>
        /// <param name="jobID">Job Primary Key</param>
        /// <param name="employeeID">Employee Primary Key</param>
        /// <param name="value">Is Employee Suitable For Job</param>
        public GlobalResponse(long jobID, long employeeID, bool value)
        {
            JobID = jobID;
            EmployeeID = employeeID;
            Value = value;
        }

        #endregion
        public void Print()
        {
            System.Console.WriteLine("(" + JobID + ", " + EmployeeID + ", " + Value + ")");
        }
    }
}
