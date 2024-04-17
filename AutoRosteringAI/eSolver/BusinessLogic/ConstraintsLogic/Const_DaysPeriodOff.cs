using System;
using System.Collections.Generic;
using System.Text;
using eSolver.Entities.Constraints;
using eSolver.Entities.Constraints.Untiles;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using Google.OrTools.Sat;
using static eSolver.BusinessLogic.Utiles.Compared;
using eSolver.BusinessLogic.Managers;

namespace eSolver.BusinessLogic.ConstraintsLogic
{
    public static class Const_DaysPeriodOff
    {
        /// <summary>
        /// /// 3.1.14 Days off within a period
        /// 
        /// This constraint must check the amount of rest days (days off) in a given period and 
        /// if scheduling the employee on the selected job would result in this being breached then they should not be available to schedule
        ///
        /// 
        /// [Fields: Start Day]
        /// 
        /// Choice of the days of the week.
        /// If populated the day off must be this day of the week and in the case of the ‘amount’ field being a value greater 1 
        /// and the ‘must be consecutive’ being ‘Y’ then the run of days off must start of this day.
        /// 
        /// [Fields: No of rest days]
        /// 
        /// What is the minimum amount of rest days required within the period being assessed.
        /// 
        /// [Fields: Must be consecutive]
        /// 
        /// Where there is more than one day required, must the days be consecutive
        ///
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="model"></param>
        /// <param name="employees"></param>
        /// <param name="jobs"></param>
        /// <param name="constraintDOWAP"></param>
        /// <param name="jobsNonSorted"></param>
        /// <param name="payPeriodDTO"></param>
        /// <param name="jobsFromOutsides"></param>
        /// <param name="startDayOfTheWeek"></param>
        public static void ApplyDaysOffWithinAPeriod(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, ConstraintDOWP constraintDOWAP, PayPeriodDTO payPeriodDTO, List<JobFromOutside> jobsFromOutsides, string startDayOfTheWeek)
        {
            int numEmp = 0;
            int numJobsInInterval = 0;
            DateRangeManager dateRangeManger = new DateRangeManager();


            for (int emp = 0; emp < employees.Count; emp++)
            {
                // TODO change vecotor to list - it may speed up solver
                List<ILiteral> currentDay = new List<ILiteral>();

                List<ILiteral> daysOff = new List<ILiteral>();  // IntVar[jobs.Count];
                List<DayOfWeek> daysOffTheWeek = new List<DayOfWeek>();
                List<DateTime> dateTimes = new List<DateTime>();
                List<DateTime> daysOnWhichEmployeeWorksOnOtherSchedule = new List<DateTime>();

                if (constraintDOWAP.ConstraintCustomRange == null)
                {
                    for (int j = 0; j < jobs.Count; j++)
                    {
                        DateTime[] dateRange = dateRangeManger.GenerateDateRange(constraintDOWAP, jobs[j].JobStartDate, jobs[j].JobEndDate, payPeriodDTO, startDayOfTheWeek);
                        if (dateRange != null)
                        {
                            int daysWithinIntereval = dateRange[1].DayOfYear - dateRange[0].DayOfYear + 1;
                            int? restDaysUpdated = UpdateConstrainsRestPeriod(constraintDOWAP, dateRange);
                            if (IsCompared(constraintDOWAP, employees[emp], jobs[j]))
                            {
                                // Check If it's last job or the beginning of the new day
                                // If there is, then implement constaint in the model, which says that if there is at least one job in the range, then 
                                // In there is not then that means that the following job is in the same day as last one and it sould've been added to list
                                if ((j == jobs.Count - 1) || jobs[j + 1].JobStartDate.DayOfYear != jobs[j].JobStartDate.DayOfYear)
                                {

                                    currentDay.Add((ILiteral)(matrix[numEmp, j]));

                                    // Check if there is existing job outside of the range
                                    foreach (JobFromOutside jobFO in jobsFromOutsides)
                                    {
                                        if (jobFO.JobStartDate.DayOfYear == jobs[j].JobStartDate.DayOfYear && IsCompared(constraintDOWAP, employees[emp], jobFO))
                                        {
                                            foreach (int id in jobFO.EmployeeAssigments)
                                            {
                                                if (id == employees[emp].Id)
                                                {
                                                    daysOnWhichEmployeeWorksOnOtherSchedule.Add(jobFO.JobStartDate.Date);

                                                    // If there is job one that specific day, then we know that employee aleady works on him and it sould've been skipped
                                                    goto ignoreInterval;
                                                }
                                            }
                                        }
                                    }

                                    // Definitoin of : employee works on specific day
                                    daysOff.Add(model.NewBoolVar("day" + j));
                                    daysOffTheWeek.Add(jobs[j].JobStartDate.DayOfWeek);
                                    dateTimes.Add(jobs[j].JobStartDate.Date);

                                    // Employee does not work on specific day, if the sum of all jobs on the following day is 0
                                    model.Add(LinearExpr.Sum(currentDay) == 0).OnlyEnforceIf(daysOff[daysOff.Count - 1]);
                                ignoreInterval:

                                    // Bumber of days that emoloyee can work or work
                                    numJobsInInterval++;

                                    // Reset the definition of the day, bcz next day will have start day at next date
                                    currentDay = new List<ILiteral>();
                                }
                                else
                                {
                                    // Add following day to list
                                    currentDay.Add(matrix[numEmp, j]);
                                }
                            }

                            // Apply constraint to model and reset all set-up for the next employee
                            if (UpdateModelDaysOffPeriod(dateRange, jobs, j, payPeriodDTO, employees[emp], model, dateRangeManger, constraintDOWAP, daysOff, startDayOfTheWeek, jobsFromOutsides, daysWithinIntereval, numJobsInInterval, dateTimes, matrix, emp, restDaysUpdated))
                            {
                                //daysOff = new IntVar[jobs.Count];
                                daysOff = new List<ILiteral>();
                                daysOffTheWeek = new List<DayOfWeek>();
                                dateTimes = new List<DateTime>();

                                currentDay = new List<ILiteral>();
                                numJobsInInterval = 0;
                            }
                        }

                    }
                }
                else
                {
                    for (int fixedIndexOfjob = 0; fixedIndexOfjob < jobs.Count; fixedIndexOfjob++)
                    {
                        if (IsCompared(constraintDOWAP, employees[emp], jobs[fixedIndexOfjob]))
                        {
                            DateTime[] dateRange = dateRangeManger.GenerateDateRange(constraintDOWAP, jobs[fixedIndexOfjob].JobStartDate, jobs[fixedIndexOfjob].JobEndDate, payPeriodDTO, startDayOfTheWeek);

                            if (dateRange != null && IsCompared(constraintDOWAP, employees[emp], jobs[fixedIndexOfjob]))
                            {
                                int? restDaysUpdated = UpdateConstrainsRestPeriod(constraintDOWAP, dateRange);

                                int beginingJob = fixedIndexOfjob;

                                // Lower limit
                                while (beginingJob >= 0 && jobs[beginingJob].JobStartDate >= dateRange[0])
                                {
                                    beginingJob--;
                                }
                                int endingJob = fixedIndexOfjob;

                                // Uper limit
                                while (endingJob < jobs.Count && jobs[endingJob].JobStartDate <= dateRange[1])
                                {
                                    endingJob++;
                                }

                                for (int i = beginingJob + 1; i < endingJob; i++)
                                {
                                    if ((i == jobs.Count - 1) || jobs[i + 1].JobStartDate.DayOfYear != jobs[i].JobStartDate.DayOfYear)
                                    {

                                        currentDay.Add(matrix[numEmp, i]);

                                        foreach (JobFromOutside jobFO in jobsFromOutsides)
                                        {
                                            if (jobFO.JobStartDate.DayOfYear == jobs[i].JobStartDate.DayOfYear && IsCompared(constraintDOWAP, employees[emp], jobFO))
                                            {
                                                foreach (int id in jobFO.EmployeeAssigments)
                                                {
                                                    if (id == employees[emp].Id)
                                                    {
                                                        daysOnWhichEmployeeWorksOnOtherSchedule.Add(jobFO.JobStartDate.Date);
                                                        goto ignoreInterval;
                                                    }
                                                }
                                            }
                                        }

                                        daysOff.Add(model.NewBoolVar("dayc" + i));
                                        daysOffTheWeek.Add(jobs[i].JobStartDate.DayOfWeek);
                                        dateTimes.Add(jobs[i].JobStartDate.Date);

                                        model.Add(LinearExpr.Sum(currentDay) == 0).OnlyEnforceIf(daysOff[daysOff.Count - 1]);


                                    ignoreInterval:
                                        numJobsInInterval++;
                                        currentDay = new List<ILiteral>();
                                    }
                                    else
                                    {
                                        currentDay.Add(matrix[numEmp, i]);
                                    }
                                }
                                //if (UpdateModelDaysOffPeriodCustom(dateRange, jobs, fixedIndexOfjob, payPeriodDTO, employees[emp], model, dateRangeManger, constraintDOWAP, daysOff, startDayOfTheWeek, jobsFromOutsides, jobs[fixedIndexOfjob].JobCustomData, endingJob - beginingJob - 1, numJobsInInterval, daysOffTheWeek, dateTimes, matrix[emp, fixedIndexOfjob], matrix, emp, daysOnWhichEmployeeWorksOnOtherSchedule, restDaysUpdated))
                                if (UpdateModelDaysOffPeriodCustom(dateRange, jobs, fixedIndexOfjob, payPeriodDTO, employees[emp], model, dateRangeManger, constraintDOWAP, daysOff, startDayOfTheWeek, jobsFromOutsides, constraintDOWAP.ConstraintCustomRange.Amount, numJobsInInterval, dateTimes, matrix[emp, fixedIndexOfjob], matrix, emp, restDaysUpdated))
                                {
                                    //daysOff = new IntVar[jobs.Count];
                                    daysOff = new List<ILiteral>();
                                    daysOffTheWeek = new List<DayOfWeek>();
                                    dateTimes = new List<DateTime>();

                                    currentDay = new List<ILiteral>();
                                    numJobsInInterval = 0;
                                }
                            }
                        }
                    }
                }
                numEmp++;
            }
        }

        private static int? UpdateConstrainsRestPeriod(ConstraintDOWP constraintDOWAP, DateTime[] dateRange)
        {
            DayOfWeek targetStartDay = new DayOfWeek();
            int daysWithinIntereval = dateRange[1].DayOfYear - dateRange[0].DayOfYear;

            if (constraintDOWAP.MustBeConsecutive) 
            {
                if (String.IsNullOrEmpty(constraintDOWAP.StartDay))
                {

                    return constraintDOWAP.NumberOfRestDays > daysWithinIntereval ? daysWithinIntereval : constraintDOWAP.NumberOfRestDays;

                }
                else
                {
                    switch (constraintDOWAP.StartDay)
                    {
                        case "Monday":
                            targetStartDay = DayOfWeek.Monday;
                            break;
                        case "Tuesday":
                            targetStartDay = DayOfWeek.Tuesday;
                            break;
                        case "Wednesday":
                            targetStartDay = DayOfWeek.Wednesday;
                            break;
                        case "Thursday":
                            targetStartDay = DayOfWeek.Thursday;
                            break;
                        case "Friday":
                            targetStartDay = DayOfWeek.Friday;
                            break;
                        case "Saturday":
                            targetStartDay = DayOfWeek.Saturday;
                            break;
                        case "Sunday":
                            targetStartDay = DayOfWeek.Sunday;
                            break;
                    }

                    int maxDays = dateRange[1].DayOfYear - dateRange[0].DayOfYear + 1;
                    int daysTOAdjsutConstraint = dateRange[1].DayOfYear - dateRange[0].DayOfYear + 1;

                    for (int i = 0; i < maxDays; i++)
                    {
                        if(dateRange[0].AddDays(i).DayOfWeek != targetStartDay)
                        {
                            daysTOAdjsutConstraint--;
                        }
                        else
                        {
                            break;
                        }
                    }

                    return constraintDOWAP.NumberOfRestDays > daysTOAdjsutConstraint ? daysTOAdjsutConstraint : constraintDOWAP.NumberOfRestDays;

                }
            }
            else
            {
                return constraintDOWAP.NumberOfRestDays > daysWithinIntereval ? daysWithinIntereval : constraintDOWAP.NumberOfRestDays;
            }
        }

        private static bool UpdateModelDaysOffPeriodCustom(DateTime[] dateRange, List<ScheduleJob> jobs, int j, PayPeriodDTO payPeriodDTO, Employee employee, CpModel model, DateRangeManager dateRangeManger, ConstraintDOWP constraintDOWAP, List<ILiteral> daysOff, string startDayOfTheWeek, List<JobFromOutside> jobsFromOutsides, int daysWithinIntereval, int numJobsInInterval, List<DateTime> dateTimes, ILiteral jobCustomTarget, ILiteral[,] matrix, int emp, int? restDaysUpdated)
        {

            bool resetCondition = false;
            if (dateRangeManger.GenerateDateRange(constraintDOWAP, jobs[j].JobStartDate, jobs[j].JobEndDate, payPeriodDTO, startDayOfTheWeek) != null && dateRange[0] != null)
            {
                if ((j == jobs.Count - 1) ||
                    (dateRangeManger.GenerateDateRange(constraintDOWAP, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek) == null) ||
                    (dateRange[0].DayOfYear != dateRangeManger.GenerateDateRange(constraintDOWAP, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0].DayOfYear)) // 06/08/2020 != 0
                {
                    if (constraintDOWAP.NumberOfRestDays != null)
                    {
                        if (constraintDOWAP.MustBeConsecutive)
                        {
                            if (String.IsNullOrEmpty(constraintDOWAP.StartDay))
                            {
                                if (dateTimes.Count > 0 && dateTimes[dateTimes.Count - 1].AddDays((double)constraintDOWAP.NumberOfRestDays) >= dateRange[1] && dateTimes[0].AddDays(-(double)constraintDOWAP.NumberOfRestDays) < dateRange[0])
                                {
                                    List<ILiteral> pickerDayOff = new List<ILiteral>();

                                    for (int i = 0; i < dateRange[1].DayOfYear - dateRange[0].AddSeconds(1).DayOfYear - constraintDOWAP.NumberOfRestDays + 1; i++)
                                    {
                                        List<ILiteral> daysConsecutive = new List<ILiteral>();
                                        for (int rest = 0; rest < restDaysUpdated; rest++)
                                        {
                                            foreach (JobFromOutside jobFO in jobsFromOutsides)
                                            {
                                                if (jobFO.JobStartDate.DayOfYear == dateRange[0].AddDays(i + rest).DayOfYear && IsCompared(constraintDOWAP, employee, jobFO))
                                                {
                                                    foreach (int id in jobFO.EmployeeAssigments)
                                                    {
                                                        if (id == employee.Id)
                                                        {
                                                            goto jumpNextPossibleCandidatForRestDaysIntervalCustomConecutive;
                                                        }
                                                    }
                                                }
                                            }

                                            bool jobExistInSchedule = false;
                                            for (int d = 0; d < dateTimes.Count; d++)
                                            {
                                                if (dateTimes[d].Date == dateRange[0].AddDays(i + rest).Date)
                                                {
                                                    daysConsecutive.Add((ILiteral)(daysOff[d]));
                                                    jobExistInSchedule = true;
                                                }

                                                if (jobExistInSchedule)
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        if (daysConsecutive.Count == 0)
                                        {
                                            return true;
                                        }

                                        pickerDayOff.Add(model.NewBoolVar("dayoff " + i));
                                        model.Add(LinearExpr.Sum(daysConsecutive) == daysConsecutive.Count).OnlyEnforceIf(pickerDayOff[pickerDayOff.Count - 1]);

                                    jumpNextPossibleCandidatForRestDaysIntervalCustomConecutive:;
                                    }

                                    model.Add(LinearExpr.Sum(pickerDayOff) == 1).OnlyEnforceIf(jobCustomTarget); ;
                                }
                            }
                            else
                            {

                                List<DateTime[]> listOfPossibleOffIntervals = ExtractPossiblePeriodsOff(restDaysUpdated, constraintDOWAP.StartDay, dateRange);

                                if (FillterFromOtherSchedule(listOfPossibleOffIntervals, jobsFromOutsides, jobs, constraintDOWAP, employee, matrix, model, emp))
                                {
                                    if (listOfPossibleOffIntervals.Count != 0)
                                    {
                                        List<ILiteral> pickerDayOff = new List<ILiteral>();
                                        for (int i = 0; i < listOfPossibleOffIntervals.Count; i++)
                                        {
                                            List<ILiteral> daysConsecutive = new List<ILiteral>();
                                            for (int rest = 0; rest < restDaysUpdated; rest++)
                                            {

                                                foreach (JobFromOutside jobFO in jobsFromOutsides)
                                                {
                                                    if (jobFO.JobStartDate.DayOfYear == dateRange[0].AddDays(i + rest).DayOfYear && IsCompared(constraintDOWAP, employee, jobFO))
                                                    {
                                                        foreach (int id in jobFO.EmployeeAssigments)
                                                        {
                                                            if (id == employee.Id)
                                                            {
                                                                goto jumpNextPossibleCandidatForRestDaysIntervalCustomNoneConecutive;
                                                            }
                                                        }
                                                    }
                                                }

                                                bool jobExistInSchedule = false;
                                                for (int d = 0; d < dateTimes.Count; d++)
                                                {
                                                    if (dateTimes[d].Date == listOfPossibleOffIntervals[i][0].AddDays(rest).Date)
                                                    {
                                                        daysConsecutive.Add(daysOff[d]);
                                                        jobExistInSchedule = true;
                                                    }

                                                    if (jobExistInSchedule)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }

                                            if (daysConsecutive.Count == 0)
                                            {
                                                return true;
                                            }

                                            pickerDayOff.Add(model.NewBoolVar("dayoff " + i + " " + emp));
                                            model.Add(LinearExpr.Sum(daysConsecutive) == daysConsecutive.Count).OnlyEnforceIf(pickerDayOff[pickerDayOff.Count - 1]); ;

                                        jumpNextPossibleCandidatForRestDaysIntervalCustomNoneConecutive:;
                                        }

                                        model.Add(LinearExpr.Sum(pickerDayOff) == 1).OnlyEnforceIf(jobCustomTarget); ;
                                    }
                                }
                            }
                        }
                        else
                        {
                            model.Add(LinearExpr.Sum(daysOff) + (daysWithinIntereval - numJobsInInterval) >= (int)restDaysUpdated).OnlyEnforceIf(jobCustomTarget);
                        }
                    }
                dontapply:
                    resetCondition = true;
                }
            }

            return resetCondition;
        }

        private static bool UpdateModelDaysOffPeriod(DateTime[] dateRange, List<ScheduleJob> jobs, int j, PayPeriodDTO payPeriodDTO, Employee employee, CpModel model, DateRangeManager dateRangeManger, ConstraintDOWP constraintDOWAP, List<ILiteral> daysOff, string startDayOfTheWeek, List<JobFromOutside> jobsFromOutsides, int daysWithinIntereval, int numJobsInInterval, List<DateTime> dateTimes, ILiteral[,] matrix, int emp, int? restDaysUpdated)
        {
            bool resetCondition = false;
            if (dateRangeManger.GenerateDateRange(constraintDOWAP, jobs[j].JobStartDate, jobs[j].JobEndDate, payPeriodDTO, startDayOfTheWeek) != null && dateRange[0] != null)
            {
                if ((j == jobs.Count - 1) ||
                    (dateRangeManger.GenerateDateRange(constraintDOWAP, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek) == null) ||
                    (dateRange[0].DayOfYear != dateRangeManger.GenerateDateRange(constraintDOWAP, jobs[j + 1].JobStartDate, jobs[j + 1].JobEndDate, payPeriodDTO, startDayOfTheWeek)[0].DayOfYear)) // 06/08/2020 != 0
                {
                    if (constraintDOWAP.NumberOfRestDays != null)
                    {
                        if (constraintDOWAP.MustBeConsecutive)
                        {
                            if (String.IsNullOrEmpty(constraintDOWAP.StartDay))
                            {
                                if (dateTimes.Count > 0 && dateTimes[dateTimes.Count - 1].AddDays((double)constraintDOWAP.NumberOfRestDays) >= dateRange[1] && dateTimes[0].AddDays(-(double)constraintDOWAP.NumberOfRestDays) < dateRange[0])
                                {
                                    List<ILiteral> pickerDayOff = new List<ILiteral>();

                                    Console.WriteLine(" DP " + (dateRange[1].DayOfYear - dateRange[0].AddSeconds(1).DayOfYear - constraintDOWAP.NumberOfRestDays + 1));
                                    for (int i = 0; i < dateRange[1].DayOfYear - dateRange[0].AddSeconds(1).DayOfYear - constraintDOWAP.NumberOfRestDays + 2; i++)
                                    {
                                        List<ILiteral> daysConsecutive = new List<ILiteral>();
                                        for (int rest = 0; rest < constraintDOWAP.NumberOfRestDays; rest++)
                                        {

                                            foreach (JobFromOutside jobFO in jobsFromOutsides)
                                            {
                                                if (jobFO.JobStartDate.DayOfYear == dateRange[0].AddDays(i + rest).DayOfYear && IsCompared(constraintDOWAP, employee, jobFO))
                                                {
                                                    foreach (int id in jobFO.EmployeeAssigments)
                                                    {
                                                        if (id == employee.Id)
                                                        {
                                                            goto jumpNextPossibleCandidatForRestDaysIntervalNoneCustomConsecutive;
                                                        }
                                                    }
                                                }
                                            }

                                            bool jobExistInSchedule = false;
                                            for (int d = 0; d < dateTimes.Count; d++)
                                            {
                                                if (dateTimes[d].Date == dateRange[0].AddDays(i + rest).Date)
                                                {
                                                    daysConsecutive.Add(daysOff[d]);
                                                    jobExistInSchedule = true;
                                                }

                                                if(jobExistInSchedule)
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        if(daysConsecutive.Count == 0)
                                        {
                                            return true;
                                        }

                                        pickerDayOff.Add(model.NewBoolVar("dayoff " + i));
                                        model.Add(LinearExpr.Sum(daysConsecutive) == daysConsecutive.Count).OnlyEnforceIf(pickerDayOff[pickerDayOff.Count - 1]);

                                        jumpNextPossibleCandidatForRestDaysIntervalNoneCustomConsecutive:;
                                    }

                                    model.Add(LinearExpr.Sum(pickerDayOff) == 1);
                                }
                            }
                            else
                            {

                                List<DateTime[]> listOfPossibleOffIntervals = ExtractPossiblePeriodsOff(restDaysUpdated, constraintDOWAP.StartDay, dateRange);

                                if (FillterFromOtherSchedule(listOfPossibleOffIntervals, jobsFromOutsides, jobs, constraintDOWAP, employee, matrix, model, emp))
                                {
                                    if (listOfPossibleOffIntervals.Count != 0)
                                    {
                                        List<ILiteral> pickerDayOff = new List<ILiteral>();
                                        for (int i = 0; i < listOfPossibleOffIntervals.Count; i++)
                                        {
                                            List<ILiteral> daysConsecutive = new List<ILiteral>();
                                            for (int rest = 0; rest < restDaysUpdated; rest++)
                                            {
                                                foreach (JobFromOutside jobFO in jobsFromOutsides)
                                                {
                                                    if (jobFO.JobStartDate.DayOfYear == dateRange[0].AddDays(i + rest).DayOfYear && IsCompared(constraintDOWAP, employee, jobFO))
                                                    {
                                                        foreach (int id in jobFO.EmployeeAssigments)
                                                        {
                                                            if (id == employee.Id)
                                                            {
                                                                goto jumpNextPossibleCandidatForRestDaysIntervalNoneCustomNoneConsecutive;
                                                            }
                                                        }
                                                    }
                                                }

                                                bool jobExistInSchedule = false;
                                                for (int d = 0; d < dateTimes.Count; d++)
                                                {
                                                    if (dateTimes[d].Date == listOfPossibleOffIntervals[i][0].AddDays(rest).Date)
                                                    {
                                                        daysConsecutive.Add(daysOff[d]);
                                                        jobExistInSchedule = true;
                                                    }

                                                    if (jobExistInSchedule)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }

                                            if (daysConsecutive.Count == 0)
                                            {
                                                return true;
                                            }

                                            pickerDayOff.Add(model.NewBoolVar("dayoff " + i + " " + emp));
                                            model.Add(LinearExpr.Sum(daysConsecutive) == daysConsecutive.Count).OnlyEnforceIf(pickerDayOff[pickerDayOff.Count - 1]);

                                        jumpNextPossibleCandidatForRestDaysIntervalNoneCustomNoneConsecutive:;
                                        }

                                        model.Add(LinearExpr.Sum(pickerDayOff) >= 1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            int daysWorkingOnTheOtherScheduleOutsideOfTheRange = 0;
                            if (j == jobs.Count - 1)
                            {
                                for (int i = jobs[j].JobStartDate.DayOfYear + 1; i != (dateRange[1].DayOfYear + 1); i++)
                                {
                                    foreach (JobFromOutside jobFO in jobsFromOutsides)
                                    {
                                        // dateRange[1].AddDays(i).DayOfYear
                                        if ((jobFO.JobStartDate.DayOfYear == i) && IsCompared(constraintDOWAP, employee, jobFO))
                                        {
                                            foreach (int id in jobFO.EmployeeAssigments)
                                            {
                                                if (id == employee.Id)
                                                {
                                                    daysWorkingOnTheOtherScheduleOutsideOfTheRange++;
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            model.Add(LinearExpr.Sum(daysOff) + (daysWithinIntereval - numJobsInInterval - daysWorkingOnTheOtherScheduleOutsideOfTheRange) >= (int)restDaysUpdated);
                        }
                    }
                dontapply:
                    resetCondition = true;
                }
            }

            return resetCondition;
        }

        private static bool FillterFromOtherSchedule(List<DateTime[]> listOfPossibleOffIntervals, List<JobFromOutside> jobsFromOutsides, List<ScheduleJob> jobs, ConstraintDOWP constraintDOWAP, Employee employee, ILiteral[,] matrix, CpModel model, int emp)
        {
            for (int i = listOfPossibleOffIntervals.Count - 1; i >= 0; i--)
            {

                if (listOfPossibleOffIntervals.Count != 0)
                {
                    foreach (ScheduleJob job in jobs)
                    {
                        if (IsCompared(constraintDOWAP, employee, job))
                        {
                            if (job.JobStartDate >= listOfPossibleOffIntervals[i][0] && job.JobStartDate <= listOfPossibleOffIntervals[i][1].AddDays(1))
                            {
                                goto nextCheck;
                            }
                        }
                    }
                }
                return false;

            nextCheck:
                foreach (JobFromOutside job in jobsFromOutsides)
                {
                    if (IsCompared(constraintDOWAP, employee, job))
                    {
                        if (job.JobStartDate >= listOfPossibleOffIntervals[i][0] && job.JobStartDate <= listOfPossibleOffIntervals[i][1].AddDays(1))
                        {
                            foreach (int empId in job.EmployeeAssigments)
                            {
                                if (empId == employee.Id)
                                {


                                    List<ILiteral> pickerDayOff = new List<ILiteral>();
                                    // Remove all employee's job from that interval
                                    for (int j = 0; j < jobs.Count; j++)
                                    {
                                        if (IsCompared(constraintDOWAP, employee, jobs[j]))
                                        {
                                            if (jobs[j].JobStartDate >= listOfPossibleOffIntervals[i][0] && jobs[j].JobStartDate <= listOfPossibleOffIntervals[i][1].AddDays(1))
                                            {
                                                model.Add((IntVar)matrix[emp, j] == 0);
                                            }
                                        }
                                    }
                                    listOfPossibleOffIntervals.RemoveAt(i);

                                    goto nextCheck2;
                                }
                            }
                        }
                    }
                }
            nextCheck2:;
            }
            return true;
        }

        /// <summary>
        /// FInd all possible periods within rage, while using parsed start day, from which it counts 
        /// </summary>
        /// <param name="numberOfRestDays"></param>
        /// <param name="startDay"></param>
        /// <param name="dateTimes"></param>
        /// <returns></returns>
        private static List<DateTime[]> ExtractPossiblePeriodsOff(int? numberOfRestDays, string startDay, DateTime[] dateTimes)
        {
            DayOfWeek targetStartDay = new DayOfWeek();

            switch (startDay)
            {
                case "Monday":
                    targetStartDay = DayOfWeek.Monday;
                    break;
                case "Tuesday":
                    targetStartDay = DayOfWeek.Tuesday;
                    break;
                case "Wednesday":
                    targetStartDay = DayOfWeek.Wednesday;
                    break;
                case "Thursday":
                    targetStartDay = DayOfWeek.Thursday;
                    break;
                case "Friday":
                    targetStartDay = DayOfWeek.Friday;
                    break;
                case "Saturday":
                    targetStartDay = DayOfWeek.Saturday;
                    break;
                case "Sunday":
                    targetStartDay = DayOfWeek.Sunday;
                    break;
            }

            DateTime iterateDate = dateTimes[0];

            while (iterateDate.DayOfWeek != targetStartDay)
            {
                iterateDate = iterateDate.AddDays(1);
            }

            if (iterateDate > dateTimes[1])
            {
                return new List<DateTime[]>();
            }

            List<DateTime[]> allIntervals = new List<DateTime[]>();

            while (iterateDate.AddDays((int)numberOfRestDays - 1) <= dateTimes[1]) 
            {
                DateTime[] dateTimes1 = new DateTime[2];
                dateTimes1[0] = iterateDate;
                dateTimes1[1] = iterateDate.AddDays((int)numberOfRestDays - 1);

                allIntervals.Add(dateTimes1);

                iterateDate = iterateDate.AddDays(7);
            }
            return allIntervals;

        }

    }
}
