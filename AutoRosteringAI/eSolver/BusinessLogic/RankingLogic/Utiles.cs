using eSolver.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eSolver.AutoSolverNew.RankingLogic
{
    static class Utiles
    {
        /// <summary>
        /// Method for checking if job is in the date range.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        public static bool CheckIfJobIsInDateRange(ScheduleJob job, DateTime[] dateRange)
        {
            return job.JobStartDate.Date >= dateRange[0].Date && job.JobStartDate.Date <= dateRange[1].Date;
        }


    }
}
