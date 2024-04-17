using System;
using System.Collections.Generic;
using System.Linq;
using eSolver.Entities.Constraints;
using eSolver.Entities;
using Google.OrTools.Sat;
using static eSolver.BusinessLogic.Utiles.Compared;
using eSolver.Entities.OutSideView;
using eSolver.BusinessLogic.Managers;
using eSolver.Entities.Interfaces;

namespace eSolver.BusinessLogic.ConstraintsLogic
{
    public class Const_MaximumNumberOfJobTimes
    {
        public static void ApplyMaximumNumberOfJobTimes(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, ScheduleActiveConstraint constraint, PayPeriodDTO payPeriodDTO, List<JobFromOutside> jobFromOutsides, string startDayOfTheWeek)
        {
            DateRangeManager dateRangeManger = new DateRangeManager();
            DateTime[] dateRangeTarget = dateRangeManger.GenerateDateRange(constraint.ConstraintMNOJTI, jobs[0].JobStartDate, jobs[0].JobEndDate, payPeriodDTO, startDayOfTheWeek);
            List<int> listOfJobsOfSameTypeDay = new List<int>() { };

            DateTime[][] dateRanges = new DateTime[jobs.Count][];
            ConstraintMNOJTI constraintMNOJTI = constraint.ConstraintMNOJTI;

            int lastNonNUllRange = -1;

            for (int j = 0; j < jobs.Count; j++)
            {
                dateRanges[j] = dateRangeManger.GenerateDateRange(constraintMNOJTI, jobs[j].JobStartDate, jobs[j].JobEndDate, payPeriodDTO, startDayOfTheWeek);
                if (dateRanges[j] == null)
                {
                    lastNonNUllRange = j;
                }
            }

            lastNonNUllRange++;

            if (constraintMNOJTI.ConstraintCustomRangeID == null)
            {
                for (int j = lastNonNUllRange; j < jobs.Count; j++)
                {
                    if (DateTime.Compare(dateRanges[j][0], dateRangeTarget[0]) == 0)
                    {
                        if (JobSatisfyConstraint(jobs[j], constraintMNOJTI))
                        {
                            listOfJobsOfSameTypeDay.Add(j);
                        }
                    }
                    // Stop if its last
                    if (j == jobs.Count - 1)
                    {
                        AddConstraintOfCurrentDayType(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, (int)constraintMNOJTI.MaxCount, constraint, jobFromOutsides, dateRangeManger, payPeriodDTO, startDayOfTheWeek);
                        listOfJobsOfSameTypeDay.Clear();
                    }// OR if it's last at period
                    else if ((DateTime.Compare(dateRanges[j + 1][0], dateRangeTarget[0]) != 0))
                    {
                        dateRangeTarget = dateRanges[j + 1];
                        AddConstraintOfCurrentDayType(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, (int)constraintMNOJTI.MaxCount, constraint, jobFromOutsides, dateRangeManger, payPeriodDTO, startDayOfTheWeek);
                        listOfJobsOfSameTypeDay.Clear();
                    }

                }
            }
            else
            {
                for (int jTarget = lastNonNUllRange; jTarget < jobs.Count; jTarget++)
                {
                    if ((JobSatisfyConstraint(jobs[jTarget], constraintMNOJTI)))
                    {
                        listOfJobsOfSameTypeDay.Clear();
                        listOfJobsOfSameTypeDay.Add(jTarget);
                        dateRangeTarget = dateRangeManger.GenerateDateRange(constraintMNOJTI, jobs[jTarget].JobStartDate, jobs[jTarget].JobEndDate, payPeriodDTO, startDayOfTheWeek);
                        int earlierTargetJob = jTarget;
                        while (--earlierTargetJob >= lastNonNUllRange && (DateTime.Compare(dateRanges[jTarget][lastNonNUllRange], jobs[earlierTargetJob].JobStartDate) <= 0))
                        {
                            if (JobSatisfyConstraint(jobs[earlierTargetJob], constraintMNOJTI))
                            {
                                listOfJobsOfSameTypeDay.Add(earlierTargetJob);
                            }
                        }
                        int laterTargetJob = jTarget;
                        while (++laterTargetJob < jobs.Count() && (DateTime.Compare(dateRanges[jTarget][1], jobs[laterTargetJob].JobStartDate) >= 0))
                        {
                            if (JobSatisfyConstraint(jobs[laterTargetJob], constraintMNOJTI))
                            {
                                listOfJobsOfSameTypeDay.Add(laterTargetJob);
                            }
                        }

                        AddConstraintOfCurrentDayTypeCustom(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, (int)constraintMNOJTI.MaxCount, constraint, jobFromOutsides, dateRangeManger, payPeriodDTO, startDayOfTheWeek, jTarget);
                        listOfJobsOfSameTypeDay = new List<int>() { };
                    }
                }
            }
        }

        private static bool JobSatisfyConstraint<T>(T scheduleJob, ConstraintMNOJTI constraintMNOJTI) where T : IJobBase
        {
            if (constraintMNOJTI.JobEndFrom == null && constraintMNOJTI.JobEndTo == null)
            {
                return (TimeSpan.Compare(new TimeSpan((long)constraintMNOJTI.JobStartFrom), scheduleJob.JobStartDate.TimeOfDay) <= 0) && (TimeSpan.Compare(new TimeSpan((long)constraintMNOJTI.JobStartTo), scheduleJob.JobStartDate.TimeOfDay) >= 0);
            }

            if (constraintMNOJTI.JobStartFrom == null && constraintMNOJTI.JobStartTo == null)
            {
                return (TimeSpan.Compare(new TimeSpan((long)constraintMNOJTI.JobEndFrom), scheduleJob.JobEndDate.TimeOfDay) <= 0) && (TimeSpan.Compare(new TimeSpan((long)constraintMNOJTI.JobEndTo), scheduleJob.JobEndDate.TimeOfDay) >= 0);
            }

            return (TimeSpan.Compare(new TimeSpan((long)constraintMNOJTI.JobStartFrom), scheduleJob.JobStartDate.TimeOfDay) <= 0) && (TimeSpan.Compare(new TimeSpan((long)constraintMNOJTI.JobStartTo), scheduleJob.JobStartDate.TimeOfDay) >= 0) &&
                (TimeSpan.Compare(new TimeSpan((long)constraintMNOJTI.JobEndFrom), scheduleJob.JobEndDate.TimeOfDay) <= 0) && (TimeSpan.Compare(new TimeSpan((long)constraintMNOJTI.JobEndTo), scheduleJob.JobEndDate.TimeOfDay) >= 0);

        }

        /// [3.1.9]  Maximum number of job types
        static void AddConstraintOfCurrentDayType(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, List<int> list, int limit, ScheduleActiveConstraint constraint, List<JobFromOutside> jobsFromOutsides, DateRangeManager dateRangeManger, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek)
        {
            if (list.Count == 0)
            {
                return;
            }

            ConstraintMNOJTI constraintMNOJTI = constraint.ConstraintMNOJTI;

            for (int emp = 0; emp < employees.Count; emp++)
            {
                List<ILiteral> tmp = new List<ILiteral>();
                //int firstLeaveDate = 0;
                for (int jl = 0; jl < list.Count; jl++)
                {
                    if (IsCompared(constraint, jobs[jl].JobCustomData, employees[emp], jobs[jl]))
                    {
                        tmp.Add(matrix[emp, list[jl]]);
                        //tmp[firstLeaveDate] = matrix[emp, list[jl]];
                        //firstLeaveDate++;
                    }
                }
                /// <example>
                /// Check jobs outside of schedule 
                /// </example>
                int daysOfWeekPutOfSchedule = 0;
                if (jobsFromOutsides != null)
                {
                    foreach (JobFromOutside jobFromOutside in jobsFromOutsides)
                    {
                        if (JobSatisfyConstraint(jobFromOutside, constraintMNOJTI))
                        {
                            if ((DateTime.Compare(dateRangeManger.GenerateDateRange(constraintMNOJTI, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0], dateRangeManger.GenerateDateRange(constraintMNOJTI, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0]) == 0))
                            {
                                foreach (int empID in jobFromOutside.EmployeeAssigments)
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

            ConstraintMNOJTI constraintMNOJTI = constraint.ConstraintMNOJTI;

            for (int emp = 0; emp < employees.Count; emp++)
            {
                List<ILiteral> tmp = new List<ILiteral>();
                //int firstLeaveDate = 0;
                for (int jl = 0; jl < list.Count; jl++)
                {
                    if (IsCompared(constraint, jobs[jl].JobCustomData, employees[emp], jobs[jl]))
                    {
                        tmp.Add(matrix[emp, list[jl]]);
                        //tmp[firstLeaveDate] = matrix[emp, list[jl]];
                        //firstLeaveDate++;
                    }
                }
                int daysOfWeekPutOfSchedule = 0;

                if (jobsFromOutsides != null)
                {
                    foreach (JobFromOutside jobFromOutside in jobsFromOutsides)
                    {
                        if (JobSatisfyConstraint(jobFromOutside, constraintMNOJTI))
                        {
                            if ((DateTime.Compare(dateRangeManger.GenerateDateRange(constraintMNOJTI, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0], dateRangeManger.GenerateDateRange(constraintMNOJTI, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0]) == 0))
                            {
                                foreach (int empID in jobFromOutside.EmployeeAssigments)
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
                    model.Add(LinearExpr.Sum(tmp) <= (limit - daysOfWeekPutOfSchedule)).OnlyEnforceIf(matrix[emp, targetIndexJob]);
                }
                else
                {
                    model.Add(LinearExpr.Sum(tmp) == 0);
                }
            }
        }
    }
}
