using eSolver.AutoSolverNew.RankingLogic;
using eSolver.BusinessLogic.Managers;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using eSolver.Entities.Responses;
using System;
using System.Collections.Generic;

namespace eSolver.SolverConfiguration
{
    /// <summary>
    /// This class responsibility is to allocate all objects that are needed for Solver.
    /// This class also calculates time needed for AutoSolve(How long solver will spent in order to find solution)
    /// </summary>
    public static class InitSetUp
    {
        public const decimal DEFAULT_JOB_TOKEN = 4M;
        public const decimal JOB_FREQ_TOKEN = 2M;
        public const decimal DAY_OF_WEEK_FREQUENCY_TOKEN = 2M;
        public const decimal SCHEDULE_HOURS_TOKEN = 12M;
        public const decimal EMPLOYEE_FILD_VALUE_TOKEN = 2M;
        public const decimal COVERAGE_TOKEN = 1M;
        public const decimal EMPLOYEE_SKILL_TOKEN = 4M;

        /// <summary>
        /// For passed parameters method will initialize all objcets and set up initial data for solver.
        /// </summary>
        /// <param name="request">All Domain models populated with passed values</param>
        /// <param name="preprocessing">Class which will sort jobs based on his dates</param>
        /// <param name="autoSolveResponse">Populated solution with employee and job Ids</param>
        /// <param name="printOutput">Print otput</param>
        /// <param name="setRestictedNumberOfEmployees">On how many jobs employee can work in same time</param>
        /// <param name="finalConsoleOutput">Print final solution</param>
        /// <param name="employees">Collection of all Employees</param>
        /// <param name="jobs">Jobs which will be solved</param>
        /// <param name="payPeriodDTO">PayPeriod model</param>
        /// <param name="StartDayOfTheWeek">Start date of week</param>
        /// <param name="preCostraints"></param>
        /// <param name="ranking"></param>
        /// <param name="maxNumOfJobsAtSameTime"></param>
        /// <param name="maximaze"></param>
        /// <param name="maxTime"></param>
        /// <param name="maxTimeRanking"></param>
        /// <param name="rankingWrappers"></param>
        /// <param name="implement"></param>
        /// <param name="token"></param>
        /// <param name="jobsFromOutside"></param>
        /// <param name="largeDateSet"></param>
        public static void InitBasicSetupOfModel(AutoSolveRequest request, out JobManager preprocessing, 
            out SolversResponse autoSolveResponse, out bool printOutput, out bool setRestictedNumberOfEmployees, 
            out bool finalConsoleOutput, out List<Employee> employees, out List<ScheduleJob> jobs, 
            out PayPeriodDTO payPeriodDTO, out string StartDayOfTheWeek, out ConstraintManager preCostraints, 
            out int maxNumOfJobsAtSameTime, 
            out bool maximaze, out float maxTime, out float maxTimeRanking, out List<RankingWrapper> rankingWrappers, 
            out bool implement, out decimal token, out List<JobFromOutside> jobsFromOutside, out bool largeDateSet, out bool? canChangeEmployeeOnTemplateJobs)
        {
            preprocessing = new JobManager();
            autoSolveResponse = new SolversResponse();
            printOutput = false;
            setRestictedNumberOfEmployees = false;
            finalConsoleOutput = true;
            implement = false;
            jobsFromOutside = request.JobOutsides;

            // =>   Parsing json strings to Lists of the objects
            employees = request.Employees;
            jobs = request.Schedule.ScheduleJobs;
            payPeriodDTO = request.PayPeriodDTO;
            StartDayOfTheWeek = request.StartDayOfTheWeek;
            preCostraints = new ConstraintManager();
            preCostraints.FromJsonToConstraints(request.ConstraintRules, request.Schedule.ScheduleActiveConstraints);

            rankingWrappers = new List<RankingWrapper>();
            canChangeEmployeeOnTemplateJobs = request.CanChangeEmployeeOnTemplateJobs;


            maxNumOfJobsAtSameTime = 1;
            maximaze = request.Maximize;

            token = COVERAGE_TOKEN;

            bool hasDynamicRanking = false;

            if (request.ScheduleRankings != null)
            {
                foreach (ScheduleRanking scheduleRanking in request.ScheduleRankings)
                {
                    foreach (RankingType rankingType in request.RankingTypes)
                    {
                        if (rankingType.Id == scheduleRanking.RankingTypeID)
                        {
                            switch (rankingType.Name)
                            {
                                case "Default job":
                                    token += DEFAULT_JOB_TOKEN;
                                    break;

                                case "Job Type Frequency":
                                    token += JOB_FREQ_TOKEN;
                                    hasDynamicRanking = true;
                                    break;

                                case "Day of week frequency":
                                    token += DAY_OF_WEEK_FREQUENCY_TOKEN;
                                    hasDynamicRanking = true;
                                    break;

                                case "Scheduled Hours":
                                    token += SCHEDULE_HOURS_TOKEN;
                                    hasDynamicRanking = true;
                                    break;

                                case "Employee field value":
                                    token += EMPLOYEE_FILD_VALUE_TOKEN;
                                    break;

                                case "Skills":
                                    token += EMPLOYEE_SKILL_TOKEN;
                                    break;
                            }
                            break;
                        }
                    }
                }
            }

            maxTime = request.MaxTime;

            // Time formula for Google Or Tools part
            if (maxTime == 300)
            {
                maxTime = (float)(8 * Math.Log10(0.025 * (employees.Count * jobs.Count + 45)));
            }

            if (employees.Count * jobs.Count > 10000)
                maxTime -= 2.5f;
            else
            {
                if (employees.Count * jobs.Count > 1000)
                    maxTime -= 1.75f;
            }
                

            maxTimeRanking = 0;
            largeDateSet = (hasDynamicRanking && maxTime < 60 && employees.Count * jobs.Count > 10000);
            //maxTime *= (float) 10;
        }
    }
}
