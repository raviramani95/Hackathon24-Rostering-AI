using eSolver.Entities.Responses;
using System;
using System.Collections.Generic;

namespace eSolver.Entities.Responses
{
    /// <summary>
    /// This class is used to store solution, and print him to conosle
    /// </summary>
    public partial class SolversResponse
    {
        #region Properties
        /// <summary>
        /// Is Solution found
        /// </summary>
        public bool IsSolutionFound { get; set; }
        /// <summary>
        /// Represent solution which is found.
        /// </summary>
        public List<FilledJob> FilledJob { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Iterate trough solution and print it
        /// </summary>
        public void PrintSolution()
        {
            for (int j = 0; j < FilledJob.Count; j++)
            {
                for (int e = 0; e < FilledJob[j].EmployeesID.Count; e++)
                {
                    Console.WriteLine("\nEmployee: " + FilledJob[j].EmployeesID[e] + " work on job " + FilledJob[j].JobID);
                }
            }
        }

        #endregion

    }
}
