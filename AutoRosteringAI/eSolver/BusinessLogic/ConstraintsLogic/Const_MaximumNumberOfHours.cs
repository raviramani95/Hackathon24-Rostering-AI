using System;
using System.Collections.Generic;
using eSolver.Entities.Constraints;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using Google.OrTools.Sat;
using static eSolver.BusinessLogic.Utiles.Compared;
using eSolver.Entities.Interfaces;
using eSolver.BusinessLogic.Managers;

namespace eSolver.BusinessLogic.ConstraintsLogic
{
    public static class Const_MaximumNumberOfHours
    {
        public static void ApplyMaximumNumberOfHours(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, ScheduleActiveConstraint constraint, PayPeriodDTO payPeriodDTO, List<JobFromOutside> jobsFromOutsides, string startDayOfTheWeek)
        {
            ConstraintMNOH currentConstraint = constraint.ConstraintMNOH;

            int numEmp = 0;
            DateRangeManager dateRangeManger = new DateRangeManager();
            
            for (int emp = 0; emp < employees.Count; emp++)
            {
                ILiteral[] tmp = new ILiteral[jobs.Count];
                List<LinearExpr> linearExpr = new List<LinearExpr>();
                if (currentConstraint.ConstraintCustomRange == null)
                {
                    List<int> jobsFromEarlierPeriod = new List<int>();
                    bool IsBeforePayPeriod = false; 

                    for (int j = 0; j < jobs.Count; j++)
                    {
                        DateTime[] dateRange = dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[j].JobStartDate, jobs[j].JobEndDate, payPeriodDTO, startDayOfTheWeek);

                        if (dateRange != null)
                        {
                            if (IsCompared(constraint, currentConstraint.CustomData, employees[emp], jobs[j]))
                            {
                                tmp[j] = matrix[numEmp, j];
                                linearExpr.Add(LinearExpr.Term(tmp[j], CalculateTime(jobs[j], dateRange)));
                                //linearExpr[j] = LinearExpr.Term(tmp[j], CalculateTime(jobs[j], dateRange));
                                if (jobs[j].JobEndDate >= dateRange[1])
                                {
                                    jobsFromEarlierPeriod.Add(j);
                                }
                            }
                            //Console.WriteLine();
                            //Console.WriteLine(emp);
                            //Console.WriteLine(IsCompared(constraint, currentConstraint.CustomData, employees[emp], jobs[j]));
                            

                            if (IsBeforePayPeriod)
                            {
                                for (int i = j - 1; i >= 0; i--)
                                {
                                    if (IsCompared(constraint, currentConstraint.CustomData, employees[emp], jobs[i]))
                                    {
                                        tmp[i] = matrix[numEmp, i];
                                        linearExpr.Add(LinearExpr.Term(tmp[i], CalculateTime(jobs[i], dateRange)));
                                        //linearExpr[i] = LinearExpr.Term(tmp[i], CalculateTime(jobs[i], dateRange)); 
                                        if (jobs[i].JobEndDate >= dateRange[1])
                                        {
                                            jobsFromEarlierPeriod.Add(i);
                                        }
                                    }
                                    //Console.WriteLine(IsCompared(constraint, currentConstraint.CustomData, employees[emp], jobs[i]));
                                }
                                IsBeforePayPeriod = false;
                            }
                        }
                        else
                        {
                            IsBeforePayPeriod = true;
                        }

                        if (UpdateModelConditions(dateRange, jobs, j, payPeriodDTO, employees[emp], model, dateRangeManger, constraint, linearExpr, startDayOfTheWeek, jobsFromOutsides, jobs[j].JobCustomData))
                        {
                            Console.WriteLine();
                            tmp = new ILiteral[jobs.Count];
                            linearExpr = new List<LinearExpr>();

                            if (j != jobs.Count - 1)
                            {
                                dateRange = dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[j+1].JobStartDate, jobs[j+1].JobEndDate, payPeriodDTO, startDayOfTheWeek);
                                for (int i = jobsFromEarlierPeriod.Count; i-- > 0;)
                                {
                                    if (jobs[jobsFromEarlierPeriod[i]].JobStartDate <= dateRange[0] && jobs[jobsFromEarlierPeriod[i]].JobEndDate >= dateRange[0])
                                    {
                                        tmp[jobsFromEarlierPeriod[i]] = matrix[numEmp, jobsFromEarlierPeriod[i]];
                                        //Console.WriteLine("A: " + (CalculateTime(jobs[jobsFromEarlierPeriod[i]], dateRange) / 60));
                                        linearExpr.Add(LinearExpr.Term(tmp[jobsFromEarlierPeriod[i]], CalculateTime(jobs[jobsFromEarlierPeriod[i]], dateRange)));
                                        //linearExpr[jobsFromEarlierPeriod[i]] = LinearExpr.Term(tmp[jobsFromEarlierPeriod[i]], CalculateTime(jobs[jobsFromEarlierPeriod[i]], dateRange));
                                    }
                                    else
                                    {
                                        jobsFromEarlierPeriod.RemoveAt(i);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int fixedIndexOfjob = 0; fixedIndexOfjob < jobs.Count; fixedIndexOfjob++)
                    {
                        DateTime[] dateRange = dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[fixedIndexOfjob].JobStartDate, jobs[fixedIndexOfjob].JobEndDate, payPeriodDTO, startDayOfTheWeek);

                        if (dateRange != null && IsCompared(constraint, currentConstraint.CustomData, employees[emp], jobs[fixedIndexOfjob]))
                        {
                            for (int i = 0; i < jobs.Count; i++)
                            {
                                /* As max number of days for job is one week - there is limit condition of breaking loop earlier
                                if (jobs[i].JobEndDate.AddDays(-7) < dateRange[1])
                                {
                                    break;
                                }*/
                                long jobsTimeWithinPeriod = CalculateTime(jobs[i], dateRange);

                                //if (jobsTimeWithinPeriod != 0) 
                                //{
                                    tmp[i] = matrix[numEmp, i];
                                linearExpr.Add(LinearExpr.Term(tmp[i], jobsTimeWithinPeriod));
                                //linearExpr[i] = LinearExpr.Term(tmp[i], jobsTimeWithinPeriod);
                                //}
                            }
                            UpdateModelConditionsCustom(dateRange, jobs, fixedIndexOfjob, payPeriodDTO, employees[emp], model, dateRangeManger, constraint, linearExpr, startDayOfTheWeek, jobsFromOutsides, (BoolVar)(matrix[emp, fixedIndexOfjob]));

                        }
                        tmp = new ILiteral[jobs.Count];
                        linearExpr = new List<LinearExpr>();
                    }
                }
                numEmp++;
            }
        }


        // It sould be parsed only current job and next job insteand of pasring the hole list
        static bool UpdateModelConditions(DateTime[] dateRange, List<ScheduleJob> jobs, int j, PayPeriodDTO payPeriodDTO, Employee employee, CpModel model, DateRangeManager dateRangeManger, ScheduleActiveConstraint constraint, List<LinearExpr> linearExpr, string startDayOfTheWeek, List<JobFromOutside> jobsFromOutsides, List<ScheduleCustomData> scheduleCustomData)
        {
            bool resetCondition = false;
            if (dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[j].JobStartDate, jobs[j].JobEndDate, payPeriodDTO, startDayOfTheWeek) != null && dateRange[0] != null)
            {
                if (j < jobs.Count -1)
                {
                    //Console.WriteLine(dateRange[0]);
                    //Console.WriteLine(dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0].Date);
                    //Console.WriteLine((DateTime.Compare(dateRange[0], dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0].Date) != 0) + "+");
                }
                /*Console.WriteLine((j == jobs.Count - 1) ||
                    (dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek) == null) ||
                    (DateTime.Compare(dateRange[0], dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0]) != 0));
                */
                if ((j == jobs.Count - 1) ||
                    (dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek) == null) ||
                    (DateTime.Compare(dateRange[0], dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0]) != 0)) // 06/08/2020 != 0
                {
                    if (IsCompared(constraint, scheduleCustomData, employee, jobs[j]))
                    {
                        long hours = 0;
                        if (jobsFromOutsides != null)
                        {
                            foreach (JobFromOutside jobFromOutside in jobsFromOutsides)
                            {
                                if (dateRange != null && dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobFromOutside.JobStartDate, jobFromOutside.JobStartDate.AddHours(jobFromOutside.HoursT.TotalHours), payPeriodDTO, startDayOfTheWeek) != null)
                                {
                                    if ((DateTime.Compare(dateRange[0], dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobFromOutside.JobStartDate, jobFromOutside.JobStartDate.AddHours(jobFromOutside.HoursT.TotalHours), payPeriodDTO, startDayOfTheWeek)[0]) == 0))
                                    {
                                        foreach (int employeeAssigments in jobFromOutside.EmployeeAssigments)
                                        {
                                            if (IsCompared(constraint, scheduleCustomData, employee, jobFromOutside))
                                            {
                                                if (employee.Id == employeeAssigments)
                                                {
                                                    hours += CalculateTime(jobFromOutside, dateRange); 
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //hours /= 60;
                        Console.WriteLine(hours);
                        Console.WriteLine();
                        if (constraint.ConstraintMNOH.SetValue != null)
                        {
                            if (hours < (int)constraint.ConstraintMNOH.SetValue * 60)
                            {
                                BoolVar suma = model.NewBoolVar("Suma " + j);

                                model.Add(LinearExpr.Sum(linearExpr) <= ((int)(constraint.ConstraintMNOH.SetValue * 60) - hours)).OnlyEnforceIf(suma); //maxHours
                                model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(suma.Not());

                            }
                            else
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= 0);
                            }
                        }
                        else if (constraint.ConstraintMNOH.MaxHoursField != null)
                        {
                            if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 1"))
                            {
                                if (employee.MaxHours1 != null)
                                {
                                    if (hours <= (int)employee.MaxHours1 * 60)
                                    {
                                        BoolVar suma = model.NewBoolVar("Suma " + j);
                                        model.Add(LinearExpr.Sum(linearExpr) <= ((int)employee.MaxHours1 * 60 - hours)).OnlyEnforceIf(suma);
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(suma.Not());
                                    }
                                    else
                                    {
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0);
                                    }
                                }
                            }
                            else if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 2"))
                            {
                                if (employee.MaxHours2 != null && hours <= (int)employee.MaxHours2 * 60)
                                {
                                    if (hours <= (int)employee.MaxHours2 * 60)
                                    {
                                        BoolVar suma = model.NewBoolVar("Suma " + j);
                                        model.Add(LinearExpr.Sum(linearExpr) <= ((int)employee.MaxHours2 * 60 - hours)).OnlyEnforceIf(suma);
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(suma.Not());
                                    }
                                    else
                                    {
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0);
                                    }
                                }
                            }
                            else if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 3"))
                            {
                                if (employee.MaxHours3 != null && hours <= (int)employee.MaxHours3 * 60)
                                {
                                    if (hours <= (int)employee.MaxHours3 * 60)
                                    {
                                        BoolVar suma = model.NewBoolVar("Suma " + j);
                                        model.Add(LinearExpr.Sum(linearExpr) <= ((int)employee.MaxHours3 * 60 - hours)).OnlyEnforceIf(suma);
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(suma.Not());
                                    }
                                    else
                                    {
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0);
                                    }
                                }
                            }
                            else if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 4"))
                            {
                                if (employee.MaxHours4 != null && hours <= (int)employee.MaxHours4 * 60)
                                {
                                    if (hours <= (int)employee.MaxHours4 * 60)
                                    {
                                        BoolVar suma = model.NewBoolVar("Suma " + j);
                                        model.Add(LinearExpr.Sum(linearExpr) <= ((int)employee.MaxHours4 * 60 - hours)).OnlyEnforceIf(suma);
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(suma.Not());
                                    }
                                    else
                                    {
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0);
                                    }
                                }
                            }
                            else if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 5"))
                            {
                                if (employee.MaxHours5 != null)
                                {
                                    if (hours <= (int)employee.MaxHours5 * 60)
                                    {
                                        BoolVar suma = model.NewBoolVar("Suma " + j);
                                        model.Add(LinearExpr.Sum(linearExpr) <= ((int)employee.MaxHours5 * 60 - hours)).OnlyEnforceIf(suma);
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(suma.Not());
                                    }
                                    else
                                    {
                                        model.Add(LinearExpr.Sum(linearExpr) <= 0);
                                    }
                                }
                            }
                        }
                    }
                    Console.WriteLine("RESET");
                    resetCondition = true;
                }
            }
            return resetCondition;
        }

        static bool UpdateModelConditionsCustom(DateTime[] dateRange, List<ScheduleJob> jobs, int j, PayPeriodDTO payPeriodDTO, Employee employee, CpModel model, DateRangeManager dateRangeManger, ScheduleActiveConstraint constraint, List<LinearExpr> linearExpr, string startDayOfTheWeek, List<JobFromOutside> jobsFromOutsides, BoolVar intVar)
        {
            long hours = 0;
            if (jobsFromOutsides != null)
            {
                foreach (JobFromOutside jobFromOutsides in jobsFromOutsides)
                {
                    if (IsCompared(constraint, constraint.ConstraintMNOH.CustomData, employee, jobFromOutsides))
                    {
                        if (dateRange != null && dateRangeManger.GenerateDateRange(constraint.ConstraintMNOH, jobFromOutsides.JobStartDate, jobFromOutsides.JobStartDate.AddHours(jobFromOutsides.HoursT.TotalHours), payPeriodDTO, startDayOfTheWeek) != null)
                        {
                            foreach (int employeeAssigment in jobFromOutsides.EmployeeAssigments)
                            {
                                if (employeeAssigment == employee.Id)
                                {
                                    if ((DateTime.Compare(dateRange[0], jobFromOutsides.JobStartDate) <= 0)
                                        && (DateTime.Compare(dateRange[1], jobFromOutsides.JobEndDate) >= 0))
                                    {
                                        hours += CalculateTime(jobFromOutsides, dateRange);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (constraint.ConstraintMNOH.CustomData == null)
            {
                if (constraint.ConstraintMNOH.SetValue != null)
                {
                    if (hours <= (int)constraint.ConstraintMNOH.SetValue * 60)
                    {
                        model.Add(LinearExpr.Sum(linearExpr) <= ((int)constraint.ConstraintMNOH.SetValue * 60) - hours).OnlyEnforceIf(intVar);//maxHours
                    }
                    else
                    {
                        model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(intVar);
                    }
                }
                else if (constraint.ConstraintMNOH.MaxHoursField != null)
                {
                    if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 1"))
                    {
                        if (employee.MaxHours1 != null)
                        {
                            if (hours <= (int)employee.MaxHours1 * 60)
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= (int)employee.MaxHours1 * 60 - hours).OnlyEnforceIf(intVar);
                            }
                            else
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(intVar);
                            }
                        }
                    }
                    else if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 2"))
                    {
                        if (employee.MaxHours2 != null)
                        {
                            if (hours <= (int)employee.MaxHours2 * 60)
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= (int)employee.MaxHours2 * 60 - hours).OnlyEnforceIf(intVar);
                            }
                            else
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(intVar);
                            }
                        }
                    }
                    else if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 3"))
                    {
                        if (employee.MaxHours3 != null)
                        {
                            if (hours <= (int)employee.MaxHours3 * 60)
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= (int)employee.MaxHours3 * 60 - hours).OnlyEnforceIf(intVar);
                            }
                            else
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(intVar);
                            }
                        }
                    }
                    else if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 4"))
                    {
                        if (employee.MaxHours4 != null)
                        {
                            if (hours <= (int)employee.MaxHours4 * 60)
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= (int)employee.MaxHours4 * 60 - hours).OnlyEnforceIf(intVar);
                            }
                            else
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(intVar);
                            }
                        }
                    }
                    else if (constraint.ConstraintMNOH.MaxHoursField.Equals("Maximum Hours 5"))
                    {
                        if (employee.MaxHours5 != null)
                        {
                            if (hours <= (int)employee.MaxHours5 * 60)
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= (int)employee.MaxHours5 * 60 - hours).OnlyEnforceIf(intVar);
                            }
                            else
                            {
                                model.Add(LinearExpr.Sum(linearExpr) <= 0).OnlyEnforceIf(intVar);
                            }
                        }
                    }
                }
            }
            return true;
        }


        private static long CalculateTime<T>(T scheduleJob, DateTime[] dateTime) where T : IJobBase
        {
            long totalMinutes = 0;
            if (scheduleJob.JobStartDate.Date != scheduleJob.JobEndDate.Date)
            {
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(0) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(0) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(0) <= scheduleJob.JobEndDate)? (long) scheduleJob.HoursT1.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(1) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(1) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(1) <= scheduleJob.JobEndDate)? (long) scheduleJob.HoursT2.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(2) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(2) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(2) <= scheduleJob.JobEndDate)? (long) scheduleJob.HoursT3.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(3) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(3) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(3) <= scheduleJob.JobEndDate)? (long) scheduleJob.HoursT4.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(4) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(4) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(4) <= scheduleJob.JobEndDate)? (long) scheduleJob.HoursT5.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(5) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(5) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(5) <= scheduleJob.JobEndDate)? (long) scheduleJob.HoursT6.TotalMinutes : 0;

                return totalMinutes;
            }
            else
            {
                return ((scheduleJob.JobStartDate >= dateTime[0]) && (scheduleJob.JobEndDate <= dateTime[1])) ? (long) scheduleJob.HoursT.TotalMinutes : 0;
            }
        }
    }
}