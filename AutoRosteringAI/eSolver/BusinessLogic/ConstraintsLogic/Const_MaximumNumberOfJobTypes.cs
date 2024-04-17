using System;
using System.Collections.Generic;
using System.Linq;
using eSolver.Entities.Constraints;
using eSolver.Entities;
using Google.OrTools.Sat;
using static eSolver.BusinessLogic.Utiles.Compared;
using eSolver.Entities.OutSideView;
using eSolver.BusinessLogic.Managers;

namespace eSolver.BusinessLogic.ConstraintsLogic
{
    public static class Const_MaximumNumberOfJobTypes
    {


        /// <summary>
        /// 3.1.9  Maximum number of job types
        /// <summary>
        public static void ApplyMaximumNumberOfJobTypes(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, ScheduleActiveConstraint constraint, PayPeriodDTO payPeriodDTO, List<JobFromOutside> jobsFromOutsides, string startDayOfTheWeek)
        {
            DateRangeManager dateRangeManger = new DateRangeManager();
            //int dayOfYear = jobs[0].JobStartDate.DayOfYear;
            //DateTime currStartOfThePeriod = GetStartDateTime(constraint, jobs[0], payPeriodDTO);
            DateTime[] dateRangeTarget = dateRangeManger.GenerateDateRange(constraint.ConstraintMNOJT, jobs[0].JobStartDate, jobs[0].JobEndDate, payPeriodDTO, startDayOfTheWeek);
            List<int> listOfJobsOfSameTypeDay = new List<int>() { };

            //List<DateTime[]> dateRanges = new List<DateTime[]>(jobs.Count);
            DateTime[][] dateRanges = new DateTime[jobs.Count][];
            ConstraintMNOJT constraintMNOJT = constraint.ConstraintMNOJT;

            int lastNonNUllRange = -1;

            for (int j = 0; j < jobs.Count; j++)
            {
                dateRanges[j] = dateRangeManger.GenerateDateRange(constraintMNOJT, jobs[j].JobStartDate, jobs[j].JobEndDate, payPeriodDTO, startDayOfTheWeek);
                if (dateRanges[j] == null)
                {
                    lastNonNUllRange = j;
                }
            }

            lastNonNUllRange++;

            if (constraint.ConstraintMNOJT.ConstraintCustomRangeID == null)
            {
                for (int j = lastNonNUllRange; j < jobs.Count; j++)
                {
                    if (DateTime.Compare(dateRanges[j][0], dateRangeTarget[0]) == 0)
                    {
                        if (jobs[j].JobTypeID == constraintMNOJT.JobTypeID)
                        {
                            listOfJobsOfSameTypeDay.Add(j);
                        }
                    }
                    // Stop if its last
                    if (j == jobs.Count - 1)
                    {
                        AddConstraintOfCurrentDayType(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, (int)constraintMNOJT.MaximalCount, constraint, jobsFromOutsides, dateRangeManger, payPeriodDTO, startDayOfTheWeek);
                        listOfJobsOfSameTypeDay.Clear();
                    }// OR if it's last at period
                    else if ((DateTime.Compare(dateRanges[j + 1][0], dateRangeTarget[0]) != 0))
                    {
                        dateRangeTarget = dateRanges[j + 1];
                        AddConstraintOfCurrentDayType(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, (int)constraintMNOJT.MaximalCount, constraint, jobsFromOutsides, dateRangeManger, payPeriodDTO, startDayOfTheWeek);
                        listOfJobsOfSameTypeDay.Clear();
                    }

                }
            }
            else
            {
                for (int jTarget = lastNonNUllRange; jTarget < jobs.Count; jTarget++)
                {
                    if (jobs[jTarget].JobTypeID == constraintMNOJT.JobTypeID)
                    {
                        listOfJobsOfSameTypeDay.Clear();
                        listOfJobsOfSameTypeDay.Add(jTarget);
                        //Console.WriteLine("{" + jTarget + "}");
                        dateRangeTarget = dateRangeManger.GenerateDateRange(constraintMNOJT, jobs[jTarget].JobStartDate, jobs[jTarget].JobEndDate, payPeriodDTO, startDayOfTheWeek);
                        int earlierTargetJob = jTarget;
                        while (--earlierTargetJob >= lastNonNUllRange && (DateTime.Compare(dateRanges[jTarget][lastNonNUllRange], jobs[earlierTargetJob].JobStartDate) <= 0))
                        {
                            if (jobs[earlierTargetJob].JobTypeID == constraintMNOJT.JobTypeID)
                            {
                                //Console.WriteLine("[" + earlierTargetJob + "]");
                                listOfJobsOfSameTypeDay.Add(earlierTargetJob);
                            }
                        }
                        int laterTargetJob = jTarget;
                        while (++laterTargetJob < jobs.Count() && (DateTime.Compare(dateRanges[jTarget][1], jobs[laterTargetJob].JobStartDate) >= 0))
                        {
                            if (jobs[laterTargetJob].JobTypeID == constraintMNOJT.JobTypeID)
                            {
                                //Console.WriteLine("(" + laterTargetJob + ")");
                                listOfJobsOfSameTypeDay.Add(laterTargetJob);
                            }
                        }

                        AddConstraintOfCurrentDayTypeCustom(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, (int)constraintMNOJT.MaximalCount, constraint, jobsFromOutsides, dateRangeManger, payPeriodDTO, startDayOfTheWeek, jTarget);
                        listOfJobsOfSameTypeDay = new List<int>() { };
                        //Console.WriteLine();
                    }
                }
            }
        }
        /// [3.1.9]  Maximum number of job types
        static void AddConstraintOfCurrentDayType(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, List<int> list, int limit, ScheduleActiveConstraint constraint, List<JobFromOutside> jobsFromOutsides, DateRangeManager dateRangeManger, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek)
        {
            if (list.Count == 0)
            {
                return;
            }

            ConstraintMNOJT constraintMNODOTW = constraint.ConstraintMNOJT;

            for (int emp = 0; emp < employees.Count; emp++)
            {
                List<ILiteral> tmp = new List<ILiteral>();
                //int firstLeaveDate = 0;
                for (int jl = 0; jl < list.Count; jl++)
                {
                    if (IsCompared(constraint, jobs[jl].JobCustomData, employees[emp], jobs[jl]))
                    {
                        tmp.Add(matrix[emp, list[jl]]);
                    }
                }
                /// <example>
                /// Check jobs outside of schedule 
                /// </example>
                int daysOfWeekPutOfSchedule = 0;
                if (jobsFromOutsides != null)
                {
                    foreach (JobFromOutside jobFromOutsides in jobsFromOutsides)
                    {
                        //if (jobsForSolver.JobTypeId == constraint.JobTypeID)
                        {
                            if ((DateTime.Compare(dateRangeManger.GenerateDateRange(constraintMNODOTW, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0], dateRangeManger.GenerateDateRange(constraintMNODOTW, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0]) == 0))
                            {
                                foreach (int empID in jobFromOutsides.EmployeeAssigments)
                                {
                                    if (employees[emp].Id == empID)
                                    {
                                        daysOfWeekPutOfSchedule++;
                                    }
                                }
                            }
                        }
                    }
                }

                if (limit >= daysOfWeekPutOfSchedule)
                {
                    model.Add(LinearExpr.Sum(tmp) <= (limit - daysOfWeekPutOfSchedule));
                }
                else
                {
                    model.Add(LinearExpr.Sum(tmp) == 0);
                }
            }
        }

        static void AddConstraintOfCurrentDayTypeCustom(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, List<int> list, int limit, ScheduleActiveConstraint constraint, List<JobFromOutside> jobsFromOutsides, DateRangeManager dateRangeManger, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek, int targetIndexJob)
        {
            if (list.Count == 0)
            {
                return;
            }

            ConstraintMNOJT constraintMNODOTW = constraint.ConstraintMNOJT;

            for (int emp = 0; emp < employees.Count; emp++)
            {
                List<ILiteral> tmp = new List<ILiteral>();
                //int firstLeaveDate = 0;
                for (int jl = 0; jl < list.Count; jl++)
                {
                    if (IsCompared(constraint, jobs[jl].JobCustomData, employees[emp], jobs[jl]))
                    {
                        tmp.Add(matrix[emp, list[jl]]);
                    }
                }
                int daysOfWeekPutOfSchedule = 0;

                if (jobsFromOutsides != null)
                {
                    foreach (JobFromOutside jobFromOutsides in jobsFromOutsides)
                    {
                        //if (jobsForSolver.JobTypeId == constraint.JobTypeID)
                        {
                            if ((DateTime.Compare(dateRangeManger.GenerateDateRange(constraintMNODOTW, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0], dateRangeManger.GenerateDateRange(constraintMNODOTW, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0]) == 0))
                            {
                                foreach (int empID in jobFromOutsides.EmployeeAssigments)
                                {
                                    if (employees[emp].Id == empID)
                                    {
                                        daysOfWeekPutOfSchedule++;
                                    }
                                }
                            }
                        }
                    }
                }

                if (limit >= daysOfWeekPutOfSchedule)
                {
                    model.Add(LinearExpr.Sum(tmp) <= (limit - daysOfWeekPutOfSchedule)).OnlyEnforceIf(matrix[emp, targetIndexJob]); ;
                }
                else
                {
                    model.Add(LinearExpr.Sum(tmp) == 0);
                }
            }
        }
    }
}