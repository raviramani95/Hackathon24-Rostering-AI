using System;

namespace eSolver.Entities
{
    /// <summary>
    /// This class represents PayPeriod Configuration
    /// </summary>
    public class PayPeriodDTO
    {
        #region Properties

        /// <summary>
        /// PayPeriod StartDate
        /// </summary>
        public DateTime? PayPeriodStartDate { get; set; }
        /// <summary>
        /// PayPeriod Number of Days
        /// </summary>
        public int? PayPeriodNumberOfDays { get; set; }
        /// <summary>
        /// Is PayPeriod Monthly
        /// </summary>
        public bool IsMonthlyPayPeriod { get; set; }

        #endregion
    }
}
