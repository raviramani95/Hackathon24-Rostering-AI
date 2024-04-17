using eSolver.BusinessLogic.Managers;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using System;
using System.Collections.Generic;
using static eSolver.AutoSolverNew.RankingLogic.Comparison;
using static eSolver.AutoSolverNew.RankingLogic.Utiles;

namespace eSolver.AutoSolverNew.RankingLogic
{
    static partial class Ranking_WeekFreq
    {
        static public void FindForWeekFreq(RankingWrapper rankingWrapper, ScheduleRanking scheduleRanking, RankingRule rankingRule, List<ScheduleCustomData> scheduleCustomData, List<RankingWrapper> rankingWrappers, List<Employee> employees, List<ScheduleJob> jobs, List<Tuple<int, JobFromOutside>> assignedEmployeeJob, IDictionary<int, int> dict, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek)
        {
            DateRangeManager dateRangeManger;
            Condition condition;
            double[] EmployeeDayOfWeek;
            rankingWrapper.conditions = new List<Condition>();
            dateRangeManger = new DateRangeManager();
            List<int>[] listOfJobsOfSameTypeDay;
            AlocateRankingWrapper(employees, out condition, out EmployeeDayOfWeek, out listOfJobsOfSameTypeDay);

            if (rankingRule.DateRange.ToLower() != "custom")
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    DateTime[] dateRange = dateRangeManger.GenerateDateRange(rankingRule, jobs[j].JobStartDate, payPeriodDTO, startDayOfTheWeek);
                    if (dateRange != null)
                    {
                        if (CheckIfJobIsInDateRange(jobs[j], dateRange))
                        {
                            for (int e = 0; e < employees.Count; e++)
                            {
                                if(IsCompared(rankingRule, jobs[j].JobCustomData, employees[e], jobs[j]))
                                {
                                    AddInListIfDayHasDayOfWeek(jobs, (DayOfWeek)Enum.Parse(typeof(DayOfWeek), rankingRule.DayOfWeek), listOfJobsOfSameTypeDay, rankingRule, j, e);
                                }
                            }
                        }

                        if (IsPerodEnded(rankingRule, jobs, payPeriodDTO, startDayOfTheWeek, dateRangeManger, j, dateRange))
                        {
                            List<DateTime> daysAtSpecificDayOfWeek = FindAllDaysOnDayOfTheWeek(dateRange, (DayOfWeek)Enum.Parse(typeof(DayOfWeek), rankingRule.DayOfWeek));
                            PopulateCondtion(listOfJobsOfSameTypeDay, rankingRule, jobs, condition, daysAtSpecificDayOfWeek);
                            CheckJobsOutside(daysAtSpecificDayOfWeek, assignedEmployeeJob, condition, rankingRule, employees);
                            condition.ListOfThaeDaysOfSepcificWeek = daysAtSpecificDayOfWeek;
                            rankingWrapper.conditions.Add(condition);
                            AlocateRankingWrapper(employees, out condition, out EmployeeDayOfWeek, out listOfJobsOfSameTypeDay);
                        }
                    }
                }
            }
            rankingWrapper.isDynamic = true;
            rankingWrappers.Add(rankingWrapper);
        }

        private static void CheckJobsOutside(List<DateTime> daysAtSpecificDayOfWeek, List<Tuple<int, JobFromOutside>> assignedEmployeeJob, Condition condition, RankingRule rankingRule, List<Employee> employees)
        {
            if (assignedEmployeeJob.Count > 0)
            {
                List<int> employeeWorksOnThatDayOfTheWeek;
                for (int day = 0; day < daysAtSpecificDayOfWeek.Count; day++)
                {
                    employeeWorksOnThatDayOfTheWeek = new List<int>();
                    foreach (Tuple<int, JobFromOutside> tuple in assignedEmployeeJob)
                    {
                        if (tuple.Item2.JobEndDate >= daysAtSpecificDayOfWeek[day] && tuple.Item2.JobStartDate <= daysAtSpecificDayOfWeek[day].AddDays(1))
                        {
                            if (IsCompared(rankingRule, tuple.Item2.JobCustomData, employees[tuple.Item1], tuple.Item2))
                            {
                                if (condition.clasuses[tuple.Item1].jobsPerDayOfTheWeek[day].Count > 0)
                                {
                                    condition.clasuses[tuple.Item1].jobsPerDayOfTheWeek[day] = new List<int[,]>();
                                }

                                if (!employeeWorksOnThatDayOfTheWeek.Contains(tuple.Item1))
                                {
                                    employeeWorksOnThatDayOfTheWeek.Add(tuple.Item1);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < employeeWorksOnThatDayOfTheWeek.Count; i++)
                    {
                        condition.clasuses[employeeWorksOnThatDayOfTheWeek[i]].externalAmount += 1;
                    }
                }
            }
        }

        /// <summary>
        /// Separate days within days from where they belong
        /// </summary>
        /// <param name="listOfJobsOfSameTypeDay"></param>
        /// <param name="rankingRule"></param>
        /// <param name="jobs"></param>
        /// <param name="condition"></param>
        /// <param name="daysAtSpecificDayOfWeek"></param>
        private static void PopulateCondtion(List<int>[] listOfJobsOfSameTypeDay, RankingRule rankingRule, List<ScheduleJob> jobs, Condition condition, List<DateTime> daysAtSpecificDayOfWeek)
        {
            for (int emp = 0; emp < listOfJobsOfSameTypeDay.Length; emp++)
            {
                condition.clasuses[emp].jobsPerDayOfTheWeek = new List<List<int[,]>>();
                for (int i = 0; i < daysAtSpecificDayOfWeek.Count; i++)
                {
                    condition.clasuses[emp].jobsPerDayOfTheWeek.Add(new List<int[,]>());
                }

                for (int j = 0; j < listOfJobsOfSameTypeDay[emp].Count; j++)
                {
                    int dayWhereJobBelong = 0;
                    bool dayStartedAfterBeforeThenEnd = jobs[listOfJobsOfSameTypeDay[emp][j]].JobStartDate < daysAtSpecificDayOfWeek[dayWhereJobBelong].AddDays(1);
                    bool dayEndedAfterBegining = jobs[listOfJobsOfSameTypeDay[emp][j]].JobEndDate > daysAtSpecificDayOfWeek[dayWhereJobBelong];
                    while (!(dayEndedAfterBegining && dayStartedAfterBeforeThenEnd))
                    {
                        dayWhereJobBelong++;
                        if (dayWhereJobBelong < daysAtSpecificDayOfWeek.Count)
                        {
                            dayStartedAfterBeforeThenEnd = jobs[listOfJobsOfSameTypeDay[emp][j]].JobStartDate < daysAtSpecificDayOfWeek[dayWhereJobBelong].AddDays(1);
                            dayEndedAfterBegining = jobs[listOfJobsOfSameTypeDay[emp][j]].JobEndDate > daysAtSpecificDayOfWeek[dayWhereJobBelong];
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (dayWhereJobBelong < daysAtSpecificDayOfWeek.Count)
                    {
                        condition.clasuses[emp].jobsPerDayOfTheWeek[dayWhereJobBelong].Add(new int[1, 2] { { emp, listOfJobsOfSameTypeDay[emp][j] } });
                    }
                }
            }
        }
        private static List<DateTime> FindAllDaysOnDayOfTheWeek(DateTime[] dateRange, DayOfWeek dayOfWeek)
        {
            List<DateTime> daysAtSpecificDayOfWeek = new List<DateTime>();
            int daysFromBeginningPeriodToDayOfTheWeek = (((int)DayOfWeek.Monday - (int)dateRange[0].DayOfWeek + 7) % 7) + (int)dayOfWeek - 1;

            while(dateRange[0].AddDays(daysFromBeginningPeriodToDayOfTheWeek) < dateRange[1])
            {
                daysAtSpecificDayOfWeek.Add(dateRange[0].AddDays(daysFromBeginningPeriodToDayOfTheWeek).Date);
                daysFromBeginningPeriodToDayOfTheWeek += 7;
            }

            return daysAtSpecificDayOfWeek;
        }

        private static bool IsPerodEnded(RankingRule rankingRule, List<ScheduleJob> jobs, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek, DateRangeManager dateRangeManger, int j, DateTime[] dateRange)
        {
            return IsLastJob(jobs, j) || IsLastJobOfThePeriod(rankingRule, jobs, payPeriodDTO, startDayOfTheWeek, dateRangeManger, j) || IsLastJobBeforeNextPeriod(rankingRule, jobs, payPeriodDTO, startDayOfTheWeek, dateRangeManger, j, dateRange);
        }

        private static bool IsLastJobBeforeNextPeriod(RankingRule rankingRule, List<ScheduleJob> jobs, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek, DateRangeManager dateRangeManger, int j, DateTime[] dateRange)
        {
            return (!dateRangeManger.CompareDate(dateRange[0], dateRangeManger.GenerateDateRange(rankingRule, jobs[j + 1].JobStartDate, payPeriodDTO, startDayOfTheWeek)[0]));
        }

        private static bool IsLastJobOfThePeriod(RankingRule rankingRule, List<ScheduleJob> jobs, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek, DateRangeManager dateRangeManger, int j)
        {
            return dateRangeManger.GenerateDateRange(rankingRule, jobs[j + 1].JobStartDate, payPeriodDTO, startDayOfTheWeek) == null;
        }

        private static bool IsLastJob(List<ScheduleJob> jobs, int j)
        {
            return j == jobs.Count - 1;
        }

        /// <summary>
        /// Alocate memory of the one condition
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="condition"></param>
        /// <param name="EmployeeDayOfWeek"></param>
        private static void AlocateRankingWrapper(List<Employee> employees, out Condition condition, out double[] EmployeeDayOfWeek, out List<int>[] listOfJobsOfSameTypeDay)
        {
            listOfJobsOfSameTypeDay = new List<int>[employees.Count];
            for (int i = 0; i < listOfJobsOfSameTypeDay.Length; i++)
            {
                listOfJobsOfSameTypeDay[i] = new List<int>();
            }

            condition = new Condition();
            condition.clasuses = new List<Clasuses>(employees.Count);

            EmployeeDayOfWeek = new double[employees.Count];
            for (int i = 0; i < employees.Count; i++)
            {
                condition.clasuses.Add(new Clasuses());
                condition.clasuses[i].calsusa = new List<int[,]>();
            }
        }
        private static void AddInListIfDayHasDayOfWeek(List<ScheduleJob> jobs, DayOfWeek dayOfWeek, List<int>[] listOfJobsOfSameTypeDay, RankingRule rankingRule, int j, int e)
        {
            if ((bool)rankingRule.CountOvernights)
            {

                DateTime endDay = jobs[j].JobEndDate;
                if (endDay.DayOfWeek == dayOfWeek)
                {
                    listOfJobsOfSameTypeDay[e].Add(j);
                }
                else
                {
                    DateTime startDay = jobs[j].JobStartDate;
                    while (startDay < endDay)
                    {
                        if ((startDay.DayOfWeek == dayOfWeek))
                        {
                            listOfJobsOfSameTypeDay[e].Add(j);
                        }
                        startDay = startDay.AddDays(1);
                    }
                }
            }
            else
            {
                if (jobs[j].JobStartDate.DayOfWeek == dayOfWeek)
                {
                    listOfJobsOfSameTypeDay[e].Add(j);
                }
            }
        }
    }
}
