using eSolver.AutoSolverNew.RankingLogic;
using eSolver.BusinessLogic.Managers;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using eSolver.SolverConfiguration;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static eSolver.Utiles;

namespace eSolver
{
    public class AutoSolveBase
    {

        public void AutoSolveBruteForce(AutoSolveRequest request, bool test, bool setRestictedNumberOfEmployees, bool finalConsoleOutput, List<Employee> employees, List<ScheduleJob> jobs, PayPeriodDTO payPeriodDTO, string StartDayOfTheWeek, ConstraintManager preCostraints, int maxNumOfJobsAtSameTime, bool maximaze, float maxTimeRanking, List<RankingWrapper> rankingWrappers, ref bool implement, List<JobFromOutside> jobsFromOutside, bool largeDateSet, List<List<long>> finalPairs, List<ScheduleJob> jobsNonSorted, RankingManager rankingControler, IEnumerable<int> setOfJobs, IEnumerable<int> setOfEmployees, int[,] availabilityMatrix, ref ModelConfiguration modelConfiguration, CpSolver solver, HashSet<int> toPrint, ref CpSolverStatus status, ref int maxEmployeesWorking, int turnForRanking, ref SolverSolutionObserver cb)
        {
            if ((status == CpSolverStatus.Feasible || status == CpSolverStatus.Optimal))
            {
                if (maxTimeRanking != 0)
                {
                    if (test)
                    {
                        PrintAutoSolve(2 * turnForRanking + 2, test);
                    }

                    implement = true;

                    modelConfiguration = ModelPreprocessing(setRestictedNumberOfEmployees, employees, jobs, maxNumOfJobsAtSameTime, rankingControler, finalPairs, jobsNonSorted, setOfJobs, setOfEmployees, availabilityMatrix, jobsFromOutside, false);

                    preCostraints.ApplyPreConstraints(ModelConfiguration.matrix, ModelConfiguration.model, employees, jobs, jobsNonSorted, payPeriodDTO, StartDayOfTheWeek, finalPairs, jobsFromOutside, request.Schedule.ScheduleActiveConstraints);

                    solver.StringParameters = "max_time_in_seconds: maxTimeRanking";
                    cb = new SolverSolutionObserver(ModelConfiguration.matrix, employees, jobs, toPrint, 0, false, cb.outputMatrixInt, false, ModelConfiguration.lLimit, ModelConfiguration.uLimit, ModelConfiguration.limit, ModelConfiguration.dynamicConstraintUpper, ModelConfiguration.dynamicConstraintLower, test)
                    {
                        stopAtFirstNonMaximizeSolution = false
                    };

                    modelConfiguration.Optimaze(maximaze, request.ScheduleRankings, request.RankingRules, request.RankingTypes, rankingControler.rankingWrappers, maxEmployeesWorking, implement, largeDateSet, turnForRanking, -1, rankingControler.maxSortOrder);
                    cb.stopwatch.Start();

                    status = solver.Solve(ModelConfiguration.model, cb);

                    //status = CpSolverStatus.Feasible;

                    PostRankingCheck(test, finalConsoleOutput, maximaze, employees, jobs, solver, cb, status, ref maxEmployeesWorking, rankingWrappers, ModelConfiguration.dynamicConstraintLower, ModelConfiguration.dynamicConstraintUpper, turnForRanking, largeDateSet, test); //Console.WriteLine(solver.ObjectiveValue); //Console.WriteLine(solver.BestObjectiveBound);

                }
            }
        }

        public void AutoSolveLargeDate(Stopwatch stopWatch, AutoSolveRequest request, bool test, bool setRestictedNumberOfEmployees, bool finalConsoleOutput, List<Employee> employees, List<ScheduleJob> jobs, PayPeriodDTO payPeriodDTO, string StartDayOfTheWeek, ConstraintManager preCostraints, int maxNumOfJobsAtSameTime, bool maximaze, float maxTime, List<RankingWrapper> rankingWrappers, ref bool implement, List<JobFromOutside> jobsFromOutside, bool largeDateSet, List<List<long>> finalPairs, List<ScheduleJob> jobsNonSorted, string fixedSearch, RankingManager rankingControler, IEnumerable<int> setOfJobs, IEnumerable<int> setOfEmployees, int[,] availabilityMatrix, ref ModelConfiguration modelConfiguration, out CpSolver solver, ref HashSet<int> toPrint, out CpSolverStatus status, ref int maxEmployeesWorking, double maxTimeSecond, ref SolverSolutionObserver cb, bool? canChangeEmployeeOnTemplateJobs)
        {
            preCostraints.ApplyPreConstraints(ModelConfiguration.matrix, ModelConfiguration.model, employees, jobs, jobsNonSorted, payPeriodDTO, StartDayOfTheWeek, finalPairs, jobsFromOutside, request.Schedule.ScheduleActiveConstraints);

            solver = new CpSolver();

            solver.StringParameters = solver.StringParameters = "num_search_workers:" + Environment.ProcessorCount + ", log_search_progress:" + test + ", random_seed: 1196, max_time_in_seconds:" + maxTime;

            cb = new SolverSolutionObserver(ModelConfiguration.matrix, employees, jobs, toPrint, 0, false, null, false, ModelConfiguration.lLimit, ModelConfiguration.uLimit, ModelConfiguration.limit, ModelConfiguration.dynamicConstraintUpper, ModelConfiguration.dynamicConstraintLower, test)
            {
                stopAtFirstNonMaximizeSolution = false
            };


            PrintTimeElapsed(stopWatch, "Apply constraints DONE: ", test);
            modelConfiguration.Optimaze(maximaze, request.ScheduleRankings, request.RankingRules, request.RankingTypes, rankingControler.rankingWrappers, maxEmployeesWorking, implement, largeDateSet, -2, -1, rankingControler.maxSortOrder);
            PrintTimeElapsed(stopWatch, "Model Optimaze DONE: ", test);

            cb.stopwatch.Start();

            status = solver.Solve(ModelConfiguration.model);
            PrintTimeElapsed(stopWatch, "Google-Or-Tool DONE: ", test);

            PostRankingCheck(test, finalConsoleOutput, maximaze, employees, jobs, solver, cb, status, ref maxEmployeesWorking, rankingWrappers, ModelConfiguration.dynamicConstraintLower, ModelConfiguration.dynamicConstraintUpper, rankingControler.rankingWrappers.Count, largeDateSet, test); //Console.WriteLine(solver.ObjectiveValue); //Console.WriteLine(solver.BestObjectiveBound);

            long k;
            implement = true;
            bool ignoreAssignedEmployees = true;
            if ((k = cb.HasPotentialForSecondDepth(rankingControler.rankingWrappers, solver, status)) != -1)
            {
                cb.ExtractSecondDepth(rankingControler.rankingWrappers, solver, k);

                
                if (canChangeEmployeeOnTemplateJobs != null)
                {
                    ignoreAssignedEmployees = Convert.ToBoolean(canChangeEmployeeOnTemplateJobs);
                }

                modelConfiguration = ModelPreprocessing(setRestictedNumberOfEmployees, employees, jobs, maxNumOfJobsAtSameTime, rankingControler, finalPairs, jobsNonSorted, setOfJobs, setOfEmployees, availabilityMatrix, jobsFromOutside, ignoreAssignedEmployees);

                cb.GetHints(solver, ModelConfiguration.matrix, ModelConfiguration.model);

                preCostraints.ApplyPreConstraints(ModelConfiguration.matrix, ModelConfiguration.model, employees, jobs, jobsNonSorted, payPeriodDTO, StartDayOfTheWeek, finalPairs, jobsFromOutside, request.Schedule.ScheduleActiveConstraints);

                solver = new CpSolver();
                toPrint = new HashSet<int>();

                solver.StringParameters = solver.StringParameters = "num_search_workers:" + Environment.ProcessorCount + ", log_search_progress: " + test + ", random_seed: 1196, " + fixedSearch + "max_time_in_seconds:" + maxTimeSecond; // 
                cb = new SolverSolutionObserver(ModelConfiguration.matrix, employees, jobs, toPrint, 0, false, cb.outputMatrixInt, false, ModelConfiguration.lLimit, ModelConfiguration.uLimit, ModelConfiguration.limit, ModelConfiguration.dynamicConstraintUpper, ModelConfiguration.dynamicConstraintLower, test)
                {
                    stopAtFirstNonMaximizeSolution = false
                };

                PrintTimeElapsed(stopWatch, "Apply constraints DONE: ", test);
                modelConfiguration.Optimaze(maximaze, request.ScheduleRankings, request.RankingRules, request.RankingTypes, rankingControler.rankingWrappers, maxEmployeesWorking, implement, largeDateSet, -2, -1, rankingControler.maxSortOrder);
                PrintTimeElapsed(stopWatch, "Model Optimaze DONE: ", test);

                cb.stopwatch.Start();

                status = solver.Solve(ModelConfiguration.model);
                PrintTimeElapsed(stopWatch, "Google-Or-Tool DONE: ", test);

                PostRankingCheck(test, finalConsoleOutput, maximaze, employees, jobs, solver, cb, status, ref maxEmployeesWorking, rankingWrappers, ModelConfiguration.dynamicConstraintLower, ModelConfiguration.dynamicConstraintUpper, rankingControler.rankingWrappers.Count, largeDateSet, test); //Console.WriteLine(solver.ObjectiveValue); //Console.WriteLine(solver.BestObjectiveBound);

            }
            cb.FilePrintSolution(cb.outputMatrixInt);
        }

        public int AutoSolvePartial(AutoSolveRequest request, bool test, bool setRestictedNumberOfEmployees, bool finalConsoleOutput, List<Employee> employees, List<ScheduleJob> jobs, PayPeriodDTO payPeriodDTO, string StartDayOfTheWeek, ConstraintManager preCostraints, int maxNumOfJobsAtSameTime, bool maximaze, float maxTime, List<RankingWrapper> rankingWrappers, out bool implement, decimal token, List<JobFromOutside> jobsFromOutside, bool largeDateSet, List<List<long>> finalPairs, List<ScheduleJob> jobsNonSorted, RankingManager rankingControler, IEnumerable<int> setOfJobs, IEnumerable<int> setOfEmployees, int[,] availabilityMatrix, out ModelConfiguration modelConfiguration, out CpSolver solver, out HashSet<int> toPrint, out CpSolverStatus status, int maxEmployeesWorking, int turnForRanking, out decimal timeForRanking, ref SolverSolutionObserver cb, bool? canChangeEmployeeOnTemplateJobs)
        {
            if (test)
            {
                PrintAutoSolve(2 * turnForRanking + 1, test);
            }

            implement = false;
            bool ignoreAssignedEmployees = true;
            if (canChangeEmployeeOnTemplateJobs != null)
            {
                ignoreAssignedEmployees = Convert.ToBoolean(canChangeEmployeeOnTemplateJobs);
            }

            modelConfiguration = ModelPreprocessing(setRestictedNumberOfEmployees, employees, jobs, maxNumOfJobsAtSameTime, rankingControler, finalPairs, jobsNonSorted, setOfJobs, setOfEmployees, availabilityMatrix, jobsFromOutside, ignoreAssignedEmployees);

            preCostraints.ApplyPreConstraints(ModelConfiguration.matrix, ModelConfiguration.model, employees, jobs, jobsNonSorted, payPeriodDTO, StartDayOfTheWeek, finalPairs, jobsFromOutside, request.Schedule.ScheduleActiveConstraints);

            solver = new CpSolver();
            toPrint = new HashSet<int>();
            timeForRanking = GetTimeForSolver(maxTime, token, rankingControler, turnForRanking);
            if (test)
            {
                Console.WriteLine("\t\t\t\t\t\tTIME :" + timeForRanking);
            }

            solver.StringParameters = solver.StringParameters = "num_search_workers:" + Environment.ProcessorCount + ", log_search_progress:" + test + ", random_seed: 1196, max_time_in_seconds:" + timeForRanking;
            cb = new SolverSolutionObserver(ModelConfiguration.matrix, employees, jobs, toPrint, 0, false, cb.outputMatrixInt, false, ModelConfiguration.lLimit, ModelConfiguration.uLimit, ModelConfiguration.limit, ModelConfiguration.dynamicConstraintUpper, ModelConfiguration.dynamicConstraintLower, test)
            {
                stopAtFirstNonMaximizeSolution = false
            };


            modelConfiguration.Optimaze(maximaze, request.ScheduleRankings, request.RankingRules, request.RankingTypes, rankingControler.rankingWrappers, maxEmployeesWorking, implement, largeDateSet, turnForRanking, -1, rankingControler.maxSortOrder);


            cb.stopwatch.Start();
            status = solver.Solve(ModelConfiguration.model);

            PostRankingCheck(test, finalConsoleOutput, maximaze, employees, jobs, solver, cb, status, ref maxEmployeesWorking, rankingWrappers, ModelConfiguration.dynamicConstraintLower, ModelConfiguration.dynamicConstraintUpper, turnForRanking, largeDateSet, test); //Console.WriteLine(solver.ObjectiveValue); //Console.WriteLine(solver.BestObjectiveBound);
            return maxEmployeesWorking;
        }

        public void AutoSolveSecondDepth(Stopwatch stopWatch, AutoSolveRequest request, bool test, bool setRestictedNumberOfEmployees, bool finalConsoleOutput, List<Employee> employees, List<ScheduleJob> jobs, PayPeriodDTO payPeriodDTO, string StartDayOfTheWeek, ConstraintManager preCostraints, int maxNumOfJobsAtSameTime, bool maximaze, List<RankingWrapper> rankingWrappers, bool implement, List<JobFromOutside> jobsFromOutside, bool largeDateSet, List<List<long>> finalPairs, List<ScheduleJob> jobsNonSorted, string hintingSearch, RankingManager rankingControler, IEnumerable<int> setOfJobs, IEnumerable<int> setOfEmployees, int[,] availabilityMatrix, ref ModelConfiguration modelConfiguration, ref CpSolver solver, ref HashSet<int> toPrint, ref CpSolverStatus status, ref int maxEmployeesWorking, int turnForRanking, decimal timeForRanking, ref SolverSolutionObserver cb, bool? canChangeEmployeeOnTemplateJobs)
        {
            long k;
            bool ignoreAssignedEmployees = true;
            if ((k = cb.HasPotentialForSecondDepth(rankingControler.rankingWrappers, solver, turnForRanking)) != -1)
            {
                cb.ExtractSecondDepth(rankingControler.rankingWrappers, solver, k);
                if (canChangeEmployeeOnTemplateJobs != null)
                {
                    ignoreAssignedEmployees = Convert.ToBoolean(canChangeEmployeeOnTemplateJobs);
                }
                modelConfiguration = ModelPreprocessing(setRestictedNumberOfEmployees, employees, jobs, maxNumOfJobsAtSameTime, rankingControler, finalPairs, jobsNonSorted, setOfJobs, setOfEmployees, availabilityMatrix, jobsFromOutside, ignoreAssignedEmployees);

                cb.GetHints(solver, ModelConfiguration.matrix, ModelConfiguration.model);

                preCostraints.ApplyPreConstraints(ModelConfiguration.matrix, ModelConfiguration.model, employees, jobs, jobsNonSorted, payPeriodDTO, StartDayOfTheWeek, finalPairs, jobsFromOutside, request.Schedule.ScheduleActiveConstraints);

                solver = new CpSolver();
                toPrint = new HashSet<int>();

                solver.StringParameters = solver.StringParameters = "num_search_workers:" + Environment.ProcessorCount + ", log_search_progress: " + test + ", random_seed: 1196, " + hintingSearch + "max_time_in_seconds:" + timeForRanking; // 

                cb = new SolverSolutionObserver(ModelConfiguration.matrix, employees, jobs, toPrint, 0, false, cb.outputMatrixInt, false, ModelConfiguration.lLimit, ModelConfiguration.uLimit, ModelConfiguration.limit, ModelConfiguration.dynamicConstraintUpper, ModelConfiguration.dynamicConstraintLower, test)
                {
                    stopAtFirstNonMaximizeSolution = false
                };


                PrintTimeElapsed(stopWatch, "Apply constraints DONE: ", test);
                modelConfiguration.Optimaze(maximaze, request.ScheduleRankings, request.RankingRules, request.RankingTypes, rankingControler.rankingWrappers, maxEmployeesWorking, implement, largeDateSet, turnForRanking, k, rankingControler.maxSortOrder);
                PrintTimeElapsed(stopWatch, "Model Optimaze DONE: ", test);

                cb.stopwatch.Start();

                status = solver.Solve(ModelConfiguration.model);
                PrintTimeElapsed(stopWatch, "Google-Or-Tool DONE: ", test);

                PostRankingCheck(test, finalConsoleOutput, maximaze, employees, jobs, solver, cb, status, ref maxEmployeesWorking, rankingWrappers, ModelConfiguration.dynamicConstraintLower, ModelConfiguration.dynamicConstraintUpper, rankingControler.rankingWrappers.Count, largeDateSet, test); //Console.WriteLine(solver.ObjectiveValue); //Console.WriteLine(solver.BestObjectiveBound);

            }
        }
    }
}