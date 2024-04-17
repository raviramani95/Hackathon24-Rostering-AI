using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eSolver.Entities.Constraints.Untiles;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using Google.OrTools.Sat;
using static eSolver.BusinessLogic.Utiles.Compared;
using eSolver.Entities.Constraints;

namespace eSolver.BusinessLogic.ConstraintsLogic
{
    public static class Const_SplitShifts
    {

        /// <summary>
        /// 3.1.15 Split shifts
        /// </summary>
        public static void ApplySplitShifts(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, ScheduleActiveConstraint constraint, List<JobFromOutside> jobsFromOutsides)
        {
            ConstraintSS currentConstraint = constraint.ConstraintSS;
            List<long> jobsWithoutPreCondition;
            List<int> employeeIDAssigmentsOnSchedule;
            for (int emp = 0; emp < employees.Count; emp++)
            {
                jobsWithoutPreCondition = new List<long>();
                /*
                 *  How jobs from other schedules affect the working schedule
                 *  Exculding jobs that are not possible to assigne
                 *  Including condition for possible assignation
                 */
                foreach (JobFromOutside jobFromOutside in jobsFromOutsides)
                {
                    // If employee satisfies custom date of the job on the other schedule for that constraint
                    if (IsCompared(constraint, constraint.ConstraintSS.CustomData, employees[emp], jobFromOutside))
                    {
                        employeeIDAssigmentsOnSchedule = jobFromOutside.EmployeeAssigments.Where(x => x == employees[emp].Id).ToList();
                        //Caclulate min and max ranges when employee will be available or unavailable for that job on the other schedule 
                        DateTime minGapCausedToBeUNAvailable = jobFromOutside.JobStartDate.AddHours(-constraint.ConstraintSS.MaximumGap);
                        DateTime maxPeriodCausedToBeUNAvailable = jobFromOutside.JobStartDate.AddHours(-constraint.ConstraintSS.MinimumRestPeriod);

                        DateTime minGapCausedToBeAvailable = jobFromOutside.JobEndDate.AddHours(constraint.ConstraintSS.MaximumGap);
                        DateTime maxPeriodCausedToBeAvailable = jobFromOutside.JobEndDate.AddHours(constraint.ConstraintSS.MinimumRestPeriod);

                        // Passing all jobs so that could eliminate jobs on which employee can't work, bacause of the job of the other schedule
                        for (int j = 0; j < jobs.Count; j++)
                        {
                            // If employee satisfies custom date of the job on the other schedule for that constraint
                            if (IsCompared(constraint, currentConstraint.CustomData, employees[emp], jobs[j]))
                            {
                                // If job is in calculated range then set him to be unassigned 
                                // Before job on the other shcuele
                                if (jobs[j].JobEndDate > maxPeriodCausedToBeUNAvailable && jobs[j].JobEndDate < minGapCausedToBeUNAvailable)
                                {
                                    // Check if that employee works on the other schedule and if it's work then set him to be unassigned
                                    for (int e = 0; e < employeeIDAssigmentsOnSchedule.Count; e++)
                                    {
                                        model.Add((IntVar)matrix[emp, j] == 0);
                                        // Add jobs that needs to be unassinged for the other jobs on wokring shedule
                                        jobsWithoutPreCondition.Add(j);
                                    }
                                }

                                // Set employee from after the job on the other schedule to be unavailable
                                // After the job on the other schedule
                                if (jobs[j].JobStartDate > minGapCausedToBeAvailable && jobs[j].JobStartDate < maxPeriodCausedToBeAvailable)
                                {
                                    foreach (int empA in employeeIDAssigmentsOnSchedule)
                                    {
                                        model.Add((IntVar)matrix[emp, j] == 0);
                                    }
                                    /*
                                    if (!(jobs[j].JobStartDate >= minGapCausedToBeAvailable  || jobs[j].JobStartDate <= maxPeriodCausedToBeAvailable))
                                    {
                                        jobsWithoutPreCondition.Add(j);
                                    }*/
                                }
                            }
                        }
                    }
                }

                /*
                 *  How jobs on the same schedule affects each others
                 */
                DateTime date = jobs[0].JobStartDate.Date;
                List<ILiteral> sameDayJobs = new List<ILiteral>();
                for (int j = 0; j < jobs.Count; j++)
                {                    
                    if (date != jobs[j].JobStartDate.Date)
                    {
                        date = jobs[j].JobStartDate.Date;                        
                    }
                    sameDayJobs.Add((ILiteral)(matrix[emp, j]));
                    // If employee satisfies custom date of the job on the other schedule for that constraint
                    if (IsCompared(constraint, currentConstraint.CustomData, employees[emp], jobs[j]))
                    {
                        // Calculate range for job
                        DateTime minGap = jobs[j].JobEndDate.AddHours(constraint.ConstraintSS.MaximumGap);
                        DateTime maxPeriod = jobs[j].JobEndDate.AddHours(constraint.ConstraintSS.MinimumRestPeriod);

                        // Indexes of jobs that will be unavailable for employee
                        List<int> invalidIndex = new List<int>();
                        // Indexes of jobs that requires to be uniassgned
                        List<int> requiredInvalidIndex = new List<int>();

                        for (int i = j + 1; i < jobs.Count; i++)
                        {
                            if (IsCompared(constraint, currentConstraint.CustomData, employees[emp], jobs[i]))
                            {
                                if (jobs[j].JobEndDate <= jobs[i].JobStartDate && jobs[i].JobStartDate <= minGap)
                                {
                                    foreach (JobFromOutside jobFromOutside in jobsFromOutsides)
                                    {
                                        if (jobFromOutside.JobEndDate <= jobs[i].JobStartDate && jobs[i].JobStartDate <= minGap)
                                        {
                                            foreach (long empJ in jobFromOutside.EmployeeAssigments)
                                            {
                                                if (employees[emp].Id == empJ)
                                                {
                                                    goto jobFromOtherScheduleRequiredIt;
                                                }
                                            }
                                        }
                                    }
                                    requiredInvalidIndex.Add(i);
                                jobFromOtherScheduleRequiredIt:;
                                }
                                //else if (jobs[i].JobStartDate > minGap && jobs[i].JobStartDate <= maxPeriod) // 22/12/2020
                                else if (jobs[j].JobEndDate <= jobs[i].JobStartDate && jobs[i].JobStartDate < maxPeriod)
                                {
                                    for (int l = 0; l < jobsWithoutPreCondition.Count; l++)
                                    {
                                        if (jobsWithoutPreCondition[l] == i)
                                        {
                                            goto jobFromOtherScheduleAnableIt;
                                        }
                                    }
                                    invalidIndex.Add(i);
                                }
                            jobFromOtherScheduleAnableIt:;
                            }
                        }

                        foreach (JobFromOutside jobFromOutside in jobsFromOutsides)
                        {
                            if (jobFromOutside.JobEndDate <= jobs[j].JobStartDate && jobs[j].JobStartDate <= minGap)
                            {
                                foreach (long empJ in jobFromOutside.EmployeeAssigments)
                                {
                                    if (emp == empJ)
                                    {
                                        requiredInvalidIndex = new List<int>();
                                    }
                                }
                            }
                        }
                        Console.WriteLine();
                        if (invalidIndex.Count != 0)
                        {
                            LinearExpr[] linearExpr = new LinearExpr[invalidIndex.Count];
                            LinearExpr[] requiredEmptyJobs = new LinearExpr[requiredInvalidIndex.Count];// + 1];
                            for (int i = 0; i < invalidIndex.Count; i++)
                            {
                                Console.WriteLine("linearExpr " + emp + " " + invalidIndex[i]);
                                linearExpr[i] = (LinearExpr)matrix[emp, invalidIndex[i]];
                            }

                            for (int i = 0; i < requiredInvalidIndex.Count; i++)
                            {
                                Console.WriteLine("requiredEmptyJobs " + emp + " " + requiredInvalidIndex[i]);
                                requiredEmptyJobs[i] = (LinearExpr)matrix[emp, requiredInvalidIndex[i]];
                            }

                            //requiredEmptyJobs[requiredInvalidIndex.Count] = (LinearExpr)matrix[emp, j].Not();


                            if (invalidIndex.Count != 0)
                            {
                                if (requiredInvalidIndex.Count == 0)
                                {
                                    model.Add(LinearExpr.Sum(linearExpr) == 0).OnlyEnforceIf(matrix[emp, j]);
                                }
                                else
                                {
                                    List<IntVar> list = new List<IntVar>();
                                    BoolVar enableJob = model.NewBoolVar("job " + j);
                                    BoolVar ifbool = model.NewBoolVar("jobr " + j);
                                    //model.Add(LinearExpr.Sum(linearExpr) == 0).OnlyEnforceIf(enableJob);
                                    model.Add(LinearExpr.Sum(requiredEmptyJobs) == 0).OnlyEnforceIf(ifbool);
                                    model.Add(LinearExpr.Sum(linearExpr) == 0).OnlyEnforceIf(enableJob.Not()); //13/20/2020 model.Add(LinearExpr.Sum(linearExpr) != 0).OnlyEnforceIf(enableJob.Not()); -- 02 02 2022 ==
                                    model.Add(LinearExpr.Sum(requiredEmptyJobs) != 0).OnlyEnforceIf(ifbool.Not());

                                    list.Add(enableJob);
                                    list.Add(ifbool);
                                    model.Add(LinearExpr.Sum(list) == 1).OnlyEnforceIf(matrix[emp, j]);
                                    
                                }
                            }
                        }
                    }
                    if (j < jobs.Count - 1)
                    {
                        if (jobs[j].JobStartDate.Date != jobs[j + 1].JobStartDate.Date)
                        {
                            if (sameDayJobs.Count > 1)
                            {
                                model.Add(LinearExpr.Sum(sameDayJobs) <= (int)sameDayJobs.Count - 1);
                                sameDayJobs.Clear();
                            }                                
                        }
                    }
                    else
                    {
                        if (sameDayJobs.Count > 1)
                        {
                            model.Add(LinearExpr.Sum(sameDayJobs) <= (int)sameDayJobs.Count - 1);
                            sameDayJobs.Clear();
                        }
                        
                    }
                }
            }
        }
    }
}
