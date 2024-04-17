using eSolver.BusinessLogic.Managers;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using System;
using System.Collections.Generic;
using System.Text;
using static eSolver.AutoSolverNew.RankingLogic.Utiles;

namespace eSolver.AutoSolverNew.RankingLogic
{
    static partial class Ranking_JobFreq
    {
        static public void FindForJobFreq(ScheduleRanking scheduleRanking, RankingRule rankingRule, List<long> allJobTypes, List<ScheduleCustomData> scheduleCustomData, List<RankingWrapper> rankingWrappers, List<Employee> employees, List<ScheduleJob> jobs, List<Tuple<int, JobFromOutside>> assignedEmployeeJob, IDictionary<int, int> dict, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek)
        {

            //IDictionary<int, int> dict = CreateDictionaryForJobTypes();
            for (int jobType = 0; jobType < allJobTypes.Count; jobType++)
            {

                RankingWrapper rankingWrapper = new RankingWrapper();
                rankingWrapper.conditions = new List<Condition>();
                rankingWrapper.rankingName = "Job Freq";
                rankingWrapper.jobTypeID = allJobTypes[jobType];
                rankingWrapper.sortOrder = (int)scheduleRanking.SortOrder;
                rankingWrapper.isDynamic = true;

                DateRangeManager dateRangeManger = new DateRangeManager();
                Condition condition = new Condition();
                condition.clasuses = new List<Clasuses>(employees.Count);

                int[,] EmployeesOnJobType = new int[employees.Count, dict.Count];

                for (int i = 0; i < employees.Count; i++)
                {
                    condition.clasuses.Add(new Clasuses());
                    condition.clasuses[i].calsusa = new List<int[,]>();
                    condition.clasuses[i].externalJobTypes = new Dictionary<long, long>();
                }

                if (rankingRule.DateRange.ToLower() != "custom")
                {
                    for (int j = 0; j < jobs.Count; j++)
                    {
                        DateTime[] dateRange = dateRangeManger.GenerateDateRange(rankingRule, jobs[j].JobStartDate, payPeriodDTO, startDayOfTheWeek);


                        if (dateRange != null && CheckIfJobIsInDateRange(jobs[j], dateRange)) // && (rankingRule.RestrictedToJobTypeID == jobs[j].JobtypeID)
                        {
                            //if(condition.clasuses[e] == null)
                            if (jobs[j].JobTypeID == allJobTypes[jobType])
                            {
                                for (int e = 0; e < employees.Count; e++)
                                {
                                    condition.clasuses[e].calsusa.Add(new int[1, 2] { { e, j } }); // MOre caluses for more jobtypes???
                                    condition.clasuses[e].externalJobTypes = new Dictionary<long, long>();
                                }
                            }
                        }

                        if (dateRange != null)
                        {
                            if (j == jobs.Count - 1 || dateRangeManger.GenerateDateRange(rankingRule, jobs[j + 1].JobStartDate, payPeriodDTO, startDayOfTheWeek) == null || (!dateRangeManger.CompareDate(dateRange[0], dateRangeManger.GenerateDateRange(rankingRule, jobs[j + 1].JobStartDate, payPeriodDTO, startDayOfTheWeek)[0])))
                            {
                                if (assignedEmployeeJob.Count > 0)
                                {
                                    for (int i = 0; i < assignedEmployeeJob.Count; i++)
                                    {
                                        //if (assignedEmployeeJob[i].Item2.JobTypeID == jobType) Don't delete this line yet
                                        {
                                            if (dateRangeManger.GenerateDateRange(rankingRule, assignedEmployeeJob[i].Item2.JobStartDate, payPeriodDTO, startDayOfTheWeek) != null && (DateTime.Compare(dateRange[0], dateRangeManger.GenerateDateRange(rankingRule, assignedEmployeeJob[i].Item2.JobStartDate, payPeriodDTO, startDayOfTheWeek)[0]) == 0))
                                            {
                                                EmployeesOnJobType[assignedEmployeeJob[i].Item1, dict[(int)assignedEmployeeJob[i].Item2.JobTypeID]] += 1;
                                            }
                                        }
                                    }
                                }
                                

                                for (int i = 0; i < employees.Count; i++)
                                {
                                    for (int f = 0; f < dict.Count; f++)
                                    {
                                        condition.clasuses[i].externalJobTypes.Add((int)allJobTypes[f], EmployeesOnJobType[i, f]);//EmployeesOnJobType[i, f]);  // List of external amounts
                                    }
                                }
   
                                rankingWrapper.conditions.Add(condition);
                                condition = new Condition();
                                condition.clasuses = new List<Clasuses>(employees.Count);

                                for (int i = 0; i < employees.Count; i++)
                                {
                                    condition.clasuses.Add(new Clasuses());
                                    condition.clasuses[i].calsusa = new List<int[,]>();
                                    condition.clasuses[i].externalJobTypes = new Dictionary<long, long>();
                                }

                                //EmployeeJobTzpes = new double[employees.Count];
                                EmployeesOnJobType = new int[employees.Count, dict.Count];
                            }
                        }
                    }
                }
                rankingWrapper.isDynamic = true;
                rankingWrappers.Add(rankingWrapper);
            }
        }

    }
}
