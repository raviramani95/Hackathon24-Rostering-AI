using System;
using System.Collections.Generic;
using System.Linq;
using eSolver.Entities.Constraints;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using Google.OrTools.Sat;
using static eSolver.BusinessLogic.Utiles.Compared;
using LinearExpr = Google.OrTools.Sat.LinearExpr;
using eSolver.Entities.Interfaces;
using eSolver.BusinessLogic.Managers;

namespace eSolver.BusinessLogic.ConstraintsLogic
{
    public static class Const_MaximumNumberOfDaysOfTheWeek
    {
        /// <summary>
        /// 3.1.8 Maximum number of days of the week
        /// </summary>
        public static void ApplyIsMaximumNumberOfDaysOfTheWeek(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, DayOfWeek dayOfWeek, ScheduleActiveConstraint constraint, List<ScheduleCustomData> scheduleCustomData, PayPeriodDTO payPeriodDTO, List<JobFromOutside> jobFromOutsides, string startDayOfTheWeek)
        {

            DateRangeManager dateRangeManger = new DateRangeManager();
            DateTime[] dateRangeTarget = dateRangeManger.GenerateDateRange(constraint.ConstraintMNODOTW, jobs[0].JobStartDate, jobs[0].JobEndDate, payPeriodDTO, startDayOfTheWeek);
            List<int> listOfJobsOfSameTypeDay = new List<int>() { };

            DateTime[][] dateRanges = new DateTime[jobs.Count][];

            int lastNonNUllRange = -1;
            for (int j = 0; j < jobs.Count; j++)
            {
                dateRanges[j] = dateRangeManger.GenerateDateRange(constraint.ConstraintMNODOTW, jobs[j].JobStartDate, jobs[j].JobEndDate, payPeriodDTO, startDayOfTheWeek);
                if (dateRanges[j] == null)
                {
                    lastNonNUllRange = j;
                }
            }

            lastNonNUllRange++;
            //string period = dateRangeManger.getTypeOfPeriod(constraint);
            ConstraintMNODOTW constraintMNODOTW = constraint.ConstraintMNODOTW;

            if (constraintMNODOTW.ConstraintCustomRangeID == null)
            {
                for (int j = lastNonNUllRange; j < jobs.Count; j++)
                {
                    if (DateTime.Compare(dateRanges[j][0], dateRangeTarget[0]) == 0)
                    {
                        AddDayOnOfTheWeek(jobs, dayOfWeek, listOfJobsOfSameTypeDay, constraintMNODOTW, j);
                    }
                    // Stop if its last
                    if (j == jobs.Count - 1)
                    {
                        AddConstraintOfCurrentDay(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, constraint, jobFromOutsides, dayOfWeek, false, 0);
                        listOfJobsOfSameTypeDay.Clear();
                    }// OR if it's last at period
                    else if ((DateTime.Compare(dateRanges[j + 1][0], dateRangeTarget[0]) != 0))
                    {
                        dateRangeTarget = dateRanges[j + 1];
                        AddConstraintOfCurrentDay(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, constraint, jobFromOutsides, dayOfWeek, false, 0);
                        listOfJobsOfSameTypeDay.Clear();
                    }

                }
            }
            else
            {
                for (int jTarget = lastNonNUllRange; jTarget < jobs.Count; jTarget++)
                {
                    if (HasDateOfTheWeek(jobs[jTarget], dayOfWeek))
                    {
                        Console.WriteLine();
                        listOfJobsOfSameTypeDay.Clear();
                        dateRangeTarget = dateRangeManger.GenerateDateRange(constraint.ConstraintMNODOTW, jobs[jTarget].JobStartDate, jobs[jTarget].JobEndDate, payPeriodDTO, startDayOfTheWeek);
                        for (int i = 0; i < jobs.Count; i++)
                        {
                            if(!((jobs[i].JobEndDate <= dateRangeTarget[0]) || (jobs[i].JobStartDate >= dateRangeTarget[1])))
                            {
                                if (HasDateOfTheWeek(jobs[i], dayOfWeek))
                                {
                                    //Console.WriteLine("Add " + i);
                                    AddDayOnOfTheWeek(jobs, dayOfWeek, listOfJobsOfSameTypeDay, constraintMNODOTW, i);
                                }
                                else
                                {
                                    //Console.WriteLine("Dont " + i);
                                }
                            }
                        }
                        /*
                        int earlierTargetJob = jTarget;
                        while (--earlierTargetJob >= lastNonNUllRange && (DateTime.Compare(dateRanges[jTarget][lastNonNUllRange], jobs[earlierTargetJob].JobStartDate) <= 0))
                        {
                            if (jobs[earlierTargetJob].JobStartDate.DayOfWeek == dayOfWeek)
                            {
                                AddDayOnOfTheWeek(jobs, dayOfWeek, listOfJobsOfSameTypeDay, constraintMNODOTW, earlierTargetJob);
                            }
                        }
                        int laterTargetJob = jTarget;
                        while (++laterTargetJob < jobs.Count() && (DateTime.Compare(dateRanges[jTarget][1], jobs[laterTargetJob].JobStartDate) >= 0))
                        {
                            if (jobs[laterTargetJob].JobStartDate.DayOfWeek == dayOfWeek)
                            {
                                AddDayOnOfTheWeek(jobs, dayOfWeek, listOfJobsOfSameTypeDay, constraintMNODOTW, laterTargetJob);
                            }
                        }*/

                        AddConstraintOfCurrentDay(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, constraint, jobFromOutsides, dayOfWeek, true, jTarget);
                        //AddConstraintOfCurrentDayCustom(matrix, model, employees, jobs, listOfJobsOfSameTypeDay, constraint, scheduleCustomData, jobFromOutsides, dateRangeManger, payPeriodDTO, startDayOfTheWeek, jTarget);
                        listOfJobsOfSameTypeDay = new List<int>() { };
                    }
                }
            }
        }

        private static bool HasDateOfTheWeek<T>(T scheduleJob, DayOfWeek dayOfWeek) where T : IJobBase
        {
            DateTime startDay = scheduleJob.JobStartDate;
            do
            {
                if (startDay.DayOfWeek == dayOfWeek)
                {
                    return true;
                }
                startDay = startDay.AddDays(1);
            }
            while (startDay.Date <= scheduleJob.JobEndDate.Date);
            return false;
        }

        private static void AddDayOnOfTheWeek(List<ScheduleJob> jobs, DayOfWeek dayOfWeek, List<int> listOfJobsOfSameTypeDay, ConstraintMNODOTW constraintMNODOTW, int j)
        {
            if ((bool)constraintMNODOTW.CountOvernights)
            {

                DateTime endDay = jobs[j].JobEndDate;
                if (endDay.DayOfWeek == dayOfWeek)
                {
                    listOfJobsOfSameTypeDay.Add(j);
                }
                else
                {
                    DateTime startDay = jobs[j].JobStartDate;
                    while (startDay < endDay)
                    {
                        if ((startDay.DayOfWeek == dayOfWeek))
                        {
                            listOfJobsOfSameTypeDay.Add(j);
                        }
                        startDay = startDay.AddDays(1);
                    }
                }
            }
            else
            {
                if (jobs[j].JobStartDate.DayOfWeek == dayOfWeek)
                {
                    listOfJobsOfSameTypeDay.Add(j);
                }
            }
        }

        /// [3.1.8] Maximum number of days of the weeks
        static void AddConstraintOfCurrentDay(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, List<int> list, ScheduleActiveConstraint constraint, List<JobFromOutside> jobFromOutsides, DayOfWeek startDayOfTheWeek, bool isCustom, int indexOfTargetedJob)
        {
            if (list.Count == 0)
            {
                return;
            }

            ConstraintMNODOTW constraintMNODOTW = constraint.ConstraintMNODOTW;

            for (int emp = 0; emp < employees.Count; emp++)
            {
                List<ILiteral> onSameDay = new List<ILiteral>();
                List<IntVar> daysOfTheWeek = new List<IntVar>();
                List<int> allPossbileDays = list.ToList();

                int mc = constraintMNODOTW.MaxCount;

                List<DateTime> daysOfTheWeekWhereEmployeeAlreadyWorksOutsideOfTheView = new List<DateTime>();

                if (jobFromOutsides != null)
                {
                    foreach (JobFromOutside jobFromOutside in jobFromOutsides)
                    {
                        if (HasDateOfTheWeek(jobFromOutside, startDayOfTheWeek))
                        {
                            for (int day = allPossbileDays.Count - 1; day >= 0; day--)
                            {
                                if (JobsHaveSaveDayDateOfTheWeek<IJobBase>(jobFromOutside, jobs[allPossbileDays[day]], startDayOfTheWeek))
                                {
                                    foreach (int empID in jobFromOutside.EmployeeAssigments)
                                    {
                                        if (employees[emp].Id == empID)
                                        {
                                            allPossbileDays.Remove(allPossbileDays[day]);

                                            mc--;
                                            goto nextDayOutOfSchedule;
                                        }
                                    }
                                }
                            }

                            for (int i = 0; i < daysOfTheWeekWhereEmployeeAlreadyWorksOutsideOfTheView.Count; i++)
                            {
                                if(jobFromOutside.JobStartDate <= daysOfTheWeekWhereEmployeeAlreadyWorksOutsideOfTheView[i] &&
                                    jobFromOutside.JobEndDate >= daysOfTheWeekWhereEmployeeAlreadyWorksOutsideOfTheView[i])
                                {
                                    goto nextDayOutOfSchedule;
                                }
                            }

                            mc--;
                            daysOfTheWeekWhereEmployeeAlreadyWorksOutsideOfTheView.Add(FindDayOfTheWeek(jobFromOutside, startDayOfTheWeek));

                        }
                    nextDayOutOfSchedule:;
                    }
                }
                Console.WriteLine(  );
                // TODO optimize one pass through the list
                for (int day = 0; day < allPossbileDays.Count; day++)
                {
                    if (onSameDay.Count == 0)
                    {
                        if (IsCompared(constraint, jobs[allPossbileDays[day]].JobCustomData, employees[emp], jobs[allPossbileDays[day]]))
                        {
                            onSameDay.Add(matrix[emp, allPossbileDays[day]]);
                        }

                    }
                    else
                    {
                        if (IsCompared(constraint, jobs[allPossbileDays[day]].JobCustomData, employees[emp], jobs[allPossbileDays[day]]))
                        {
                            onSameDay.Add(matrix[emp, allPossbileDays[day]]);
                        }

                        
                    }
                    if (day == allPossbileDays.Count - 1 || !JobsHaveSaveDayDateOfTheWeek(jobs[allPossbileDays[day]], jobs[allPossbileDays[day+1]], startDayOfTheWeek))//jobs[allPossbileDays[day]].JobStartDate.DayOfYear != jobs[allPossbileDays[day + 1]].JobStartDate.DayOfYear
                    {
                        if (onSameDay.Count > 0)
                        {                            
                            BoolVar currentDay = model.NewBoolVar("nest0 " + allPossbileDays[day] + " " + emp);                            
                            model.Add(LinearExpr.Sum(onSameDay) == 0).OnlyEnforceIf(currentDay.Not());
                            daysOfTheWeek.Add(currentDay);
                            onSameDay = new List<ILiteral>();
                        }
                    }
                }
                if (isCustom)
                {
                    if (mc >= 1) 
                    {
                        model.Add(LinearExpr.Sum(daysOfTheWeek) <= (mc)).OnlyEnforceIf(matrix[emp, indexOfTargetedJob]);
                    }
                    else
                    {
                        model.Add(LinearExpr.Sum(daysOfTheWeek) == 0);
                    } 
                }
                else
                {
                    if (mc >= 1)
                    {
                        model.Add(LinearExpr.Sum(daysOfTheWeek) <= (mc));
                    }
                    else
                    {
                        model.Add(LinearExpr.Sum(daysOfTheWeek) <= 0);
                    }
                }
            }
        }

        private static DateTime FindDayOfTheWeek(JobFromOutside jobFromOutside, DayOfWeek startDayOfTheWeek)
        {
            int i = 0;
            while(true)
            {
                if (jobFromOutside.JobStartDate.AddDays(i).DayOfWeek == startDayOfTheWeek)
                {
                    return jobFromOutside.JobStartDate.AddDays(i).Date;
                }
                i++;
            }
        }

        private static bool JobsHaveSaveDayDateOfTheWeek<T>(T scheduleJob1, T scheduleJob2, DayOfWeek dayOfWeek) where T : IJobBase
        {
            int i = 0;
            DateTime firstDayDateOfTheWeek = DateTime.Today;
            DateTime secondDayDateOfTheWeek = DateTime.Today; // Any date initialize
            while(scheduleJob1.JobStartDate.AddDays(i).Date <= scheduleJob1.JobEndDate.Date)
            {
                if(scheduleJob1.JobStartDate.AddDays(i).Date.DayOfWeek == dayOfWeek)
                {
                    firstDayDateOfTheWeek = scheduleJob1.JobStartDate.AddDays(i).Date;
                    break;
                }
                i++;
            }

            int j = 0;
            while (scheduleJob2.JobStartDate.AddDays(j).Date <= scheduleJob2.JobEndDate.Date)
            {
                if (scheduleJob2.JobStartDate.AddDays(j).Date.DayOfWeek == dayOfWeek)
                {
                    secondDayDateOfTheWeek = scheduleJob2.JobStartDate.AddDays(j).Date;
                    break;
                }
                j++;
            }

            return firstDayDateOfTheWeek == secondDayDateOfTheWeek;
        }

        static void AddConstraintOfCurrentDayCustom(IntVar[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, List<int> list, int limit, String comparationMode, String comparationValue, String employeeField, ScheduleCustomData customData, String operatoion, List<ScheduleCustomData> scheduleCustomData, ScheduleActiveConstraint constraint, List<JobFromOutside> jobsForSolvers, DateRangeManager dateRangeManger, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek, int targetIndexJob)
        {
            /*
            if (list.Count == 0)
            {
                return;
            }

            ConstraintMNODOTW constraintMNODOTW = constraint.ConstraintMNODOTW;

            for (int emp = 0; emp < employees.Count; emp++)
            {
                IntVar[] tmp = new IntVar[list.Count];
                int firstLeaveDate = 0;
                for (int jl = 0; jl < list.Count; jl++)
                {
                    if (IsCompared(constraint, scheduleCustomData, employees[emp], jobs[jl]))
                    {
                        tmp[firstLeaveDate] = matrix[emp, list[jl]];
                        firstLeaveDate++;
                    }
                }
                int daysOfWeekPutOfSchedule = 0;
                if (jobsForSolvers != null)
                {
                    if (jobFromOutsides != null && onSameDayRelativeID != null)
                    {
                        foreach (JobFromOutside jobFromOutside in jobFromOutsides)
                        {
                            if (jobFromOutside.JobStartDate.DayOfYear == jobs[onSameDayRelativeID[onSameDayRelativeID.Count - 1]].JobStartDate.DayOfYear)
                            {
                                if (DateTime.Compare(dateRangeManger.GenerateDateRange(constraintMNODOTW, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0], dateRangeManger.GenerateDateRange(constraintMNODOTW, jobFromOutside.JobStartDate, jobFromOutside.JobEndDate.AddHours(jobFromOutside.Hours1.Hours), payPeriodDTO, startDayOfTheWeek)[0]) == 0)
                                {
                                    foreach (int empID in jobFromOutside.EmployeeAssigments)
                                    {
                                    }
                                }
                            }
                        }
                    }
                    
                    foreach (JobsOutside jobsForSolver in jobsForSolvers)
                    {
                        if (jobsForSolver.DayOfWeek == constraintMNODOTW.DayOfWeek)
                        {
                            if ((DateTime.Compare(dateRangeManger.GenerateDateRange(constraintMNODOTW, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0], jobsForSolver.JobStartDate) <= 0) &&
                                (DateTime.Compare(dateRangeManger.GenerateDateRange(constraintMNODOTW, jobs[list[0]].JobStartDate, jobs[list[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[1], jobsForSolver.JobStartDate) >= 0))
                            {
                                foreach (EmployeeAssigment employeeAssigment in employeeAssigments)
                                {
                                    if (jobsForSolver.Id == employeeAssigment.JobID)
                                    {
                                        if (employees[emp].Id == employeeAssigment.EmployeeID)
                                        {
                                            daysOfWeekPutOfSchedule++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (daysOfWeekPutOfSchedule <= limit)
                {
                    model.Add(LinearExpr.Sum(tmp) <= (limit - daysOfWeekPutOfSchedule)).OnlyEnforceIf(matrix[emp, targetIndexJob]); ;
                }
                else
                {
                    model.Add(LinearExpr.Sum(tmp) == 0);
                }
            }*/
        }

        private static void AddConstraintOfCurrentDayCustom(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, List<int> listOfJobsOfSameTypeDay, ScheduleActiveConstraint constraint, List<ScheduleCustomData> scheduleCustomData, List<JobFromOutside> jobFromOutsides, DateRangeManager dateRangeManger, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek, int jTarget)
        {
            if (listOfJobsOfSameTypeDay.Count == 0)
            {
                return;
            }

            ConstraintMNODOTW constraintMNODOTW = constraint.ConstraintMNODOTW;

            for (int emp = 0; emp < employees.Count; emp++)
            {
                ILiteral[] tmp = new ILiteral[listOfJobsOfSameTypeDay.Count];
                int firstLeaveDate = 0;
                for (int jl = 0; jl < listOfJobsOfSameTypeDay.Count; jl++)
                {
                    if (IsCompared(constraint, scheduleCustomData, employees[emp], jobs[listOfJobsOfSameTypeDay[jl]]))
                    {
                        tmp[firstLeaveDate] = matrix[emp, listOfJobsOfSameTypeDay[jl]];
                        firstLeaveDate++;
                    }
                }
                int daysOfWeekPutOfSchedule = 0;
                int limit = constraintMNODOTW.MaxCount;

                if (jobFromOutsides != null) //&& onSameDayRelativeID != null
                {
                    foreach (JobFromOutside jobFromOutside in jobFromOutsides)
                    {
                        if (jobFromOutside.JobStartDate.DayOfYear == jobs[jTarget].JobStartDate.DayOfYear)
                        {
                            if (DateTime.Compare(dateRangeManger.GenerateDateRange(constraintMNODOTW, jobs[listOfJobsOfSameTypeDay[0]].JobStartDate, jobs[listOfJobsOfSameTypeDay[0]].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0], dateRangeManger.GenerateDateRange(constraintMNODOTW, jobFromOutside.JobStartDate, jobFromOutside.JobEndDate.AddHours(jobFromOutside.HoursT.TotalHours), payPeriodDTO, startDayOfTheWeek)[0]) == 0)
                            {
                                foreach (int empID in jobFromOutside.EmployeeAssigments)
                                {
                                    if (employees[emp].Id == empID)
                                    {
                                        limit--;
                                    }
                                }
                            }
                        }
                    }
                }

                //Console.WriteLine("Limit "+limit);
                if (daysOfWeekPutOfSchedule <= limit)
                {
                    model.Add(LinearExpr.Sum(tmp) <= (limit - daysOfWeekPutOfSchedule)).OnlyEnforceIf(matrix[emp, jTarget]);
                }
                else
                {
                    model.Add(LinearExpr.Sum(tmp) == 0);
                }
            }
        }

        static Employee FindIndexOfEmployeeWithID(int id, List<Employee> employees)
        {
            for (int i = 0; i < employees.Count; i++)
            {
                if (employees[i].Id == id)
                {
                    return employees[i];
                }
            }
            return null;
        }

    }
}