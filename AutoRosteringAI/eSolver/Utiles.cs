using eSolver.AutoSolverNew.RankingLogic;
using eSolver.BusinessLogic.Managers;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using eSolver.Entities.Responses;
using eSolver.SolverConfiguration;
using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace eSolver
{
    public class Utiles
    {
        public static void PrintTimeElapsed(Stopwatch stopWatch, string message, bool test)
        {
            /*
            if (test)
            {
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine(message + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10));
            }*/
        }

        public static decimal GetTimeForSolver(float maxTime, decimal token, RankingManager rankingControler, int turnForRanking)
        {
            decimal timeForRanking = 0;
            switch (rankingControler.rankingWrappers[turnForRanking].rankingName)
            {
                case "Default":
                    timeForRanking = InitSetUp.DEFAULT_JOB_TOKEN;
                    break;
                case "Job Freq":
                    timeForRanking = InitSetUp.JOB_FREQ_TOKEN;
                    break;
                case "Week Freq":
                    timeForRanking = InitSetUp.DAY_OF_WEEK_FREQUENCY_TOKEN;
                    break;
                case "Hours":
                    timeForRanking = InitSetUp.SCHEDULE_HOURS_TOKEN;
                    break;
                case "Employee Field":
                    timeForRanking = InitSetUp.EMPLOYEE_FILD_VALUE_TOKEN;
                    break;
                case "Coverage":
                    timeForRanking = InitSetUp.COVERAGE_TOKEN;
                    break;
                case "Skills":
                    timeForRanking = InitSetUp.EMPLOYEE_SKILL_TOKEN;
                    break;
            }
            timeForRanking = timeForRanking / token * (decimal)maxTime;
            return timeForRanking;
        }

        internal static ModelConfiguration ModelPreprocessing(bool setRestictedNumberOfEmployees, List<Employee> employees, List<ScheduleJob> jobs, int maxNumOfJobsAtSameTime, RankingManager rankingControler, List<List<long>> finalPairs, List<ScheduleJob> jobsNonSorted, IEnumerable<int> setOfJobs, IEnumerable<int> setOfEmployees, int[,] availabilityMatrix, List<JobFromOutside> jobsFromOutsides, bool ignoreAlreadyAssignedJobs)
        {
            ModelConfiguration modelConfiguration = new ModelConfiguration(employees, jobs, setOfEmployees, setOfJobs);
            modelConfiguration.InitialModel();

            modelConfiguration.AssignAvailableEmployees(employees, availabilityMatrix, ignoreAlreadyAssignedJobs);
            modelConfiguration.ApplyRequired(setRestictedNumberOfEmployees);
            modelConfiguration.NotAlreadyAssigned(finalPairs, maxNumOfJobsAtSameTime, employees, jobsNonSorted);

            return modelConfiguration;
        }

        public static SolversResponse PostSolver(SolversResponse autoSolveResponse, bool test, bool finalConsoleOutput, SolverSolutionObserver cb)
        {
            /// <return>
            /// If there is solution,"foundSolution" : true
            /// "jobs" : {with list of id's of the employees}
            /// First number of required are going to be assigned
            /// else are going to be onSlot
            /// 
            /// Else, if there is no soltuion, "foundSolution" : false
            /// "jobs" : null
            /// </return>
            
            if (cb.GetOutPutMatrix() != null)
            {
                SolversResponse sjson = cb.GetSolution(test);
                sjson.IsSolutionFound = true;
                if (finalConsoleOutput && test)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("*** Solutionion for output! ***");
                    Console.WriteLine();
                    Console.WriteLine();
                }

                autoSolveResponse = cb.GetSolution(test);
            }
            else
            {
                SolversResponse sjson = new SolversResponse
                {
                    IsSolutionFound = false
                };
                string json2 = JsonConvert.SerializeObject(sjson);

            }

            return autoSolveResponse;
        }

        public static void PostRankingCheck(bool test, bool finalConsoleOutput, bool maximaze, List<Employee> employees, List<ScheduleJob> jobs, CpSolver solver, SolverSolutionObserver cb, CpSolverStatus status, ref int maxEmployeesWorking, List<RankingWrapper> rankingWrappers, IntVar[] dynamicConstraintLower, IntVar[] dynamicConstraintUpper, int turnForRanking, bool largeDateSet, bool test1)
        {
            if ((status == CpSolverStatus.Unknown) && test)
            {
                Console.WriteLine("                 Unknown!!!                  ");
            }

            else if (status == CpSolverStatus.Feasible || status == CpSolverStatus.Optimal)
            {
                cb.ParseSolutionBetweenSolverModels(solver, status, ModelConfiguration.matrix, rankingWrappers, dynamicConstraintLower, dynamicConstraintUpper, maxEmployeesWorking, turnForRanking, largeDateSet, test);
            }

            else if (status == CpSolverStatus.Infeasible && test)
            {
                Console.WriteLine("                 Infeasible!!!                  ");

            }

            if (cb.GetOutPutMatrix() != null && test)
            {
                cb.ConsolePrintSolution(cb.GetOutPutMatrix());
            }

            if (test)
            {
                Console.WriteLine(maximaze);
                Console.WriteLine(cb.GetMistake() != 0);
                Console.WriteLine(!cb.foundAtFirstNonMaximizeSolution);
            }

            if (test)
            {
                if (finalConsoleOutput)
                {
                    Console.WriteLine("Number    of jobs: " + jobs.Count);
                    Console.WriteLine("Employess of jobs: " + employees.Count);
                    Console.WriteLine("Statistics");
                    Console.WriteLine(String.Format("  - solve status    : {0}", status));
                    Console.WriteLine("  - conflicts       : " + solver.NumConflicts());
                    Console.WriteLine("  - branches        : " + solver.NumBranches());
                    Console.WriteLine("  - wall time       : " + solver.WallTime() + " ms");
                    Console.WriteLine("  - full time       : " + cb.stopwatch.Elapsed + " s");
                    Console.WriteLine("  - solutions       : " + cb.CountsSoultions());
                    Console.WriteLine("  - loss            : " + cb.GetMistake());
                }
            }

            if (maxEmployeesWorking == -1)
            {
                maxEmployeesWorking = cb.countAssigments;
            }
        }

        public static void PrintRangs(bool test, float[,] RankMatrix)
        {
            if (test)
            {
                Console.WriteLine("Rank matrica after");
                for (int x = 0; x < RankMatrix.GetLength(0); x++)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    for (int y = 0; y < RankMatrix.GetLength(1); y++)
                    {
                        Console.Write(String.Format(" {0:0.000}", RankMatrix[x, y]));
                    }
                }
            }
        }

        /// <summary>
        /// For testing purpose, write the serial number of the auto solve's order
        /// </summary>
        /// <param name="v"></param>
        /// <param name="test"></param>
        public static void PrintAutoSolve(int v, bool test)
        {
            if (test)
            {
                Console.WriteLine();
                Console.WriteLine("####################################################");
                Console.WriteLine();
                Console.WriteLine("\t\tAUTO-SOLVE " + v);
                Console.WriteLine();
                Console.WriteLine("####################################################");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Preprocess jobs data type, 
        /// sort the jobs by the start date and 
        /// find all overlapping jobs, 
        /// that are gone be used later for not already assigned constraint
        /// </summary>
        /// <param name="preprocessing"></param>
        /// <param name="test"></param>
        /// <param name="employees"></param>
        /// <param name="jobs"></param>
        /// <param name="finalPairs"></param>
        /// <param name="jobsNonSorted"></param>
        /// <returns></returns>
        public static List<ScheduleJob> JobsPreprocess(JobManager preprocessing, bool test, List<Employee> employees, List<ScheduleJob> jobs, out List<List<long>> finalPairs, out List<ScheduleJob> jobsNonSorted)
        {
            finalPairs = new List<List<long>>();
            jobsNonSorted = jobs;
            if (jobs.Count > 1)
            {
                jobs = preprocessing.BubbleSort(jobs);
                //jobs = jobs.OrderBy(o => o.JobStartDate).ToList();
                List<DateTime> dataPoints = preprocessing.SortChronologicalIntervals(jobs);
                dataPoints.Sort((ps1, ps2) => DateTime.Compare(ps1, ps2));
                finalPairs = preprocessing.FindOverLappingNIntervals(jobs, dataPoints);
            }
            for (int i = 0; i < jobs.Count; i++)
            {
                jobs[i].HoursT = TimeSpan.FromSeconds(jobs[i].Hours / 10000000);
                jobs[i].HoursT1 = TimeSpan.FromSeconds(jobs[i].Hours1 / 10000000);
                jobs[i].HoursT2 = TimeSpan.FromSeconds(jobs[i].Hours2 / 10000000);
                jobs[i].HoursT3 = TimeSpan.FromSeconds(jobs[i].Hours3 / 10000000);
                jobs[i].HoursT4 = TimeSpan.FromSeconds(jobs[i].Hours4 / 10000000);
                jobs[i].HoursT5 = TimeSpan.FromSeconds(jobs[i].Hours5 / 10000000);
                jobs[i].HoursT6 = TimeSpan.FromSeconds(jobs[i].Hours6 / 10000000);
            }

            return jobs;
        }
    }
}
