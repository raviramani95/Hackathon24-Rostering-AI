using eSolver.Entities;
using System;
using System.Collections.Generic;
using eSolver.Entities.Constraints;
using eSolver.Entities.Interfaces;

namespace eSolver.BusinessLogic.Managers
{
    class DateRangeManager
    {
        /// <summary>
        /// Compares DateTime object <paramref name="date"/> with <paramref name="startDate"/> and <paramref name="endDate"/>.
        /// <para>If <paramref name="date"/> Ticks are grather or equal than <paramref name="startDate"/> AND smaller or equal with <paramref name="endDate"/> Ticks.
        /// Method will return <c>True</c> in all other cases return value will be <c>False</c></para>
        /// <param name="date">Represents Date which you want to check.</param>
        /// <param name="startDate">Represents start date of passed interval.</param>
        /// <param name="endDate">Represents end date of passed interval.</param>
        /// </summary>
        /// <returns><c>bool</c> Which represents is passed date inside passed date range.</returns>
        public static bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            if (startDate.Ticks < date.Ticks && endDate.Ticks > date.Ticks)
                return true;
            return false;
        }

        internal List<DateTime[]> FindAllDateRanges(List<ScheduleJob> jobs, ConstraintMNOH constraintMNOH, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek)
        {
            List<DateTime[]> dateTimes = new List<DateTime[]>();
            foreach (ScheduleJob job in jobs)
            {
                //GenerateDateRange()
            }
            return null;
        }

        /// <summary>
        /// Compares passed Date ranges.
        /// </summary>
        /// <param name="StartRange">Range Start Date.</param>
        /// <param name="EndRange">Range End Date.</param>
        /// <param name="StartInterval">Interval Start Date.</param>
        /// <param name="EndInterval">Interval End Date</param>
        /// <returns><c>bool</c> Represents is Passed range inside desired date range</returns>
        public static bool IsDateRangeInsideRange(DateTime StartRange, DateTime EndRange, DateTime StartInterval, DateTime EndInterval)
        {
            if ((StartInterval.Ticks >= StartRange.Ticks && StartInterval.Ticks <= EndRange.Ticks) && (EndInterval.Ticks >= StartRange.Ticks && EndInterval.Ticks <= EndRange.Ticks))
                return true;
            return false;
        }

        /// <summary>
        /// Method compare passed dates based on <paramref name="lessOrEqual"/>. If we pass false for <paramref name="lessOrEqual"/> method operator will be less
        /// otherwise operator will be less or equal
        /// </summary>
        /// <param name="dateTime1">DateTime object</param>
        /// <param name="dateTime2">DateTime object</param>
        /// <param name="lessOrEqual">bool param</param>
        /// <returns></returns>
        public static bool IsFirstDateBeforeSecondDate(DateTime dateTime1, DateTime dateTime2, bool lessOrEqual)
        {
            if (lessOrEqual)
                return dateTime1.Ticks <= dateTime2.Ticks;
            return dateTime1.Ticks < dateTime2.Ticks;
        }
        /// <summary>
        /// Method used for cheking if two jobs overlap 
        /// </summary>
        /// <param name="sDateOne"></param>
        /// <param name="eDateOne"></param>
        /// <param name="sDateTwo"></param>
        /// <param name="eDateTwo"></param>
        /// <returns></returns>
        public static bool DoesPeriodsOverlaps(DateTime sDateOne, DateTime eDateOne, DateTime sDateTwo, DateTime eDateTwo)
        {
            return sDateOne <= eDateTwo && sDateTwo <= eDateOne;
        }


        /// <summary>
        /// Generate job's date range for constraint's calculations 
        /// </summary>
        /// <param name="jobStartDate">Job's start date</param>
        /// <param name="jobEndDate">Job's end date</param>
        /// <param name="constraintDateRange">Object with date range type</param>
        /// <returns>Array of dates where the first element is start date of date range and the last element is end date of date range</returns>
        /// <exception cref="ArgumentOutOfRangeException">The excpetion that is thrown when the value of an argument is outside
        /// of allowable range of values as defined by invoked method.</exception>
        public DateTime[] GenerateDateRange(ITmpConstraint constraintDateRange, DateTime jobStartDate, DateTime jobEndDate, PayPeriodDTO payPeriodDTO, string nameOfFirstDayInWeek)
        {
            bool isBaseDate = constraintDateRange.ConstraintBaseDateRangeID != null;
            bool isCustom = constraintDateRange.ConstraintCustomRangeID != null;
            DateTime startDate;
            DateTime endDate;
            if (constraintDateRange.IsWeek)
            {
                startDate = CalculateFirstDateInWeek(nameOfFirstDayInWeek, jobStartDate).AddHours(-jobStartDate.Hour).AddMinutes(-jobStartDate.Minute).AddSeconds(-jobStartDate.Second); ;
                endDate = startDate.AddDays(7).AddSeconds(-1); ;
                return new DateTime[] { startDate, endDate };
            }
            else if (constraintDateRange.IsMonth)
            {
                // => startDate represents first day in month
                // => endDate last first day in month
                startDate = jobStartDate.AddDays(1 - jobStartDate.Day).AddHours(-jobStartDate.Hour).AddMinutes(-jobStartDate.Minute).AddSeconds(-jobStartDate.Second);
                endDate = startDate.AddMonths(1).AddSeconds(-1);
                return new DateTime[] { startDate, endDate };
            }
            else if (constraintDateRange.IsPayPeriod)
            {
                DateTime[] firstDateRange = GeneratePayPeriodDateRange(jobStartDate.Date, payPeriodDTO);
                if (firstDateRange == null)
                {
                    return null;
                }

                if (jobStartDate.Date != jobEndDate.Date)
                {
                    DateTime[] secondDateRange = GeneratePayPeriodDateRange(jobEndDate.Date, payPeriodDTO);
                    return new DateTime[] { firstDateRange[0], secondDateRange[1] };
                }
                return firstDateRange;
            }
            else if (isBaseDate)
            {
                if (jobStartDate.Date < constraintDateRange.ConstraintBaseDateRange.BaseDate.Date && jobEndDate.Date < constraintDateRange.ConstraintBaseDateRange.BaseDate.Date)
                {
                    return null;
                }

                DateTime[] firstDateRange = GenerateBaseDateRange(jobStartDate.Date, constraintDateRange.ConstraintBaseDateRange.BaseDate, constraintDateRange.ConstraintBaseDateRange.BaseDateRange);
                if (firstDateRange == null)
                {
                    return null;
                }
                // Ask Zorica about this code
                /*
                if (jobStartDate.Date != jobEndDate.Date)
                {
                    DateTime[] secondDateRange = GenerateBaseDateRange(jobEndDate.Date, constraintDateRange.ConstraintBaseDateRange.BaseDate, constraintDateRange.ConstraintBaseDateRange.BaseDateRange);
                    return new DateTime[] { firstDateRange[0], secondDateRange[1] };
                }*/

                return firstDateRange;
            }
            else if (constraintDateRange.ConstraintCustomRange != null)
            {
                if (constraintDateRange.ConstraintCustomRange.Offset == 0 && constraintDateRange.ConstraintCustomRange.Amount == 0)
                {
                    return new DateTime[] { jobStartDate, jobEndDate };
                }
                startDate = jobStartDate.AddDays(constraintDateRange.ConstraintCustomRange.Offset).AddHours(-jobStartDate.Hour).AddMinutes(-jobStartDate.Minute).AddSeconds(-jobStartDate.Second);
                endDate = startDate.AddDays((int)constraintDateRange.ConstraintCustomRange.Amount - 1) + new TimeSpan(23, 59, 0);
                return new DateTime[] { startDate, endDate };
            }

            // => Constraint is not applicable on passed job
            return null;
        }


        /// <summary>
        /// Method used for creating pay peeriod date range for passed date.
        /// </summary>
        /// <param name="date">Job's start or end date</param>
        /// <returns>Pay period date range based on passed date.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The excpetion that is thrown when the value of an argument is outside
        /// of allowable range of values as defined by invoked method.</exception>
        public static DateTime[] GeneratePayPeriodDateRange(DateTime date, PayPeriodDTO payPeriodDTO)
        {
            if (payPeriodDTO == null || payPeriodDTO.PayPeriodStartDate == null || date.Date < payPeriodDTO.PayPeriodStartDate.Value.Date)
            {
                return null;
            }

            if (payPeriodDTO.IsMonthlyPayPeriod)
            {
                DateTime st = payPeriodDTO.PayPeriodStartDate.Value;
                DateTime et = payPeriodDTO.PayPeriodStartDate.Value.AddDays(DateTime.DaysInMonth(payPeriodDTO.PayPeriodStartDate.Value.Year, payPeriodDTO.PayPeriodStartDate.Value.Month) - 1);
                while (date.Date > et)
                {
                    st = et.AddDays(1);
                    et = st.AddDays(DateTime.DaysInMonth(st.Year, st.Month) - 1);

                }
                return new DateTime[] { st, et };

            }

            int numberOfDays = payPeriodDTO.PayPeriodNumberOfDays.Value;
            DateTime startDate;
            DateTime endDate;
            //if job is in defualt pay period date range (from sys pref page)
            if (date.Date >= payPeriodDTO.PayPeriodStartDate.Value.Date && date.Date <= payPeriodDTO.PayPeriodStartDate.Value.Date.AddDays(numberOfDays - 1))
            {
                startDate = payPeriodDTO.PayPeriodStartDate.Value;
                endDate = startDate.AddDays(numberOfDays - 1);
                return new DateTime[] { startDate, endDate };
            }

            int difference = (int)(date - payPeriodDTO.PayPeriodStartDate.Value).TotalDays;
            int module = difference % numberOfDays;
            if (difference == module)
            {
                //if difference is same as module, job start date is pay period start date
                startDate = date.Date;

            }
            else
            {

                int diffNumberOfDaysModule = numberOfDays - module;
                if (diffNumberOfDaysModule == 1)
                {
                    int negativeDays = (-module);
                    startDate = date.AddDays(negativeDays);
                }
                else
                {
                    if (module == 0)
                    {
                        startDate = date;

                    }
                    else
                    {
                        //module++;
                        int negativeDays = (-module);
                        startDate = date.AddDays(negativeDays);
                    }
                }
            }

            endDate = startDate.AddDays(numberOfDays - 1);
            return new DateTime[] { startDate.Date, endDate.Date };
        }

        /// <summary>
        /// Generate base date range
        /// </summary>
        /// <param name="date">Job's start or end date.</param>
        /// <param name="baseDate">Base date </param>
        /// <param name="baseDateRange">Base date range</param>
        /// <returns>Base date range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The excpetion that is thrown when the value of an argument is outside
        /// of allowable range of values as defined by invoked method.</exception>
        public static DateTime[] GenerateBaseDateRange(DateTime date, DateTime baseDate, int baseDateRange)
        {
            int difference = (int)(date - baseDate).TotalDays;
            int module = difference % baseDateRange;
            DateTime startDate;
            DateTime endDate;
            if (date >= baseDate && date <= baseDate.AddDays(baseDateRange - 1))
            {
                startDate = baseDate;
                endDate = startDate.AddDays(baseDateRange - 1).Date;
                endDate += new TimeSpan(23, 59, 0);
                return new DateTime[] { startDate, endDate };
            }

            if (difference == module)
            {
                //if difference is same as module, job start date is pay period start date
                startDate = date;
            }
            else
            {

                int diffNumberOfDaysModule = baseDateRange - module;
                if (diffNumberOfDaysModule == 1)
                {
                    int negativeDays = (-module);
                    startDate = date.AddDays(negativeDays).Date;
                }
                else
                {
                    if (module == 0)
                    {
                        startDate = date;

                    }
                    else
                    {
                        //module++;
                        int negativeDays = (-module);
                        startDate = date.AddDays(negativeDays).Date;
                    }
                }
            }
            endDate = startDate.AddDays(baseDateRange - 1);
            endDate += new TimeSpan(23, 59, 0);
            return new DateTime[] { startDate.Date, endDate };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameOfFirstDayInWeek"></param>
        /// <param name="jobStartDate"></param>
        /// <returns></returns>
        public static DateTime CalculateFirstDateInWeek(string nameOfFirstDayInWeek, DateTime jobStartDate)
        {
            while (!nameOfFirstDayInWeek.Equals(jobStartDate.DayOfWeek.ToString()))
            {
                jobStartDate = jobStartDate.AddDays(-1);
            }
            return jobStartDate;
        }

        /// <summary>
        /// Method used for checking if passed day of week is between start date and end date
        /// </summary>
        /// <param name="startDate">Start date of date range</param>
        /// <param name="endDate">End date of date range</param>
        /// <param name="dayOfWeek">Day of the week</param>
        /// <returns>Return number of passed day of week between date range.</returns>
        public static int CheckifDayOfWeekExitsInDateRange(DateTime startDate, DateTime endDate, string dayOfWeek)
        {
            int count = 0;
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek.ToString().Equals(dayOfWeek))
                {
                    count++;
                }
            }
            return count;
        }


        public DateTime[] GenerateDateRange(RankingRule rankingRule, DateTime JobStartDate, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek)
        {
            if (rankingRule.DateRange.ToLower() == "week")
            {
                DateTime startDate = CalculateFirstDateInWeek(startDayOfTheWeek, JobStartDate).AddHours(-JobStartDate.Hour).AddMinutes(-JobStartDate.Minute).AddSeconds(-JobStartDate.Second);
                DateTime endDate = startDate.AddDays(6);

                return new DateTime[] { startDate, endDate };
            }
            else if (rankingRule.DateRange.ToLower() == "month")
            {
                DateTime thisMonthStart = JobStartDate.AddDays(1 - JobStartDate.Day).AddHours(-JobStartDate.Hour).AddMinutes(-JobStartDate.Minute).AddSeconds(-JobStartDate.Second);
                DateTime thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                return new DateTime[] { thisMonthStart, thisMonthEnd };
            }
            else if (rankingRule.DateRange.ToLower() == "pay period")
            {
                if (payPeriodDTO == null)
                {
                    return null;
                }

                if (JobStartDate.Date < payPeriodDTO.PayPeriodStartDate.Value.Date)
                {
                    return null;
                }


                if (payPeriodDTO.IsMonthlyPayPeriod)
                {
                    DateTime st = payPeriodDTO.PayPeriodStartDate.Value;
                    DateTime et = payPeriodDTO.PayPeriodStartDate.Value.AddDays(DateTime.DaysInMonth(payPeriodDTO.PayPeriodStartDate.Value.Year, payPeriodDTO.PayPeriodStartDate.Value.Month) - 1);
                    while (JobStartDate.Date > et.Date)
                    {
                        st = et.AddDays(1);
                        et = st.AddDays(DateTime.DaysInMonth(st.Year, st.Month) - 1);
                    }

                    return new DateTime[] { st, et };
                }

                int numberOfDays = payPeriodDTO.PayPeriodNumberOfDays.Value;
                int difference = (int)(JobStartDate - payPeriodDTO.PayPeriodStartDate.Value).TotalDays;
                int module = difference % numberOfDays;
                DateTime startDate;
                DateTime endDate;
                //if job is in defualt pay period date range (from sys pref page)
                if (JobStartDate.Date >= payPeriodDTO.PayPeriodStartDate.Value.Date && JobStartDate.Date <= payPeriodDTO.PayPeriodStartDate.Value.Date.AddDays(numberOfDays - 1))
                {
                    startDate = payPeriodDTO.PayPeriodStartDate.Value;
                    endDate = startDate.AddDays(numberOfDays - 1);
                    return new DateTime[] { startDate, endDate };
                }

                if (difference == module)
                {
                    //if difference is same as module, job start date is pay period start date
                    startDate = JobStartDate.Date;

                }
                else
                {

                    int diffNumberOfDaysModule = numberOfDays - module;
                    if (diffNumberOfDaysModule == 1)
                    {
                        int negativeDays = (-module);
                        startDate = JobStartDate.AddDays(negativeDays);
                    }
                    else
                    {
                        if (module == 0)
                        {
                            startDate = JobStartDate;

                        }
                        else
                        {
                            //module++;
                            int negativeDays = (-module);
                            startDate = JobStartDate.AddDays(negativeDays);
                        }
                    }
                }

                endDate = startDate.AddDays(numberOfDays - 1);
                return new DateTime[] { startDate, endDate };

            }
            else if (rankingRule.DateRange.ToLower() == "custom")
            {
                /*
                if (scheduleActiveConstraint.Offset == 0 && scheduleActiveConstraint.Amount == 0)
                {
                    return new DateTime[] { jobsForSolver.JobStartDate, jobsForSolver.JobEndDate };
                }*/
                DateTime startDate = JobStartDate.AddDays((int)rankingRule.Offset);
                DateTime endDate = JobStartDate.AddDays((int)rankingRule.Amount);
                return new DateTime[] { startDate, endDate };
            }
            else if (rankingRule.DateRange.ToLower() == "base date")
            {
                if (JobStartDate.Date < rankingRule.BaseDate.Value.Date)
                {
                    return null;
                }
                int difference = (int)(JobStartDate - rankingRule.BaseDate.Value).TotalDays;
                int module = difference % rankingRule.BaseDateRange.Value;
                DateTime startDate;
                DateTime endDate;
                //if job is in defualt pay period date range (from sys pref page)
                if (JobStartDate.Date >= rankingRule.BaseDate.Value && JobStartDate.Date <= rankingRule.BaseDate.Value.Date.AddDays(rankingRule.BaseDateRange.Value - 1))
                {
                    startDate = rankingRule.BaseDate.Value;
                    endDate = startDate.AddDays(rankingRule.BaseDateRange.Value - 1);
                    return new DateTime[] { startDate, endDate };
                }

                if (difference == module)
                {
                    //if difference is same as module, job start date is pay period start date
                    startDate = JobStartDate.Date;
                }
                else
                {

                    int diffNumberOfDaysModule = rankingRule.BaseDateRange.Value - module;
                    if (diffNumberOfDaysModule == 1)
                    {
                        int negativeDays = (-module);
                        startDate = JobStartDate.AddDays(negativeDays);
                    }
                    else
                    {
                        if (module == 0)
                        {
                            startDate = JobStartDate;

                        }
                        else
                        {
                            //module++;
                            int negativeDays = (-module);
                            startDate = JobStartDate.AddDays(negativeDays);
                        }
                    }
                }
                endDate = startDate.AddDays(rankingRule.BaseDateRange.Value - 1);
                return new DateTime[] { startDate, endDate };
            }
            return null;
        }


        public bool CompareDate(DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1.Year == dateTime2.Year && dateTime1.Month == dateTime2.Month && dateTime1.Day == dateTime2.Day;
        }
    }
}
