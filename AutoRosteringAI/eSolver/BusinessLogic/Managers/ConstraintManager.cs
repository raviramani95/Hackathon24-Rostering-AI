using System;
using System.Collections.Generic;
using eSolver.Entities;
using Google.OrTools.Sat;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_MaximumNumberOfHours;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_EmployeeLeaverStatus;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_EmployeeMustWorkWithAnotherEmployee;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_EmployeeMustNotWorkWithAnotherEmployee;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_MaximumNumberOfDaysOfTheWeek;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_MaximumNumberOfJobTypes;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_SplitShifts;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_DaysPeriodOff;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_MaximumNumberOfJobTypesConsecutively;
using static eSolver.BusinessLogic.ConstraintsLogic.Const_MaximumNumberOfJobTimes;

using eSolver.Entities.OutSideView;

namespace eSolver.BusinessLogic.Managers
{
    public class ConstraintManager
    {
        #region Propeties

        List<string> ConstraintNames = new List<string>();


        #endregion
        /// <summary>
        /// Method finds ConstraintRuleName based on ConstraintRuleID. From list active constraints method will find ConstraintRuleName in list of all defined constraint rules
        /// </summary>
        /// <param name="allConstraintRules">All Constraint Rules defined in System</param>
        /// <param name="scheduleActiveConstraints">Constraints defined in schedules</param>
        public void FromJsonToConstraints(List<AllConstraintRule> allConstraintRules, List<ScheduleActiveConstraint> scheduleActiveConstraints)
        {
            /*
             * Iterate trough all active constraints in Schedule, 
             * find appropriate RuleID and store ConstraintRuleName in Collection
             */
            foreach (ScheduleActiveConstraint scheduleActiveConstraint in scheduleActiveConstraints)
            {
                foreach (AllConstraintRule allConstraintRule in allConstraintRules)
                {
                    if (allConstraintRule.Id == scheduleActiveConstraint.ConstraintRuleID)
                    {
                        ConstraintNames.Add(allConstraintRule.Name);
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// MAIN PART of the constraints
        /// Pass through the all constraints.
        /// Apply necessary from constraintRule.
        /// Constraints does not have priorities.
        /// Some constraint need to be applied only once.
        /// 
        /// Applaying constraints to the model
        /// There is 18 diffrent types of the constraints
        /// </summary>
        /// </summary>
        public void ApplyPreConstraints(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, List<ScheduleJob> jobsNonSorted, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek, List<List<long>> finalPairs, List<JobFromOutside> jobFromOutsides, List<ScheduleActiveConstraint> constraints)
        {
            bool isEmployeeLeaverStatusApplied = false;
            for (int i = 0; i < constraints.Count; i++)
            {
                //Console.WriteLine(constraintName[i]);
                if (ConstraintNames[i].Equals("Maximum Number Of Hours"))
                {
                    // [3.1.2] Maximum number of hours
                    if (constraints[i].ComparisonValues != null && constraints[i].ComparisonValues.Count > 0)
                    {
                        constraints[i].ConstraintMNOH.ComparisonValues = constraints[i].ComparisonValues;
                    }
                    ApplyMaximumNumberOfHours(matrix, model, employees, jobs, constraints[i], payPeriodDTO, jobFromOutsides, startDayOfTheWeek);
                }
                else if (ConstraintNames[i].Equals("Employee Leaver Status") && !isEmployeeLeaverStatusApplied)
                {
                    // [3.1.3] Employee leaver status
                    ApplyEmployeeLeaverStatus(matrix, model, employees, new List<ScheduleJob>(jobs), jobsNonSorted);
                    isEmployeeLeaverStatusApplied = true;   // It is enough to check once
                }
                /*
                else if (constraintName[i].Equals("Employee Leaver Status"))
                {
                    // 3.1.4  Employee planned absence
                    applyEmployeePlannedAbsence(matrix, model, employees, jobs);
                }
                
                else if (constraint.constraintRule.isEmployeeAvailability)
                {
                    // 3.1.5  Employee availability
                    applyEmployeeAvailability(matrix, model, employees, jobs);
                }
                */
                else if (ConstraintNames[i].Equals("Employee Must Work With Another Employee"))
                {
                    // 3.1.6  Employee must work with another employee
                    ApplEmployeeMustWorkWithAnotherEmployee(matrix, model, employees, jobs, constraints[i], finalPairs, jobFromOutsides);
                }
                else if (ConstraintNames[i].Equals("Employee Must Not Work With Another Employee"))
                {
                    // 3.1.7  Employee must not work with another employee
                    ApplyEmployeeMustNotWorkWithAnotherEmployee(matrix, model, employees, jobs, constraints[i], finalPairs, jobFromOutsides);
                }
                else if (ConstraintNames[i].Equals("Maximum Number Of Days Of The Week"))
                {
                    // [3.1.8] Maximum number of days of the weeks
                    ApplyIsMaximumNumberOfDaysOfTheWeek(matrix, model, employees, new List<ScheduleJob>(jobs), ((DayOfWeek)Enum.Parse(typeof(DayOfWeek), constraints[i].ConstraintMNODOTW.DayOfWeek)), constraints[i], constraints[i].ConstraintMNODOTW.CustomData, payPeriodDTO, jobFromOutsides, startDayOfTheWeek);
                }
                else if (ConstraintNames[i].Equals("Maximum Number Of Job Types"))
                {
                    // 3.1.9  Maximum number of job types
                    ApplyMaximumNumberOfJobTypes(matrix, model, employees, new List<ScheduleJob>(jobs), constraints[i], payPeriodDTO, jobFromOutsides, startDayOfTheWeek);
                }
                else if (ConstraintNames[i].Equals("Maximum Number Of Job Times"))
                {
                    // 3.1.10 Maximum number of job times
                    ApplyMaximumNumberOfJobTimes(matrix, model, employees, new List<ScheduleJob>(jobs), constraints[i], payPeriodDTO, jobFromOutsides, startDayOfTheWeek);
                }
                else if (ConstraintNames[i].Equals("Maximum Number Of Job Types Consecutively"))
                {
                    // 3.1.11 Maximum number of job types consecutively
                    if (constraints[i].ComparisonValues != null && constraints[i].ComparisonValues.Count > 0)
                    {
                        constraints[i].ConstraintMNOJTC.ComparisonValues = constraints[i].ComparisonValues;
                    }
                    ApplyMaximumNumberOfJobTypesConsecutively(matrix, model, employees, new List<ScheduleJob>(jobs), constraints[i].ConstraintMNOJTC, payPeriodDTO, jobFromOutsides, startDayOfTheWeek);
                }
                /*
                else if (constraint.constraintRule.isMustNotWorkForTimePeriodAfterJobType)
                {
                    // 3.1.12 Must not work for a time period after a certain job type
                    applyMustNotWorkForATimePeriodAfterACertainJobType(matrix, model, employees, jobs);
                }
                else if (constraint.constraintRule.isMaximumNumberOfAverageHoursInPeriod)
                {
                    // 3.1.13 Maximum number of average hours in a period
                    applyMaximumNumberOfAverageHoursInAPeriod(matrix, model, employees, jobs);
                }
                */
                else if (ConstraintNames[i].Equals("Days Off Within A Period"))
                {
                    // 3.1.14 Days off within a period
                    if (constraints[i].ComparisonValues != null && constraints[i].ComparisonValues.Count > 0)
                    {
                        constraints[i].ConstraintDOWP.ComparisonValues = constraints[i].ComparisonValues;
                    }
                    ApplyDaysOffWithinAPeriod(matrix, model, employees, jobs, constraints[i].ConstraintDOWP, payPeriodDTO, jobFromOutsides, startDayOfTheWeek);
                }
                else if (ConstraintNames[i].Equals("Split Shifts"))
                {
                    // 3.1.15 Split shifts
                    ApplySplitShifts(matrix, model, employees, jobs, constraints[i], jobFromOutsides);
                }
                /*
                else if (constraint.constraintRule.isNightWorkerMinimumAge)
                {
                    // 3.1.16 Night worker minimum age
                    applyNightWorkerMinimumAge(matrix, model, employees, jobs);
                }
                else if (constraint.constraintRule.isEmployeeFieldValue)
                {
                    // 3.1.17 Employee field value
                    applyEmployeeFieldValue(matrix, model, employees, jobs);
                }
                else if (constraint.constraintRule.isEmployeeRoster)
                {
                    // 3.1.18 Employee roster
                    applyEmployeeRoster(matrix, model, employees, jobs);
                }
                */
            }
        }
    }
}
