using eSolver.Entities;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using eSolver.AutoSolverNew.RankingLogic;
using System.Linq;
using static eSolver.Statistic.ConsoleWriter;
using eSolver.Entities.Responses;
using eSolver.BusinessLogic.Managers;

// For the development purpose
// using static eSolver.Statistic.FileWriter;

namespace eSolver.SolverConfiguration
{
    /// <summary>
    /// In each found solution this class will be invoked
    /// </summary>
    /// <return>
    /// Each solution declared in Hash toPrint is going to printed
    /// It can storage and return best solution based on dispersion error//mistake//loss
    /// </return>
    public partial class SolverSolutionObserver : CpSolverSolutionCallback
    {
        /// <summary>
        /// Constructor for SolverSolutionObserver
        /// </summary>
        /// <param name="matrix">The model from which solver is going to try to find solutions</param>
        /// <param name="employees">List of employees</param>
        /// <param name="jobs">List of sorted jobs</param>
        /// <param name = "toPrint" > Solution which are going to be displayed at the console if printSolutions has true value</param>
        /// <param name="betterSolutions">Nuber of better soultions found from the beginning of the search</param>
        public SolverSolutionObserver(ILiteral[,] matrix, List<Employee> employees, List<ScheduleJob> jobs, HashSet<int> toPrint, int betterSolutions, bool rang, int[,] nonRankedSolution, bool extraParam, IntVar[] lLimit, IntVar[] uLimit, IntVar limit, IntVar[] dynamicConstraintUpper, IntVar[] dynamicConstraintLower, bool test)
        {
            this.matrix = matrix;
            this.employees = employees;
            this.jobs = jobs;
            this.betterSolutions = betterSolutions;
            foundAtFirstNonMaximizeSolution = false;
            solutionsToPrint = toPrint;
            
            if (nonRankedSolution != null)
            {
                //this.scoreRank = checkRankingConditions.Score(nonRankedSolution, false, printSolutions, scheduleCustomData);
            }
            else
            {
                scoreRank = -1;
            }

            isRang = rang;
            this.outputMatrixInt = nonRankedSolution;
            if (nonRankedSolution != null && test)
            {
                ConsoleWriteSolution(nonRankedSolution, jobs, employees, solutionCount, mistakeOfOutputMatrix);
            }
            this.extraParam = extraParam;

            this.lLimit = lLimit;
            this.uLimit = uLimit;
            this.limit = limit;
            this.dynamicConstraintLower = dynamicConstraintLower;
            this.dynamicConstraintUpper = dynamicConstraintUpper;
        }
        
        /// <summary>
        /// It is measured time to find that solution,
        /// then it will measure loss and check if it is better solution then current one
        /// and if it is that solution will be storaged
        /// </summary>
        /// <remarks>
        /// There is one overloaded methode for printin final solution
        /// </remarks>
        /// <param name="matrix"></param>


        internal void GetHints(CpSolver solver, ILiteral[,] matrix, CpModel model)
        {
            for (int e = 0; e < employees.Count; e++)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    model.AddHint((IntVar)matrix[e,j], outputMatrixInt[e, j]);
                }
            }
        }

        /// <summary>
        /// Print solution without checking mistake
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public int PrintOneSolution(IntVar[,] matrix)
        {
            int filled = 0;
            for (int j = 0; j < jobs.Count; j++)
            {
                for (int e = 0; e < employees.Count; e++)
                {
                    Console.Write((int)Value(matrix[e, j]));
                }
                Console.WriteLine();
            }
            return filled;
        }
        
        /// <summary>
        /// Measure loss
        /// Check if there better solution
        /// Update the best solution
        /// </summary>
        public void CheckSoultion(IntVar[,] matrix)
        {
            int mistake = 0;
            for (int j = 0; j < jobs.Count; j++)
            {
                int sum = 0;
                for (int e = 0; e < employees.Count; e++)
                {
                    sum += (int)Value(matrix[e, j]);
                }
                mistake += (int)Math.Pow((int)jobs[j].NoOfEmployeesRequired - sum, 2);
            }
            if (mistake < mistakeOfOutputMatrix)
            {
                mistakeOfOutputMatrix = mistake;
                if (outputMatrixInt == null)
                {
                    outputMatrixInt = new int[employees.Count, jobs.Count];
                }
                for (int j = 0; j < jobs.Count; j++)
                {
                    for (int e = 0; e < employees.Count; e++)
                    {
                        outputMatrixInt[e, j] = (int)Value(matrix[e, j]);
                    }
                }
                betterSolutions++;
            }
        }

        /// <summary>
        /// Score each solution with rankings. 
        /// Depending on the score same the best found solution
        /// </summary>
        /// <param name="matrix"></param>
        public void RankSolution(ILiteral[,] matrix, bool onlyCoverage, bool saveMoreCounts)
        {
            int[,] intMatrix = new int[employees.Count, jobs.Count];

            for (int j = 0; j < jobs.Count; j++)
            {
                for (int e = 0; e < employees.Count; e++)
                {
                    intMatrix[e, j] = (int)Value((LinearExpr)matrix[e, j]);
                }
            }

            decimal currentScore = scoreRank + 1;
            //decimal currentScore = checkRankingConditions.Score(intMatrix, onlyCoverage, printSolutions, scheduleCustomData);
            if (saveMoreCounts)
            {
                int countAssigmentsTmp = CountAssigments(matrix);
                if (countAssigments <= countAssigmentsTmp)
                {
                    SaveSolution(matrix);
                }
            }
            else
            {

                if ((decimal)scoreRank < currentScore)
                {
                    scoreRank = currentScore;
                    SaveSolution(matrix);
                    ConsoleWriteSolution(outputMatrixInt, jobs, employees, solutionCount, mistakeOfOutputMatrix);
                }
            }
        }

        /// <summary>
        /// Current soultion is gone be saved at local parametars of the class.
        /// </summary>
        /// <param name="matrix"></param>
        public void SaveSolution(ILiteral[,] matrix)
        {
            if (outputMatrixInt == null)
            {
                outputMatrixInt = new int[employees.Count, jobs.Count];
            }
            for (int j = 0; j < jobs.Count; j++)
            {
                for (int e = 0; e < employees.Count; e++)
                {
                    outputMatrixInt[e, j] = (int)Value((LinearExpr)matrix[e, j]);
                }
            }
            betterSolutions++;
        }

        /// <summary>
        /// Count how many assigments have been found.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public int CountAssigments(ILiteral[,] matrix)
        {
            int filled = 0;
            for (int j = 0; j < jobs.Count; j++)
            {
                for (int e = 0; e < employees.Count; e++)
                {
                    filled += (int)Value((LinearExpr)matrix[e, j]);
                }
            }
            return filled; 
        }

        /// <summary>
        /// The class which are going to be invoked for each solution
        /// if it's auto-solve then it will stop searching for nest solution.
        /// </summary>
        public override void OnSolutionCallback()
        {
            if (extraParam)
            {
                Console.WriteLine("ERROR 404 - Ispis nije moguc");
            }

            if (matrix != null)
            {
                SaveSolution(matrix);
            }

            solutionCount++;
            if (isRang)
            {
                //Console.WriteLine();
                RankSolution(matrix, false, false);
            }
            else
            {
                int countAssigmentsTmp = CountAssigments(matrix);
                if (countAssigments <= countAssigmentsTmp)
                {
                    countAssigments = countAssigmentsTmp;
                    RankSolution(matrix, false, true);
                }
            }

            if (mistakeOfOutputMatrix == 0 || stopAtFirstNonMaximizeSolution)
            {
                foundAtFirstNonMaximizeSolution = true;
                
                this.StopSearch();
                this.stopwatch.Stop();
            }
        }
        
        /// <summary>
        /// Getter for current solution
        /// </summary>
        /// <returns></returns>
        public int CountsSoultions()
        {
            return solutionCount;
        }

        /// <summary>
        /// Getter for currently best mistake
        /// </summary>
        /// <returns></returns>
        public int GetMistake()
        {
            return mistakeOfOutputMatrix;
        }

        /// <summary>
        /// Getter for currently best matrix solution
        /// </summary>
        /// <returns></returns>
        public int[,] GetOutPutMatrix()
        {
            return outputMatrixInt;
        }

        /// <remark>
        /// It is going to be used for initializing second solver
        /// </remark>
        /// <returns>How many employees are assigned - Sum of the matrix (-1 for not found solution)</returns>
        public int GetMaxEmployeeNumber()
        {
            if (outputMatrixInt != null)    // matrix can't be null bcz of initial atributes
            {
                int sum = 0;
                for (int j = 0; j < jobs.Count; j++)
                {
                    for (int e = 0; e < employees.Count; e++)
                    {
                        sum += (int)Value((IntVar)matrix[e, j]);
                    }
                }
                return sum;
            }
            return -1;      // matrix is null  so rest of the program decide if it's gone continue with searhing

        }

        /// <summary>
        /// Getting best solution preapered for the output
        /// </summary>
        /// <returns>Solution in format of SolutionJSON</returns>
        internal SolversResponse GetSolution(bool test)
        {
            if (test)
            {
                Console.WriteLine("Solution");
            }
            SolversResponse sjson = new SolversResponse
            {
                FilledJob = new List<FilledJob>()
            };
            for (int j = 0; j < jobs.Count; j++)
            {
                FilledJob fillJob = new FilledJob
                {
                    JobID = jobs[j].JobID,
                    AfterSortingJobID = j,
                    EmployeesID = new List<long>()
                };
                for (int e = 0; e < employees.Count; e++)
                {
                    if (outputMatrixInt[e, j] == 1)
                    {
                        fillJob.EmployeesID.Add(employees[e].Id);
                    }
                }
                sjson.FilledJob.Add(fillJob);
            }

            return sjson;
        }

        public long GetTotalSum(float[, ] RankMatrix)
        {
            long sum = 0;
            if (RankMatrix != null && outputMatrixInt!=null)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    for (int e = 0; e < employees.Count; e++)
                    {
                        if ((outputMatrixInt[e, j]) == 1){
                            sum += (long)((double)(outputMatrixInt[e, j]) * Math.Round(10000000.0 * RankMatrix[e, j]));
                        }
                    }
                }
            }
            return sum;
        }


        public void ParseSolutionBetweenSolverModels(CpSolver solver, CpSolverStatus status, ILiteral[,] matrix_out, List<AutoSolverNew.RankingLogic.RankingWrapper> rankingWrappers, IntVar[] dynamicConstraintLower, IntVar[] dynamicConstraintUpper, int maxEmployeesWorking, int turnForRanking, bool largeDateSet, bool test)
        {
            if((status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible))
            {
                SaveCrossSolution(solver, matrix_out, rankingWrappers, dynamicConstraintLower, dynamicConstraintUpper, maxEmployeesWorking, turnForRanking, largeDateSet, test);
            }
                
        }
        

        public  void SaveCrossSolution(CpSolver solver, ILiteral[,] matrix_out, List<AutoSolverNew.RankingLogic.RankingWrapper> rankingWrappers, IntVar[] dynamicConstraintLower, IntVar[] dynamicConstraintUpper, int maxEmployeesWorking, int turnForRanking, bool largeDateSet, bool test)
        {
            int total = 0;
            if (outputMatrixInt == null)
            {
                outputMatrixInt = new int[employees.Count, jobs.Count];
            }
            for (int j = 0; j < jobs.Count; j++)
            {
                for (int e = 0; e < employees.Count; e++)
                {
                    outputMatrixInt[e, j] = solver.BooleanValue(matrix_out[e, j]) ? 1 : 0;
                    total += solver.BooleanValue(matrix_out[e, j]) ? 1 : 0;
                }
            }

            matrix = matrix_out;
            betterSolutions++;

            countAssigments = total;

            if (largeDateSet)
            {
                //for (int k = 0; k < turnForRanking + 1; k++)
                for (int k = 0; k < rankingWrappers.Count; k++)
                {
                    if (rankingWrappers.Count > k) // For large dateset k can be bigger number
                    {
                        if (rankingWrappers[k].isDynamic == true)
                        {

                            for (int i = 0; i < rankingWrappers[k].conditions.Count; i++)
                            {
                                if (test)
                                {
                                    // For the development purpose
                                    //FileWriteDynamicRankingParametars(rankingWrappers, k, i, solver.Value(dynamicConstraintUpper[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]), solver.Value(dynamicConstraintLower[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]));
                                }
                                rankingWrappers[k].conditions[i].upperAmout = solver.Value(dynamicConstraintUpper[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]);
                                rankingWrappers[k].conditions[i].lowerAmout = solver.Value(dynamicConstraintLower[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]);
                            }
                        }
                        else
                        {
                            rankingWrappers[k].preciseAmout = 0;
                            for (int e = 0; e < employees.Count; e++)
                            {
                                for (int j = 0; j < jobs.Count; j++)
                                {
                                    rankingWrappers[k].preciseAmout += (long)Math.Round(Math.Round(rankingWrappers[k].rang[e, j], 4) * 10000) * (solver.BooleanValue(matrix_out[e, j]) ? 1 : 0);
                                }
                            }
                            if (test)
                            {
                                // For the development purpose
                                //FileWriteStaticRankingParametars();
                            }
                        }
                    }
                }
            }
            else
            {

                if (turnForRanking < rankingWrappers.Count()) // To debug
                {
                    if (rankingWrappers[turnForRanking].isDynamic == true)
                    {

                        for (int i = 0; i < rankingWrappers[turnForRanking].conditions.Count; i++)
                        {
                            if (test)
                            {
                                // For the development purpose
                                //FileWriteDynamicRankingParametars(rankingWrappers, k, i, solver.Value(dynamicConstraintUpper[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]), solver.Value(dynamicConstraintLower[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]));
                            }
                            rankingWrappers[turnForRanking].conditions[i].upperAmout = solver.Value(dynamicConstraintUpper[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, turnForRanking)]);
                            rankingWrappers[turnForRanking].conditions[i].lowerAmout = solver.Value(dynamicConstraintLower[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, turnForRanking)]);
                        }
                    }
                    else
                    {
                        rankingWrappers[turnForRanking].preciseAmout = 0;
                        for (int e = 0; e < employees.Count; e++)
                        {
                            for (int j = 0; j < jobs.Count; j++)
                            {
                                rankingWrappers[turnForRanking].preciseAmout += (long)Math.Round(Math.Round(rankingWrappers[turnForRanking].rang[e, j], 4) * 10000) * (solver.BooleanValue(matrix_out[e, j]) ? 1 : 0);
                            }
                        }

                        if (test)
                        {
                            // For the development purpose
                            //FileWriteStaticRankingParametars();
                        }
                    }
                }
            }
        }


        internal int HasPotentialForSecondDepth(List<RankingWrapper> rankingWrappers, CpSolver solver, CpSolverStatus status)
        {
            if (status == CpSolverStatus.Feasible || status == CpSolverStatus.Optimal)
            {
                for (int k = 0; k < rankingWrappers.Count; k++)
                {

                    if (rankingWrappers[k].isDynamic == true)
                    {
                        long minimalLower = 9999999;
                        long maximalUpper = -1;
                        for (int i = 0; i < rankingWrappers[k].conditions.Count; i++)
                        {
                            maximalUpper = solver.Value(dynamicConstraintUpper[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]) > maximalUpper ? solver.Value(dynamicConstraintUpper[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]) : maximalUpper;
                            minimalLower = solver.Value(dynamicConstraintLower[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]) < minimalLower ? solver.Value(dynamicConstraintLower[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, k)]) : minimalLower;
                        }
                        if (minimalLower + 100 < maximalUpper)
                        {
                            return k;
                        }
                    }
                }
            }
            return -1;
        }



        internal long HasPotentialForSecondDepth(List<RankingWrapper> rankingWrappers, CpSolver solver, int turnForRanking)
        {
            if (rankingWrappers[turnForRanking].isDynamic == true)
            {
                long minimalLower = 9999999;
                long maximalUpper = -1;
                for (int i = 0; i < rankingWrappers[turnForRanking].conditions.Count; i++)
                {
                    try
                    {
                        maximalUpper = solver.Value(dynamicConstraintUpper[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, turnForRanking)]) > maximalUpper ? solver.Value(dynamicConstraintUpper[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, turnForRanking)]) : maximalUpper;
                        minimalLower = solver.Value(dynamicConstraintLower[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, turnForRanking)]) < minimalLower ? solver.Value(dynamicConstraintLower[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, turnForRanking)]) : minimalLower;
                    }
                    catch
                    {
                    }
                }
                if (minimalLower + 1 < maximalUpper)
                {
                    return turnForRanking;
                }
            }
            return -1;
        }

        internal void ConsolePrintSolution(int[,] v)
        {
            // For the development purpose
            ConsoleWriteSolution(v, jobs, employees, solutionCount, mistakeOfOutputMatrix);
        }
        internal void FilePrintSolution(int[,] v)
        {
            // For the development purpose
            //FileWriteSolution(v, jobs, employees, solutionCount, mistakeOfOutputMatrix);
        }

        internal void ExtractSecondDepth(List<RankingWrapper> rankingWrappers, Google.OrTools.Sat.CpSolver solver, long k)
        {
            double difference = 0;
            int ignoringSoultions = 0;
            for (int i = 0; i < rankingWrappers[(int)k].conditions.Count; i++)
            {
                rankingWrappers[(int)k].conditions[i].upperAmoutImplementation = solver.Value(dynamicConstraintUpper[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, (int)k)]);
                rankingWrappers[(int)k].conditions[i].lowerAmoutImplementation = solver.Value(dynamicConstraintLower[i + RankingManager.CountStartingIndexOfDynamicConstraintForDynamicConstraint(rankingWrappers, (int)k)]);

                difference = rankingWrappers[(int)k].conditions[i].upperAmout - rankingWrappers[(int)k].conditions[i].lowerAmout;

                ignoringSoultions = ((int)(difference * 0.2 / 100)) * 100;

                rankingWrappers[(int)k].conditions[i].upperAmoutImplementation -= ignoringSoultions;
                rankingWrappers[(int)k].conditions[i].lowerAmoutImplementation += ignoringSoultions;

                rankingWrappers[(int)k].conditions[i].clasusesSecondDepthDynamicConstraint = new List<Clasuses>();
                rankingWrappers[(int)k].conditions[i].clasusesSecondDepthImplement = new List<Clasuses>();


                switch (rankingWrappers[(int)k].rankingName)
                {
                    case "Job Freq":
                        long[] numberOfJobTypesPerEmployee = new long[employees.Count];

                        for (int e = 0; e < rankingWrappers[(int)k].conditions[i].clasuses.Count; e++)
                        {
                            for (int j = 0; j < rankingWrappers[(int)k].conditions[i].clasuses[e].calsusa.Count; j++)
                            {
                                if (rankingWrappers[(int)k].jobTypeID == jobs[(int)rankingWrappers[(int)k].conditions[i].clasuses[e].calsusa[j].GetValue(0, 1)].JobTypeID)
                                {
                                    numberOfJobTypesPerEmployee[e] += (int)outputMatrixInt[e, (int)rankingWrappers[(int)k].conditions[i].clasuses[e].calsusa[j].GetValue(0, 1)]; // Value(matrix[e, 5]) != 0 ? DateTime.Parse(jobs[j].Hours).Hour : 0;
                                }
                            }

                            numberOfJobTypesPerEmployee[e] += rankingWrappers[(int)k].conditions[i].clasuses[e].externalJobTypes[rankingWrappers[(int)k].jobTypeID];
                        }

                        for (int p = 0; p < numberOfJobTypesPerEmployee.Length; p++)
                        {
                            if (numberOfJobTypesPerEmployee[p] * 100 > rankingWrappers[(int)k].conditions[i].lowerAmoutImplementation && numberOfJobTypesPerEmployee[p] * 100 < rankingWrappers[(int)k].conditions[i].upperAmoutImplementation)
                            {
                                rankingWrappers[(int)k].conditions[i].clasusesSecondDepthDynamicConstraint.Add(rankingWrappers[(int)k].conditions[i].clasuses[p]);
                            }
                            else
                            {
                                rankingWrappers[(int)k].conditions[i].clasusesSecondDepthImplement.Add(rankingWrappers[(int)k].conditions[i].clasuses[p]);

                            }
                        }
                        break;

                    case "Week Freq":
                        break;

                    case "Hours":
                        double[] listOfHours = new double[employees.Count];

                        for (int e = 0; e < rankingWrappers[(int)k].conditions[i].clasuses.Count; e++)
                        {
                            for (int j = 0; j < rankingWrappers[(int)k].conditions[i].clasuses[e].calsusa.Count; j++)
                            {
                                listOfHours[e] += (int)outputMatrixInt[e, (int)rankingWrappers[(int)k].conditions[i].clasuses[e].calsusa[j].GetValue(0, 1)] * jobs[(int)rankingWrappers[(int)k].conditions[i].clasuses[e].calsusa[j].GetValue(0, 1)].HoursT.TotalMinutes; // Value(matrix[e, 5]) != 0 ? DateTime.Parse(jobs[j].Hours).Hour : 0;
                            }

                            listOfHours[e] += rankingWrappers[(int)k].conditions[i].clasuses[e].externalAmount;
                        }

                        for (int p = 0; p < listOfHours.Length; p++)
                        {
                            if (listOfHours[p]  > rankingWrappers[(int)k].conditions[i].lowerAmoutImplementation && listOfHours[p]  < rankingWrappers[(int)k].conditions[i].upperAmoutImplementation)
                            {
                                rankingWrappers[(int)k].conditions[i].clasusesSecondDepthDynamicConstraint.Add(rankingWrappers[(int)k].conditions[i].clasuses[p]);
                            }
                            else
                            {
                                rankingWrappers[(int)k].conditions[i].clasusesSecondDepthImplement.Add(rankingWrappers[(int)k].conditions[i].clasuses[p]);

                            }
                        }

                        break;
                }
            }
        }

        #region Private Properties

        public int[,] outputMatrixInt; 
        public int mistakeOfOutputMatrix = Int32.MaxValue;
        public int solutionCount;
        public int betterSolutions;
        public int countAssigments;
        
        public  ILiteral[,] matrix;
        private readonly List<Employee> employees;
        private readonly List<ScheduleJob> jobs;
        protected List<ScheduleCustomData> scheduleCustomData;
        private readonly HashSet<int> solutionsToPrint;
        public bool isRang;

        public decimal scoreRank;
        //CheckRankingConditions checkRankingConditions;

        public bool stopAtFirstNonMaximizeSolution;
        public bool foundAtFirstNonMaximizeSolution;
        public bool extraParam;
        IntVar[] lLimit, uLimit;
        IntVar limit;
        //IntVar[,] dynamicCosntraints;
        IntVar[] dynamicConstraintUpper;
        IntVar[] dynamicConstraintLower;

        public Stopwatch stopwatch = new Stopwatch();
        #endregion
    }
}
