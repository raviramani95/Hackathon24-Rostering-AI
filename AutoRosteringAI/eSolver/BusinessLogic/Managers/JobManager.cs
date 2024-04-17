using eSolver.Entities;
using eSolver.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eSolver.BusinessLogic.Managers
{
    public class JobManager
    {
        /// <summary>
        /// Sorting jobs in chronological order of the starting times
        /// </summary>
        /// <param name="jobs">Job which need to be solved</param>
        /// <returns>List of Sorted Jobs</returns>
        /// <exception cref="ArgumentNullException">This exception will be occured if some job have JobStartDate NULL Value.</exception>
        public List<ScheduleJob> BubbleSort(List<ScheduleJob> jobs)
        {
            return jobs.OrderBy(o => (o.JobStartDate)).ToList();
        }

        /// <summary>
        /// Creating List of dates. Newlly created List will be populated with Start and End dates from passed Collection.
        /// </summary>
        /// <param name="jobs">Collection of ScheduleJob. See <see cref="ScheduleJob"/> for more details.</param>
        /// <returns>List of sorted data points for the list of the jobs</returns>
        public List<DateTime> SortChronologicalIntervals(List<ScheduleJob> jobs)
        {
            // Collcetion which will be returned
            List<DateTime> listDates = new List<DateTime>(jobs.Count * 2);
            // Iterate trough passed collection and populate retValue of function.
            foreach (ScheduleJob job in jobs)
            {
                listDates.Add(job.JobEndDate);
                listDates.Add(job.JobStartDate);
            }

            return listDates;
        }

        /// <summary>
        /// For passed jobs and dates, this method will find all jobs that are overlaping based on Start and End Date.
        /// </summary>
        /// <param name="jobs"></param>
        /// <param name="dateTimes"></param>
        /// <returns>Collection of ordered pairs, those pairs are JobIds which are overlaping</returns>
        public List<List<long>> FindOverLappingNIntervals(List<ScheduleJob> jobs, List<DateTime> dateTimes)
        {
            // Collection which will be returned
            List<List<long>> jobsWhichAreOverlaping = new List<List<long>>();
            for (int d = 0; d < dateTimes.Count - 1; d++)
            {
                // One item in collection which will be returned
                if (dateTimes[d] != dateTimes[d + 1])
                {
                    List<long> orderedPair = new List<long>();
                    for (int j = 0; j < jobs.Count; j++)
                    {
                        if ((jobs[j].JobStartDate < dateTimes[d].AddSeconds(1)) && (jobs[j].JobEndDate > dateTimes[d + 1].AddSeconds(-1)))
                        {
                            orderedPair.Add(jobs[j].JobID);
                        }
                    }
                    // If there is more than 2 jobs which are overlaping check does they allready exists in collection.
                    if (orderedPair.Count >= 2)
                    {
                        bool include = true;
                        for (int i = jobsWhichAreOverlaping.Count - 1; i > -1; i--)
                        {
                            // If there is same ordered pair in collection set include to false
                            if (orderedPair.All(tmp => jobsWhichAreOverlaping[i].Contains(tmp)))
                            {
                                include = false;
                            }
                            // TODO Filip need to check this condition and document him. 29102020
                            /*
                            else if (jobsWhichAreOverlaping[i].All(tmp => orderedPair.Contains(tmp))) 
                            {
                                jobsWhichAreOverlaping.RemoveAt(i); 
                            }*/
                        }
                        // based on this parameter add solution or ignore hims
                        if (include == true)
                        {
                            jobsWhichAreOverlaping.Add(orderedPair);
                        }
                    }
                }
            }
            return jobsWhichAreOverlaping;
        }

    }
}
