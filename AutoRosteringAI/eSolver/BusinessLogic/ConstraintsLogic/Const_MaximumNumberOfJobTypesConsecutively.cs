using System;
using System.Collections.Generic;
using eSolver.BusinessLogic.Managers;
using eSolver.Entities;
using eSolver.Entities.Constraints;
using eSolver.Entities.OutSideView;
using Google.OrTools.Sat;
using static eSolver.BusinessLogic.Utiles.Compared;

namespace eSolver.BusinessLogic.ConstraintsLogic
{
    public static class Const_MaximumNumberOfJobTypesConsecutively
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix">Matrix with zeros and ones. Which represents where Employee can or can not work.</param>
        /// <param name="model">Google-OR Model with constraints and rankings</param>
        /// <param name="employees">Collection of all employees</param>
        /// <param name="jobs">All jobs which solver should assign</param>
        /// <param name="constraint">Maximum number of job types cons Constraint</param>
        /// <param name="payPeriodDTO">Pay Period range</param>
        /// <param name="jobsFromOutsides">Jobs which have impact on jobs which will be solved</param>
        /// <param name="startDayOfTheWeek">Name of first day of the week</param>
        public static void ApplyMaximumNumberOfJobsConsecutively(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, ConstraintMNOJTC constraintMNOJTC, List<JobFromOutside> jobsFromOutsides)
        {
            // It is needed later when it will be calculated position of day in array intervalDays, using job's start date
            DateTime startDateTimeInterval = jobs[0].JobStartDate.Date.AddDays(-constraintMNOJTC.MaximalCount - constraintMNOJTC.RestDays + 1);
            DateTime endDateTimeInterval = jobs[jobs.Count - 1].JobEndDate.Date.AddDays(constraintMNOJTC.MaximalCount + constraintMNOJTC.RestDays - 1);

            // 0 - does not work on that job
            // 1 - does work on that job
            // 2 - does work on that job type
            int[,] employeeWorksOnJobFromOtherSchedule = FIndAlassigmentsFromOtherSchedules(employees, jobs, constraintMNOJTC, jobsFromOutsides, startDateTimeInterval, endDateTimeInterval);
            int[,] employeeWorksOnJobFromOtherScheduleREST = FIndAlassigmentsFromOtherSchedulesREST(employees, jobs, constraintMNOJTC, jobsFromOutsides, startDateTimeInterval, endDateTimeInterval);

            for (int emp = 0; emp < employees.Count; emp++)
            {
                List<ILiteral> daysOffForAllTypes = new List<ILiteral>();
                List<ILiteral> daysOffForSpecificJobType = new List<ILiteral>();

                // For the REST period
                List<ILiteral> daysOffForAllTypesREST = new List<ILiteral>();
                List<ILiteral> daysOffForSpecificJobTypeREST = new List<ILiteral>();

                // All jobs
                List<ILiteral>[] includingAllJobsOnSpecificDate = new List<ILiteral>[employeeWorksOnJobFromOtherSchedule.GetLength(1)];
                // Only with specific job type
                List<ILiteral>[] includeWithSpecificJobTypeAndDate = new List<ILiteral>[employeeWorksOnJobFromOtherSchedule.GetLength(1)];


                // All jobs for rest days
                List<ILiteral>[] includingAllJobsOnSpecificDateREST = new List<ILiteral>[employeeWorksOnJobFromOtherSchedule.GetLength(1)];
                // Only with specific job type for rest days
                List<ILiteral>[] includeWithSpecificJobTypeAndDateREST = new List<ILiteral>[employeeWorksOnJobFromOtherSchedule.GetLength(1)];

                for (int jwsd = 0; jwsd < includingAllJobsOnSpecificDate.Length; jwsd++)
                {
                    includingAllJobsOnSpecificDate[jwsd] = new List<ILiteral>();
                    includeWithSpecificJobTypeAndDate[jwsd] = new List<ILiteral>();


                    includingAllJobsOnSpecificDateREST[jwsd] = new List<ILiteral>();
                    includeWithSpecificJobTypeAndDateREST[jwsd] = new List<ILiteral>();
                }

                for (int j = 0; j < jobs.Count; j++)
                {
                    if (IsCompared(constraintMNOJTC, employees[emp], jobs[j]))
                    {
                        int partOfJob = 0;
                        Console.WriteLine(jobs[j].JobStartDate.AddDays(partOfJob).Date);
                        while (jobs[j].JobStartDate.AddDays(partOfJob).Date <= jobs[j].JobEndDate.Date)
                        {
                            // Max count
                            if (partOfJob != 1)
                            {
                                includingAllJobsOnSpecificDate[(int)(jobs[j].JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);

                                includeWithSpecificJobTypeAndDate[(int)(jobs[j].JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);

                                if (partOfJob == 2)
                                {
                                    includingAllJobsOnSpecificDate[(int)(jobs[j].JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);
                                    includeWithSpecificJobTypeAndDate[(int)(jobs[j].JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);
                                }
                            }

                            // Rest 
                            includingAllJobsOnSpecificDateREST[(int)(jobs[j].JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);
                            includeWithSpecificJobTypeAndDateREST[(int)(jobs[j].JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);

                            partOfJob++;
                        }
                        Console.WriteLine(partOfJob);
                    }
                }

                for (int j = 0; j < includingAllJobsOnSpecificDate.Length; j++)
                {
                    BoolVar boolVar = new BoolVar(model.Model, "");
                    // All Job Types
                    if (includingAllJobsOnSpecificDate[j].Count > 0)
                    {
                        daysOffForAllTypes.Add(model.NewBoolVar("day" + j));

                        // Employee does not work on specific day, if the sum of all jobs on the following day is 0
                        model.Add(LinearExpr.Sum(includingAllJobsOnSpecificDate[j]) == 0).OnlyEnforceIf(daysOffForAllTypes[daysOffForAllTypes.Count - 1].Not());
                        model.Add(LinearExpr.Sum(includingAllJobsOnSpecificDate[j]) >= 1).OnlyEnforceIf(daysOffForAllTypes[daysOffForAllTypes.Count - 1]);
                    }
                    else
                    {
                        daysOffForAllTypes.Add(boolVar);
                    }

                    // Specific Job Type
                    if (includeWithSpecificJobTypeAndDate[j].Count > 0)
                    {
                        daysOffForSpecificJobType.Add(model.NewBoolVar("day" + j));

                        // Employee does not work on specific day, if the sum of all jobs on the following day is 0
                        model.Add(LinearExpr.Sum(includeWithSpecificJobTypeAndDate[j]) == 0).OnlyEnforceIf(daysOffForSpecificJobType[daysOffForSpecificJobType.Count - 1].Not());
                        model.Add(LinearExpr.Sum(includeWithSpecificJobTypeAndDate[j]) >= 1).OnlyEnforceIf(daysOffForSpecificJobType[daysOffForSpecificJobType.Count - 1]);
                    }
                    else
                    {
                        daysOffForSpecificJobType.Add(boolVar);
                    }


                    // Rest
                    if (includingAllJobsOnSpecificDateREST[j].Count > 0)
                    {
                        daysOffForAllTypesREST.Add(model.NewBoolVar("day" + j));

                        // Employee does not work on specific day, if the sum of all jobs on the following day is 0
                        model.Add(LinearExpr.Sum(includingAllJobsOnSpecificDateREST[j]) == 0).OnlyEnforceIf(daysOffForAllTypesREST[daysOffForAllTypesREST.Count - 1].Not());
                        model.Add(LinearExpr.Sum(includingAllJobsOnSpecificDateREST[j]) >= 1).OnlyEnforceIf(daysOffForAllTypesREST[daysOffForAllTypesREST.Count - 1]);
                    }
                    else
                    {
                        daysOffForAllTypesREST.Add(boolVar);
                    }

                    // Specific Job Type
                    if (includeWithSpecificJobTypeAndDateREST[j].Count > 0)
                    {
                        daysOffForSpecificJobTypeREST.Add(model.NewBoolVar("day" + j));

                        // Employee does not work on specific day, if the sum of all jobs on the following day is 0
                        model.Add(LinearExpr.Sum(includeWithSpecificJobTypeAndDateREST[j]) == 0).OnlyEnforceIf(daysOffForSpecificJobTypeREST[daysOffForSpecificJobTypeREST.Count - 1].Not());
                        model.Add(LinearExpr.Sum(includeWithSpecificJobTypeAndDateREST[j]) >= 1).OnlyEnforceIf(daysOffForSpecificJobTypeREST[daysOffForSpecificJobTypeREST.Count - 1]);
                    }
                    else
                    {
                        daysOffForSpecificJobTypeREST.Add(boolVar);
                    }
                }

                // Add Constraint 
                for (int i = 0; i < includingAllJobsOnSpecificDate.Length - constraintMNOJTC.MaximalCount - constraintMNOJTC.RestDays + 1; i++)
                {
                    var newDay = model.NewBoolVar("constraint MNOJTC day " + i + " " + emp);

                    List<ILiteral> workOnEachDay = new List<ILiteral>();
                    int countDaysOnOtherSchedule = 0;

                    for (int j = 0; j < constraintMNOJTC.MaximalCount; j++)
                    {
                        if (employeeWorksOnJobFromOtherSchedule[emp, i + j] == 2)
                        {
                            countDaysOnOtherSchedule++;
                        }
                        else
                        {
                            workOnEachDay.Add(daysOffForSpecificJobType[i + j]);
                        }
                    }

                    model.Add(LinearExpr.Sum(workOnEachDay) == constraintMNOJTC.MaximalCount - countDaysOnOtherSchedule).OnlyEnforceIf(newDay);
                    model.Add(LinearExpr.Sum(workOnEachDay) < constraintMNOJTC.MaximalCount - countDaysOnOtherSchedule).OnlyEnforceIf(newDay.Not());

                    List<ILiteral> restOnEachDay = new List<ILiteral>();
                    for (int j = constraintMNOJTC.MaximalCount; j < constraintMNOJTC.MaximalCount + constraintMNOJTC.RestDays; j++)
                    {
                        if (employeeWorksOnJobFromOtherScheduleREST[emp, i + j] == 2)
                        {
                            model.Add(newDay == 0);
                        }
                        restOnEachDay.Add(daysOffForSpecificJobTypeREST[i + j]);
                    }
                    model.Add(LinearExpr.Sum(restOnEachDay) == 0).OnlyEnforceIf(newDay);
                }
            }

        }

        public static void ApplyMaximumNumberOfJobTypesConsecutively(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, ConstraintMNOJTC constraintMNOJTC, PayPeriodDTO payPeriodDTO, List<JobFromOutside> jobsFromOutsides, string startDayOfTheWeek)
        {
            if (jobs.Exists(x => x.JobTypeID == constraintMNOJTC.JobTypeID))
            {
                ApplyMaximumNumberOfJobsConsecutively(matrix, model, employees, jobs, constraintMNOJTC, jobsFromOutsides);
            }
            else
            {
                // It is needed later when it will be calculated position of day in array intervalDays, using job's start date
                DateTime startDateTimeInterval = jobs[0].JobStartDate.Date.AddDays(-constraintMNOJTC.MaximalCount - constraintMNOJTC.RestDays + 1);
                DateTime endDateTimeInterval = jobs[jobs.Count - 1].JobEndDate.Date.AddDays(constraintMNOJTC.MaximalCount + constraintMNOJTC.RestDays - 1);

                // 0 - does not work on that job
                // 1 - does work on that job
                // 2 - does work on that job type
                int[,] employeeWorksOnJobFromOtherSchedule = FIndAlassigmentsFromOtherSchedules(employees, jobs, constraintMNOJTC, jobsFromOutsides, startDateTimeInterval, endDateTimeInterval);
                int[,] employeeWorksOnJobFromOtherScheduleREST = FIndAlassigmentsFromOtherSchedulesREST(employees, jobs, constraintMNOJTC, jobsFromOutsides, startDateTimeInterval, endDateTimeInterval);

                for (int emp = 0; emp < employees.Count; emp++)
                {
                    List<ILiteral> daysOffForAllTypes = new List<ILiteral>();
                    List<ILiteral> daysOffForSpecificJobType = new List<ILiteral>();

                    // For the REST period
                    List<ILiteral> daysOffForAllTypesREST = new List<ILiteral>();
                    List<ILiteral> daysOffForSpecificJobTypeREST = new List<ILiteral>();

                    // All jobs
                    List<ILiteral>[] includingAllJobsOnSpecificDate = new List<ILiteral>[employeeWorksOnJobFromOtherSchedule.GetLength(1)];
                    // Only with specific job type
                    List<ILiteral>[] includeWithSpecificJobTypeAndDate = new List<ILiteral>[employeeWorksOnJobFromOtherSchedule.GetLength(1)];


                    // All jobs for rest days
                    List<ILiteral>[] includingAllJobsOnSpecificDateREST = new List<ILiteral>[employeeWorksOnJobFromOtherSchedule.GetLength(1)];
                    // Only with specific job type for rest days
                    List<ILiteral>[] includeWithSpecificJobTypeAndDateREST = new List<ILiteral>[employeeWorksOnJobFromOtherSchedule.GetLength(1)];

                    for (int jwsd = 0; jwsd < includingAllJobsOnSpecificDate.Length; jwsd++)
                    {
                        includingAllJobsOnSpecificDate[jwsd] = new List<ILiteral>();
                        includeWithSpecificJobTypeAndDate[jwsd] = new List<ILiteral>();


                        includingAllJobsOnSpecificDateREST[jwsd] = new List<ILiteral>();
                        includeWithSpecificJobTypeAndDateREST[jwsd] = new List<ILiteral>();
                    }

                    for (int j = 0; j < jobs.Count; j++)
                    {
                        if (IsCompared(constraintMNOJTC, employees[emp], jobs[j]))
                        {
                            int partOfJob = 0;
                            Console.WriteLine(jobs[j].JobStartDate.AddDays(partOfJob).Date);
                            while (jobs[j].JobStartDate.AddDays(partOfJob).Date <= jobs[j].JobEndDate.Date)
                            {
                                // Max count
                                if (partOfJob != 1)
                                {
                                    includingAllJobsOnSpecificDate[(int)(jobs[j].JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);
                                    if (jobs[j].JobTypeID == constraintMNOJTC.JobTypeID)
                                    {
                                        includeWithSpecificJobTypeAndDate[(int)(jobs[j].JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);
                                    }

                                    if (partOfJob == 2)
                                    {
                                        includingAllJobsOnSpecificDate[(int)(jobs[j].JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);
                                        if (jobs[j].JobTypeID == constraintMNOJTC.JobTypeID)
                                        {
                                            includeWithSpecificJobTypeAndDate[(int)(jobs[j].JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);
                                        }
                                    }
                                }

                                // Rest 
                                includingAllJobsOnSpecificDateREST[(int)(jobs[j].JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);
                                if (jobs[j].JobTypeID == constraintMNOJTC.JobTypeID)
                                {
                                    includeWithSpecificJobTypeAndDateREST[(int)(jobs[j].JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays].Add(matrix[emp, j]);
                                }

                                partOfJob++;
                            }
                            Console.WriteLine(partOfJob);
                        }
                    }

                    for (int j = 0; j < includingAllJobsOnSpecificDate.Length; j++)
                    {
                        // All Job Types
                        if (includingAllJobsOnSpecificDate[j].Count > 0)
                        {                            
                            daysOffForAllTypes.Add(model.NewBoolVar("day" + j));

                            // Employee does not work on specific day, if the sum of all jobs on the following day is 0
                            model.Add(LinearExpr.Sum(includingAllJobsOnSpecificDate[j]) == 0).OnlyEnforceIf(daysOffForAllTypes[daysOffForAllTypes.Count - 1].Not());
                            model.Add(LinearExpr.Sum(includingAllJobsOnSpecificDate[j]) >= 1).OnlyEnforceIf(daysOffForAllTypes[daysOffForAllTypes.Count - 1]);
                        }
                        else
                        {
                            daysOffForAllTypes.Add(null);
                        }

                        // Specific Job Type
                        if (includeWithSpecificJobTypeAndDate[j].Count > 0)
                        {
                            daysOffForSpecificJobType.Add(model.NewBoolVar("day" + j));

                            // Employee does not work on specific day, if the sum of all jobs on the following day is 0
                            model.Add(LinearExpr.Sum(includeWithSpecificJobTypeAndDate[j]) == 0).OnlyEnforceIf(daysOffForSpecificJobType[daysOffForSpecificJobType.Count - 1].Not());
                            model.Add(LinearExpr.Sum(includeWithSpecificJobTypeAndDate[j]) >= 1).OnlyEnforceIf(daysOffForSpecificJobType[daysOffForSpecificJobType.Count - 1]);
                        }
                        else
                        {
                            daysOffForSpecificJobType.Add(null);
                        }


                        // Rest
                        if (includingAllJobsOnSpecificDateREST[j].Count > 0)
                        {
                            daysOffForAllTypesREST.Add(model.NewBoolVar("day" + j));

                            // Employee does not work on specific day, if the sum of all jobs on the following day is 0
                            model.Add(LinearExpr.Sum(includingAllJobsOnSpecificDateREST[j]) == 0).OnlyEnforceIf(daysOffForAllTypesREST[daysOffForAllTypesREST.Count - 1].Not());
                            model.Add(LinearExpr.Sum(includingAllJobsOnSpecificDateREST[j]) >= 1).OnlyEnforceIf(daysOffForAllTypesREST[daysOffForAllTypesREST.Count - 1]);
                        }
                        else
                        {
                            daysOffForAllTypesREST.Add(null);
                        }

                        // Specific Job Type
                        if (includeWithSpecificJobTypeAndDateREST[j].Count > 0)
                        {
                            daysOffForSpecificJobTypeREST.Add(model.NewBoolVar("day" + j));

                            // Employee does not work on specific day, if the sum of all jobs on the following day is 0
                            model.Add(LinearExpr.Sum(includeWithSpecificJobTypeAndDateREST[j]) == 0).OnlyEnforceIf(daysOffForSpecificJobTypeREST[daysOffForSpecificJobTypeREST.Count - 1].Not());
                            model.Add(LinearExpr.Sum(includeWithSpecificJobTypeAndDateREST[j]) >= 1).OnlyEnforceIf(daysOffForSpecificJobTypeREST[daysOffForSpecificJobTypeREST.Count - 1]);
                        }
                        else
                        {
                            daysOffForSpecificJobTypeREST.Add(null);
                        }
                    }

                    // Add Constraint 
                    for (int i = 0; i < includingAllJobsOnSpecificDate.Length - constraintMNOJTC.MaximalCount - constraintMNOJTC.RestDays + 1; i++)
                    {
                        var newDay = model.NewBoolVar("constraint MNOJTC day " + i + " " + emp);

                        List<ILiteral> workOnEachDay = new List<ILiteral>();
                        int countDaysOnOtherSchedule = 0;

                        for (int j = 0; j < constraintMNOJTC.MaximalCount; j++)
                        {
                            if (employeeWorksOnJobFromOtherSchedule[emp, i + j] == 2)
                            {
                                countDaysOnOtherSchedule++;
                            }
                            else
                            {
                                workOnEachDay.Add(daysOffForSpecificJobType[i + j]);
                            }
                        }

                        model.Add(LinearExpr.Sum(workOnEachDay) == constraintMNOJTC.MaximalCount - countDaysOnOtherSchedule).OnlyEnforceIf(newDay);
                        model.Add(LinearExpr.Sum(workOnEachDay) < constraintMNOJTC.MaximalCount - countDaysOnOtherSchedule).OnlyEnforceIf(newDay.Not());

                        List<ILiteral> restOnEachDay = new List<ILiteral>();
                        for (int j = constraintMNOJTC.MaximalCount; j < constraintMNOJTC.MaximalCount + constraintMNOJTC.RestDays; j++)
                        {
                            if (employeeWorksOnJobFromOtherScheduleREST[emp, i + j] >= 1)
                            {
                                model.Add(newDay == 0);
                            }

                            restOnEachDay.Add(daysOffForAllTypesREST[i + j]);

                            if (employeeWorksOnJobFromOtherScheduleREST[emp, i + j] == 2)
                            {
                                model.Add(newDay == 0);
                            }

                            restOnEachDay.Add(daysOffForSpecificJobTypeREST[i + j]);
                        }

                        model.Add(LinearExpr.Sum(restOnEachDay) == 0).OnlyEnforceIf(newDay);
                    }
                }
            }
        }

        private static int[,] FIndAlassigmentsFromOtherSchedules(List<Employee> employees, List<ScheduleJob> jobs, ConstraintMNOJTC constraintMNOJTC, List<JobFromOutside> jobsFromOutsides, DateTime startDateTimeInterval, DateTime endDateTimeInterval)
        {
            int[,] employeeWorksOnJobFromOtherSchedule = new int[employees.Count, (int)(jobs[jobs.Count - 1].JobEndDate.Date - jobs[0].JobStartDate.Date).TotalDays + 2 * (constraintMNOJTC.MaximalCount + constraintMNOJTC.RestDays - 1) + 7];

            for (int i = 0; i < employeeWorksOnJobFromOtherSchedule.GetLength(0); i++)
            {
                for (int j = 0; j < employeeWorksOnJobFromOtherSchedule.GetLength(1); j++)
                {
                    employeeWorksOnJobFromOtherSchedule[i, j] = 0;
                }
            }

            foreach (JobFromOutside jobFO in jobsFromOutsides)
            {
                int partOfJob = 0;

                while (jobFO.JobStartDate.AddDays(partOfJob).Date <= jobFO.JobEndDate.Date)
                {
                    if (partOfJob != 1)
                    {
                        if (jobFO.JobStartDate.AddDays(partOfJob).Date >= startDateTimeInterval && jobFO.JobEndDate.Date <= endDateTimeInterval)
                        {
                            for (int empAssigment = 0; empAssigment < jobFO.EmployeeAssigments.Count; empAssigment++)
                            {
                                if (IsCompared(constraintMNOJTC, employees[empAssigment], jobFO))
                                {
                                    for (int e = 0; e < employees.Count; e++)
                                    {
                                        if (employees[e].Id == jobFO.EmployeeAssigments[empAssigment])
                                        {
                                            if (jobs.Exists(x => x.JobTypeID == constraintMNOJTC.JobTypeID))
                                            {
                                                employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] = 2;
                                                if (employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] == 0)
                                                {
                                                    employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] = 1;
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                if (jobFO.JobTypeID == constraintMNOJTC.JobTypeID)
                                                {
                                                    employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] = 2;
                                                }
                                                else if (employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] == 0)
                                                {
                                                    employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] = 1;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (partOfJob == 2)
                        {
                            if (jobFO.JobStartDate.AddDays(partOfJob - 1).Date >= startDateTimeInterval && jobFO.JobEndDate.Date <= endDateTimeInterval)
                            {
                                for (int empAssigment = 0; empAssigment < jobFO.EmployeeAssigments.Count; empAssigment++)
                                {
                                    if (IsCompared(constraintMNOJTC, employees[empAssigment], jobFO))
                                    {
                                        for (int e = 0; e < employees.Count; e++)
                                        {
                                            if (employees[e].Id == jobFO.EmployeeAssigments[empAssigment])
                                            {
                                                if (jobs.Exists(x => x.JobTypeID == constraintMNOJTC.JobTypeID))
                                                {
                                                    employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays] = 2;
                                                    if (employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays] == 0)
                                                    {
                                                        employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays] = 1;
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                    if (jobFO.JobTypeID == constraintMNOJTC.JobTypeID)
                                                    {
                                                        employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays] = 2;
                                                    }
                                                    else if (employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays] == 0)
                                                    {
                                                        employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob - 1).Date - startDateTimeInterval.Date).TotalDays] = 1;
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    partOfJob++;
                }
            }

            return employeeWorksOnJobFromOtherSchedule;
        }

        private static int[,] FIndAlassigmentsFromOtherSchedulesREST(List<Employee> employees, List<ScheduleJob> jobs, ConstraintMNOJTC constraintMNOJTC, List<JobFromOutside> jobsFromOutsides, DateTime startDateTimeInterval, DateTime endDateTimeInterval)
        {
            int[,] employeeWorksOnJobFromOtherSchedule = new int[employees.Count, (int)(jobs[jobs.Count - 1].JobEndDate.Date - jobs[0].JobStartDate.Date).TotalDays + 2 * (constraintMNOJTC.MaximalCount + constraintMNOJTC.RestDays - 1) + 7];

            for (int i = 0; i < employeeWorksOnJobFromOtherSchedule.GetLength(0); i++)
            {
                for (int j = 0; j < employeeWorksOnJobFromOtherSchedule.GetLength(1); j++)
                {
                    employeeWorksOnJobFromOtherSchedule[i, j] = 0;
                }
            }

            foreach (JobFromOutside jobFO in jobsFromOutsides)
            {
                int partOfJob = 0;

                while (jobFO.JobStartDate.AddDays(partOfJob).Date <= jobFO.JobEndDate.Date)
                {
                    if (jobFO.JobStartDate.AddDays(partOfJob).Date >= startDateTimeInterval && jobFO.JobEndDate.Date <= endDateTimeInterval)
                    {
                        for (int empAssigment = 0; empAssigment < jobFO.EmployeeAssigments.Count; empAssigment++)
                        {
                            if (IsCompared(constraintMNOJTC, employees[empAssigment], jobFO))
                            {
                                for (int e = 0; e < employees.Count; e++)
                                {
                                    if (employees[e].Id == jobFO.EmployeeAssigments[empAssigment])
                                    {
                                        if (jobs.Exists(x => x.JobTypeID == constraintMNOJTC.JobTypeID))
                                        {
                                            employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] = 2;
                                            if (employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] == 0)
                                            {
                                                employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] = 1;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            if (jobFO.JobTypeID == constraintMNOJTC.JobTypeID)
                                            {
                                                employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] = 2;
                                            }
                                            else if (employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] == 0)
                                            {
                                                employeeWorksOnJobFromOtherSchedule[e, (int)(jobFO.JobStartDate.AddDays(partOfJob).Date - startDateTimeInterval.Date).TotalDays] = 1;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    partOfJob++;
                }
            }

            return employeeWorksOnJobFromOtherSchedule;
        }
    }
}
