using eSolver.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using eSolver.BusinessLogic.Utiles;
using System.Linq;

namespace eSolver.AutoSolverNew.RankingLogic
{
    static partial class Ranking_Skills
    {
        static public void FindSkillEmployee(RankingWrapper rankingWrapper, ScheduleRanking scheduleRanking, RankingRule rankingRule, List<Employee> employees, List<ScheduleJob> jobs, bool reverseOrder, int? sortOrder, List<ScheduleCustomData> scheduleCustomData, List<RankingWrapper> rankingWrappers, List<JobTypeNonEssentialSkill> nonEssentialSkills, List<SkillMatrix> skillMatrixList)
        {
            rankingWrapper.rang = new float[employees.Count, jobs.Count];
            for (int e = 0; e < employees.Count; e++)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    rankingWrapper.rang[e, j] = 0;
                }
            }

            List<SkillMatrix> employeeSkilllist = new List<SkillMatrix>();

            for (int j = 0; j < jobs.Count; j++)
            {

                for (int e = 0; e < employees.Count; e++)
                {
                    employeeSkilllist = skillMatrixList.Where(x => x.EmployeeID == employees[e].Id).ToList();
                    for (int n = 0; n < nonEssentialSkills.Count; n++)
                    {
                        if (employeeSkilllist.FirstOrDefault(a => a.SkillCodeID == nonEssentialSkills[n].SkillCodeID) != null)
                        {
                            rankingWrapper.rang[e, j] += (float)Math.Pow(10, -(int)sortOrder) / 2;
                        }
                    }
                }
            }

            rankingWrappers.Add(rankingWrapper);
        }

    }
}
