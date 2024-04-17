using eSolver.Entities.Responses;

namespace AutoRosteringAI
{
    public class AutoRosterResponse
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
    }
}
