using eSolver.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace eSolver.AutoSolverNew.RankingLogic
{
    static partial class Ranking_DefaultJob
    {
        static public void FindForDefaultJob(RankingWrapper rankingWrapper, ScheduleRanking scheduleRanking, RankingRule rankingRule, List<Employee> employees, List<ScheduleJob> jobs, bool reverseOrder, int? sortOrder, List<ScheduleCustomData> scheduleCustomData, List<RankingWrapper> rankingWrappers)
        {
            rankingWrapper.rang = new float[employees.Count, jobs.Count];
            for (int e = 0; e < employees.Count; e++)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    rankingWrapper.rang[e, j] = 0;
                }
            }

            for (int e = 0; e < employees.Count; e++)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    if (rankingRule.RestrictedToJobTypeID == null || rankingRule.RestrictedToJobTypeID == jobs[j].JobTypeID)
                    {
                        if (!reverseOrder && employees[e].JobTypeID == jobs[j].JobTypeID)
                        {
                            //Console.Write("+ ");
                            rankingWrapper.rang[e, j] += (float)Math.Pow(10, -(int)sortOrder) / 2;
                        }
                        else if (reverseOrder && employees[e].JobTypeID == jobs[j].JobTypeID)
                        {
                            rankingWrapper.rang[e, j] -= (float)Math.Pow(10, -(int)sortOrder) / 2;
                        }
                        else
                        {
                            //Console.Write("- ");
                        }
                    }
                }
                //Console.WriteLine();
            }
            rankingWrappers.Add(rankingWrapper);
        }

    }
}
