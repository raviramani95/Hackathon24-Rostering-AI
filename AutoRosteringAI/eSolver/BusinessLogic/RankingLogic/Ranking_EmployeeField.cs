using eSolver.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using eSolver.BusinessLogic.Utiles;

namespace eSolver.AutoSolverNew.RankingLogic
{
    static partial class Ranking_EmployeeField
    {

        static public void FindForEmployeeField(RankingWrapper rankingWrapper, ScheduleRanking scheduleRanking, RankingRule rankingRule, List<Employee> employees, List<ScheduleJob> jobs, bool reverseOrder, int? sortOrder, List<ScheduleCustomData> scheduleCustomData, List<RankingWrapper> rankingWrappers)
        {
            rankingWrapper.rang = new float[employees.Count, jobs.Count];
            for (int e = 0; e < employees.Count; e++)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    rankingWrapper.rang[e, j] = 0;
                }
            }

            List<Employee> sortedEmployees = Ranking.SortEmployeesByEmployeeField(rankingRule.EmployeeField, true, employees);
            Dictionary<int, float> keyValues = new Dictionary<int, float>();

            for (int se = 0; se < sortedEmployees.Count; se++)
            {
                for (int e = 0; e < employees.Count; e++)
                {
                    if (sortedEmployees[se].Id == employees[e].Id)
                    {
                        keyValues.Add(e, (float)se / (float)employees.Count);
                        break;
                    }
                }
            }

            if (!(string.IsNullOrEmpty(rankingRule.ComparisonMode)))
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    if (rankingRule.RestrictedToJobTypeID == null)
                    {
                        for (int e = 0; e < employees.Count; e++)
                        {
                            if (Ranking.IsCompared(rankingRule, scheduleCustomData, employees[e], jobs[j]))
                            {
                                rankingWrapper.rang[e, j] += (float)1.0 / (float)employees.Count;
                            }
                        }
                    }
                    else if (rankingRule.RestrictedToJobTypeID == jobs[j].JobTypeID)
                    {
                        for (int e = 0; e < employees.Count; e++)
                        {
                            if (Ranking.IsCompared(rankingRule, scheduleCustomData, employees[e], jobs[j]))
                            {
                                rankingWrapper.rang[e, j] += (float)1.0 / (float)employees.Count;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    if (rankingRule.RestrictedToJobTypeID == null || rankingRule.RestrictedToJobTypeID == jobs[j].JobTypeID)
                    {
                        for (int e = 0; e < employees.Count; e++)
                        {
                            if (!reverseOrder)
                            {
                                rankingWrapper.rang[e, j] += (float)Math.Pow(10, -(int)sortOrder) / 2 + keyValues[e]; ;
                            }
                            else if (reverseOrder)
                            {
                                rankingWrapper.rang[e, j] -= (float)Math.Pow(10, -(int)sortOrder) / 2 + keyValues[e]; ;
                            }
                        }
                    }
                }
            }

            rankingWrappers.Add(rankingWrapper);
        }

    }
}
