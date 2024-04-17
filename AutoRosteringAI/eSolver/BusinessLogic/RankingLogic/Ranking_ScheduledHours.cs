using eSolver.BusinessLogic.Utiles;
using eSolver.BusinessLogic.Managers;
using eSolver.Entities;
using eSolver.Entities.Interfaces;
using eSolver.Entities.OutSideView;
using System;
using System.Collections.Generic;
using System.Text;
using static eSolver.AutoSolverNew.RankingLogic.Comparison;
using static eSolver.AutoSolverNew.RankingLogic.Utiles;

namespace eSolver.AutoSolverNew.RankingLogic
{
    static partial class Ranking_ScheduledHours
    {
        private static long CalculateTime<T>(T scheduleJob, DateTime[] dateTime) where T : IJobBase
        {
            long totalMinutes = 0;
            if (scheduleJob.JobStartDate.Date != scheduleJob.JobEndDate.Date)
            {
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(0) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(0) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(0) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT1.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(1) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(1) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(1) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT2.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(2) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(2) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(2) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT3.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(3) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(3) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(3) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT4.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(4) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(4) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(4) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT5.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(5) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(5) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(5) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT6.TotalMinutes : 0; 

                return totalMinutes;
            }
            else
            {
                return ((scheduleJob.JobStartDate >= dateTime[0]) && (scheduleJob.JobEndDate.Date <= dateTime[1].Date)) ? (long)scheduleJob.HoursT.TotalMinutes : 0;
            }
        }

        static public void FindForScheduledHours(RankingWrapper rankingWrapper, ScheduleRanking scheduleRanking, RankingRule rankingRule, int[,] availabilityMatrix, List<ScheduleCustomData> scheduleCustomData, List<RankingWrapper> rankingWrappers, List<Employee> employees, List<ScheduleJob> jobs, List<Tuple<int, JobFromOutside>> assignedEmployeeJob, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek)
        {
            //rankingWrapper = new RankingWrapper();
            rankingWrapper.conditions = new List<Condition>();

            DateRangeManager dateRangeManger = new DateRangeManager();
            Condition condition = new Condition();
            condition.clasuses = new List<Clasuses>(employees.Count);

            double[] EmployeeHours = new double[employees.Count];

            for (int i = 0; i < employees.Count; i++)
            {
                condition.clasuses.Add(new Clasuses());
                condition.clasuses[i].calsusa = new List<int[,]>();
                condition.clasuses[i].calsusaHours = new List<long>();
            }


            for (int j = 0; j < jobs.Count; j++)
            {
                DateTime[] dateRange = dateRangeManger.GenerateDateRange(rankingRule, jobs[j].JobStartDate, payPeriodDTO, startDayOfTheWeek);

                if (dateRange != null && CheckIfJobIsInDateRange(jobs[j], dateRange))
                {
                    for (int e = 0; e < employees.Count; e++)
                    {
                        //if (availabilityMatrix[e, j] == 0 && IsCompared(rankingRule, customData, employees[e], jobs[j]))
                        if (availabilityMatrix[e, j] >= -1 && IsCompared(rankingRule, null, employees[e], jobs[j]))
                        {
                            condition.clasuses[e].calsusa.Add(new int[1, 2] { { e, j } });
                            condition.clasuses[e].calsusaHours.Add(CalculateTime(jobs[j], dateRange));
                        }
                    }
                }

                if (dateRange != null)
                {
                    if (j == jobs.Count - 1 || dateRangeManger.GenerateDateRange(rankingRule, jobs[j + 1].JobStartDate, payPeriodDTO, startDayOfTheWeek) == null || (!dateRangeManger.CompareDate(dateRange[0], dateRangeManger.GenerateDateRange(rankingRule, jobs[j + 1].JobStartDate, payPeriodDTO, startDayOfTheWeek)[0])))
                    {
                        if (assignedEmployeeJob.Count > 0)
                        {
                            
                            foreach (Tuple<int, JobFromOutside> tuple in assignedEmployeeJob)
                            {
                                if (dateRangeManger.GenerateDateRange(rankingRule, tuple.Item2.JobStartDate, payPeriodDTO, startDayOfTheWeek) != null && (DateTime.Compare(dateRange[0], dateRangeManger.GenerateDateRange(rankingRule, tuple.Item2.JobStartDate, payPeriodDTO, startDayOfTheWeek)[0]) == 0))
                                {
                                    EmployeeHours[tuple.Item1] += CalculateTime(tuple.Item2, dateRange);
                                }
                            }
                        }

                        for (int i = 0; i < employees.Count; i++)
                        {
                            condition.clasuses[i].externalAmount = EmployeeHours[i];
                        }

                        rankingWrapper.conditions.Add(condition);
                        condition = new Condition();
                        condition.clasuses = new List<Clasuses>(employees.Count);

                        for (int i = 0; i < employees.Count; i++)
                        {
                            condition.clasuses.Add(new Clasuses());
                            condition.clasuses[i].calsusa = new List<int[,]>();
                            condition.clasuses[i].calsusaHours = new List<long>();
                        }

                        EmployeeHours = new double[employees.Count];
                    }
                }
            }
 
            rankingWrapper.isDynamic = true;
            rankingWrappers.Add(rankingWrapper);
        }
    }
}
