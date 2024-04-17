using eSolver.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace eSolver.BusinessLogic.Utiles
{
    public class JobUtiles
    {
        public static bool DoesOverlappes(IJobBase job1, IJobBase job2)
        {
            return ((job1.JobStartDate >= job2.JobStartDate) && (job1.JobStartDate <= job2.JobEndDate) ||
                    (job2.JobStartDate >= job1.JobStartDate) && (job2.JobStartDate <= job1.JobEndDate));
        }

        public static List<DateTime> GetRangeOfDayOfTheWeeks(IJobBase job)
        {
            List<DateTime> dateTimesOfTheJob = new List<DateTime>();
            for (DateTime date = job.JobStartDate.Date; date <= job.JobEndDate.Date; date = date.AddDays(1))
            {
                dateTimesOfTheJob.Add(date.Date);
            }
            return dateTimesOfTheJob;
        }
    }
}
