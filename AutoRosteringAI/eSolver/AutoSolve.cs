using eSolver.AutoSolverNew.RankingLogic;
using eSolver.Entities;
using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

using static eSolver.SolverConfiguration.InitSetUp;
using static eSolver.Utiles;
using eSolver.Entities.Interfaces;
using eSolver.SolverConfiguration;
using eSolver.Entities.OutSideView;
using eSolver.BusinessLogic.Managers;
using eSolver.Entities.Responses;

namespace eSolver
{
    public class AutoSolve: AutoSolveBase, IDisposable
    {
        /// <summary>
        /// Framework for solution control
        /// </summary>
        public SolverSolutionObserver cb;

        public AutoSolve()
        {
        }

        /// <summary>
        /// Main method for solving linear problem
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public SolversResponse Solve(string json)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Map passed JSON to domain model 
            AutoSolveRequest request = JsonConvert.DeserializeObject<AutoSolveRequest>(json);

            // If something is NULL do nothing
            if (request.Schedule == null || request.Schedule.ScheduleJobs.Count == 0 || request.Employees.Count == 0 )
                return null;

            InitBasicSetupOfModel(request, out JobManager preprocessing, out SolversResponse autoSolveResponse,
                out bool test, out bool setRestictedNumberOfEmployees, out bool finalConsoleOutput,
                out List<Employee> employees, out List<ScheduleJob> jobs, out PayPeriodDTO payPeriodDTO,
                out string StartDayOfTheWeek, out ConstraintManager preCostraints,
                out int maxNumOfJobsAtSameTime, out bool maximaze,
                out float maxTime, out float maxTimeRanking, out List<RankingWrapper> rankingWrappers,
                out bool implement, out decimal token, out List<JobFromOutside> jobsFromOutside, out bool largeDateSet, out bool? canChangeEmployeeOnTemplateJobs);

            jobs = JobsPreprocess(preprocessing, test, employees, jobs, out List<List<long>> finalPairs, out List<ScheduleJob> jobsNonSorted);
            MapJobs(request.JobOutsides);

            string fixedSearch = "search_branching:FIXED_SEARCH, ";
            string hintingSearch = "search_branching:FIXED_SEARCH, ";  //"search_branching:HINT_SEARCH, ";
            //string wayOfSearchSecond = "search_branching:FIXED_SEARCH, ";

            RankingManager rankingControler = new RankingManager(employees, jobs, request.ScheduleRankings, request.RankingRules, request.RankingTypes, payPeriodDTO, StartDayOfTheWeek, rankingWrappers, jobsFromOutside, request.NonEssentialSkills, request.SkillMatrixList);

            IEnumerable<int> setOfJobs = Enumerable.Range(0, jobs.Count);
            IEnumerable<int> setOfEmployees = Enumerable.Range(0, employees.Count);
            
            PrintTimeElapsed(stopWatch, "Init time DONE: ", test);

            #if DEBUG
                // For the development purpose
                //eSolver.Statistic.FileWriter.FileClearText();
                test = true;
            #else
                test = false;
            #endif

            // -----------------------------------------------------------------------------------

            int[,] availabilityMatrix = new int[employees.Count, jobs.Count];
            ModelConfiguration modelConfiguration;
            CpSolver solver;
            HashSet<int> toPrint;
            CpSolverStatus status;
            int maxEmployeesWorking = -1;

            modelConfiguration = ModelPreprocessing(setRestictedNumberOfEmployees, employees, jobs, maxNumOfJobsAtSameTime, rankingControler, finalPairs, jobsNonSorted, setOfJobs, setOfEmployees, availabilityMatrix, jobsFromOutside, false);

            PrintTimeElapsed(stopWatch, "CheckRankingConditions DONE: ", test);
            rankingControler.FindIndexes(availabilityMatrix);
            PrintTimeElapsed(stopWatch, "FindIndexes DONE: ", test);
            rankingControler.SortPerOrder();
            PrintTimeElapsed(stopWatch, "SortPerOrder DONE: ", test);
            if (!largeDateSet)
            {
                rankingControler.ConverageIndexes(availabilityMatrix);
            }
            PrintTimeElapsed(stopWatch, "Dynamic model DONE: ", test);

            modelConfiguration.InitDynamic(rankingControler.rankingWrappers);



            double maxTimeFirst = largeDateSet && (rankingWrappers.Count > 0 && rankingWrappers[0].isDynamic) ? maxTime * 0.7 : maxTime;
            double maxTimeSecond = largeDateSet && (rankingWrappers.Count > 0 && rankingWrappers[0].isDynamic) ? maxTime * 0.3 : 0;

            implement = false;
            
            toPrint = new HashSet<int>();
            cb = new SolverSolutionObserver(ModelConfiguration.matrix, employees, jobs, toPrint, 0, false, null, false, ModelConfiguration.lLimit, ModelConfiguration.uLimit, ModelConfiguration.limit, ModelConfiguration.dynamicConstraintUpper, ModelConfiguration.dynamicConstraintLower, test)
            {
                stopAtFirstNonMaximizeSolution = false
            };

            if (largeDateSet)
            {
                AutoSolveLargeDate(stopWatch, request, test, setRestictedNumberOfEmployees, finalConsoleOutput, employees, jobs, payPeriodDTO, StartDayOfTheWeek, preCostraints, maxNumOfJobsAtSameTime, maximaze, maxTime, rankingWrappers, ref implement, jobsFromOutside, largeDateSet, finalPairs, jobsNonSorted, fixedSearch, rankingControler, setOfJobs, setOfEmployees, availabilityMatrix, ref modelConfiguration, out solver, ref toPrint, out status, ref maxEmployeesWorking, maxTimeSecond, ref cb, canChangeEmployeeOnTemplateJobs);
            }
            else
            {
                for (int turnForRanking = 0; turnForRanking < rankingControler.rankingWrappers.Count; turnForRanking++)
                {
                    if (!(turnForRanking > 0 && rankingWrappers[turnForRanking].rankingName == "Coverage"))
                    {
                        decimal timeForRanking;
                        maxEmployeesWorking = AutoSolvePartial(request, test, setRestictedNumberOfEmployees, finalConsoleOutput, employees, jobs, payPeriodDTO, StartDayOfTheWeek, preCostraints, maxNumOfJobsAtSameTime, maximaze, maxTime, rankingWrappers, out implement, token, jobsFromOutside, largeDateSet, finalPairs, jobsNonSorted, rankingControler, setOfJobs, setOfEmployees, availabilityMatrix, out modelConfiguration, out solver, out toPrint, out status, maxEmployeesWorking, turnForRanking, out timeForRanking, ref cb, canChangeEmployeeOnTemplateJobs);

                        AutoSolveSecondDepth(stopWatch, request, test, setRestictedNumberOfEmployees, finalConsoleOutput, employees, jobs, payPeriodDTO, StartDayOfTheWeek, preCostraints, maxNumOfJobsAtSameTime, maximaze, rankingWrappers, implement, jobsFromOutside, largeDateSet, finalPairs, jobsNonSorted, hintingSearch, rankingControler, setOfJobs, setOfEmployees, availabilityMatrix, ref modelConfiguration, ref solver, ref toPrint, ref status, ref maxEmployeesWorking, turnForRanking, timeForRanking, ref cb, canChangeEmployeeOnTemplateJobs);
                    }
                    continue;
                    //AutoSolveBruteForce(request, test, setRestictedNumberOfEmployees, finalConsoleOutput, employees, jobs, payPeriodDTO, StartDayOfTheWeek, preCostraints, maxNumOfJobsAtSameTime, maximaze, maxTimeRanking, rankingWrappers, ref implement, jobsFromOutside, largeDateSet, finalPairs, jobsNonSorted, rankingControler, setOfJobs, setOfEmployees, availabilityMatrix, ref modelConfiguration, solver, toPrint, ref status, ref maxEmployeesWorking, turnForRanking, ref cb);
                }
               
                cb.FilePrintSolution(cb.outputMatrixInt);
            }
            autoSolveResponse = PostSolver(autoSolveResponse, test, finalConsoleOutput, cb);

            PrintTimeElapsed(stopWatch, "Autosolve DONE :", true);
             return autoSolveResponse;

        }

        /// <summary>
        /// For passed list of jobs outside of AutoSolve range we will adjust hours. 
        /// This method will convert passed Hours values
        /// from long in to TimeSpan type.
        /// </summary>
        /// <param name="jobOutsides">List of jobs outside of AutoSolve Range</param>
        /// <exception cref="OverflowException">This exception will be fired if there is Hours value which can't be converted in TimeSpan</exception>
        /// <exception cref="ArgumentException">If some of parameters doesn't matching</exception>
        public void MapJobs<T>(List<T> jobBases) where T : IJobBase
        {
            // If collection is different from NULL adjust hour values
            if (jobBases != null)
            {
                for (int i = 0; i < jobBases.Count; i++)
                {
                    jobBases[i].HoursT = TimeSpan.FromSeconds(jobBases[i].Hours / jobBases[i].COUNT_OF_MILISECONDS);
                    jobBases[i].HoursT1 = TimeSpan.FromSeconds(jobBases[i].Hours1 / jobBases[i].COUNT_OF_MILISECONDS);
                    jobBases[i].HoursT2 = TimeSpan.FromSeconds(jobBases[i].Hours2 / jobBases[i].COUNT_OF_MILISECONDS);
                    jobBases[i].HoursT3 = TimeSpan.FromSeconds(jobBases[i].Hours3 / jobBases[i].COUNT_OF_MILISECONDS);
                    jobBases[i].HoursT4 = TimeSpan.FromSeconds(jobBases[i].Hours4 / jobBases[i].COUNT_OF_MILISECONDS);
                    jobBases[i].HoursT5 = TimeSpan.FromSeconds(jobBases[i].Hours5 / jobBases[i].COUNT_OF_MILISECONDS);
                    jobBases[i].HoursT6 = TimeSpan.FromSeconds(jobBases[i].Hours6 / jobBases[i].COUNT_OF_MILISECONDS);
                }
            }
        }

        #region IDisposable Implementation

        /// <summary>
        /// Pointer to an external unmanaged resource.
        /// </summary>
        private IntPtr handle;
        /// <summary>
        /// Other managed resource this class uses.
        /// </summary>
        private readonly Component component = new Component();
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Disposing IdNameDropDownModel
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, 
        /// the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. 
        /// If disposing equals false, the method has been called by the runtime from inside the finalizer and you 
        /// should not reference other objects.Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    component.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                CloseHandle(handle);
                handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }
        /// <summary>
        /// Use interop to call the method necessary to clean up the unmanaged resource.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        /// <summary>
        /// Use C# destructor syntax for finalization code. This destructor will run only if the Dispose method does not get called.
        /// It gives your base class the opportunity to finalize. Do not provide destructors in types derived from this class.
        /// </summary>
        ~AutoSolve()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

    }
}
