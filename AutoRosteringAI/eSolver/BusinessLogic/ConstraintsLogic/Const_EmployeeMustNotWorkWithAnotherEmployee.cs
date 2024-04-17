using System;
using System.Collections.Generic;
using System.Linq;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using eSolver.SolverConfiguration;
using Google.OrTools.Sat;
using static eSolver.BusinessLogic.Utiles.Compared;

namespace eSolver.BusinessLogic.ConstraintsLogic
{
    public static class Const_EmployeeMustNotWorkWithAnotherEmployee
    {
        /// <summary>
        /// 3.1.7  Employee must not work with another employee
        /// </summary>
        public static void ApplyEmployeeMustNotWorkWithAnotherEmployee(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, ScheduleActiveConstraint scheduleActiveConstraint, List<List<long>> finalPairsOfIDs, List<JobFromOutside> jobsFromOutside)
        {
            {
                int idA, idB;
                FaidEmployeeWithIDs(employees, scheduleActiveConstraint, out idA, out idB);

                if (idA != -1 || idB != -1)
                {
                    switch (scheduleActiveConstraint.ConstraintEMNWWAEs.ReferenceArea)
                    {
                        case "Job": // Same job
                            {
                                ReferenceAreaJob(matrix, model, jobs, idA, idB);
                                break;
                            }
                        case "Subgroup": // Same sub-schedule
                            {
                                ReferenceAreaSubGroup(matrix, model, jobs, finalPairsOfIDs, idA, idB);
                                break;
                            }
                        case "Schedule": // Same schedule
                            {
                                ReferenceAreaSchedule(matrix, model, jobs, finalPairsOfIDs, idA, idB);
                                break;
                            }
                        case "All Schedules": // All schedule
                            {
                                ReferenceAreaAllSchedules(matrix, model, jobs, scheduleActiveConstraint, finalPairsOfIDs, jobsFromOutside, idA, idB);
                            }
                            break;
                    }
                }
            }
        }

        private static void FaidEmployeeWithIDs(List<Employee> employees, ScheduleActiveConstraint scheduleActiveConstraint, out int idA, out int idB)
        {
            idA = -1;
            idB = -1;
            for (int i = 0; i < employees.Count; i++)
            {
                if (employees[i].Id == scheduleActiveConstraint.ConstraintEMNWWAEs.EmployeeAID)
                {
                    idA = i;
                }

                if (employees[i].Id == scheduleActiveConstraint.ConstraintEMNWWAEs.EmployeeBID)
                {
                    idB = i;
                }
            }

            IList<List<int>> finalPairs = new List<List<int>>();
        }

        private static void ReferenceAreaAllSchedules(ILiteral[,] matrix, CpModel model, List<ScheduleJob> jobs, ScheduleActiveConstraint scheduleActiveConstraint, List<List<long>> finalPairsOfIDs, List<JobFromOutside> jobsFromOutside, int idA, int idB)
        {

                IList<List<int>> finalPairs = transoframIDtoIndex(finalPairsOfIDs, jobs);
                AddSelfOverLapping(finalPairs, jobs.Count);                

                if (jobsFromOutside != null)
                {
                    foreach (JobFromOutside jobFromOutside in jobsFromOutside)
                    {
                        List<int> empBSouldNotWork = new List<int>();
                        List<int> empASouldNotWork = new List<int>();
                    ILiteral[] tmpMustNotWork = new ILiteral[jobs.Count() * 2];
                        foreach (int empID in jobFromOutside.EmployeeAssigments)
                            for (int j = jobs.Count() - 1; j >= 0; j--)
                            {
                                if (IsPeriodsOverlaps(jobFromOutside.JobStartDate, jobFromOutside.JobEndDate, jobs[j].JobStartDate, jobs[j].JobEndDate))
                                {
                                    if (scheduleActiveConstraint.ConstraintEMNWWAEs.EmployeeBID == empID)
                                    {
                                        empASouldNotWork.Add(j);
                                    }
                                }
                                if (IsPeriodsOverlaps(jobs[j].JobStartDate, jobs[j].JobEndDate, jobFromOutside.JobStartDate, jobFromOutside.JobEndDate))
                                {
                                    if (scheduleActiveConstraint.ConstraintEMNWWAEs.EmployeeAID == empID)
                                    {
                                        empBSouldNotWork.Add(j);
                                    }
                                }
                            }
                        int counter = 0;

                        for (int i = 0; i < empBSouldNotWork.Count; i++)
                        {
                            tmpMustNotWork[counter++] = matrix[idB, empBSouldNotWork[i]];
                        }

                        for (int j = 0; j < empASouldNotWork.Count; j++)
                        {
                            tmpMustNotWork[counter++] = matrix[idA, empASouldNotWork[j]];
                        }

                        if (empASouldNotWork.Count > 0)
                        {
                            model.Add(LinearExpr.Sum(tmpMustNotWork) == 0);
                        }
                    }
                }

            // Find all Mentors - outside od range [BID]
            foreach (List<int> overlappedJobs in finalPairs)
            {
                for (int smaller = 0; smaller < overlappedJobs.Count(); smaller++)
                {
                    ILiteral[] tmpA = new ILiteral[overlappedJobs.Count()];
                    for (int greater = 0; greater < overlappedJobs.Count(); greater++)
                    {
                        {
                            if (IsPeriodsOverlaps(jobs[overlappedJobs[greater]].JobStartDate,
                                jobs[overlappedJobs[greater]].JobEndDate,
                                jobs[overlappedJobs[smaller]].JobStartDate,
                                jobs[overlappedJobs[smaller]].JobEndDate))
                            {
                                tmpA[greater] = matrix[idB, overlappedJobs[greater]];
                            }
                        }
                    }

                    model.Add(LinearExpr.Sum(tmpA) == 0).OnlyEnforceIf(matrix[idA, overlappedJobs[smaller]]); // 21/05
                }
            }
        }

        private static void ReferenceAreaSchedule(ILiteral[,] matrix, CpModel model, List<ScheduleJob> jobs, List<List<long>> finalPairsOfIDs, int idA, int idB)
        {
            if (idA != -1 && idB != -1)
            {
                IList<List<int>> finalPairs = transoframIDtoIndex(finalPairsOfIDs, jobs);
                AddSelfOverLapping(finalPairs, jobs.Count);

                foreach (List<int> overlappedJobs in finalPairs)
                {
                    for (int smaller = 0; smaller < overlappedJobs.Count(); smaller++)
                    {
                        ILiteral[] tmpA = new ILiteral[overlappedJobs.Count()];
                        for (int greater = 0; greater < overlappedJobs.Count(); greater++)
                        {
                            {
                                if (IsPeriodsOverlaps(jobs[overlappedJobs[greater]].JobStartDate,
                                    jobs[overlappedJobs[greater]].JobEndDate,
                                    jobs[overlappedJobs[smaller]].JobStartDate,
                                    jobs[overlappedJobs[smaller]].JobEndDate))
                                {
                                    tmpA[greater] = matrix[idB, overlappedJobs[greater]];
                                }
                            }
                        }
                        model.Add(LinearExpr.Sum(tmpA) == 0).OnlyEnforceIf(matrix[idA, overlappedJobs[smaller]]);
                    }
                }
            }
        }

        private static void ReferenceAreaSubGroup(ILiteral[,] matrix, CpModel model, List<ScheduleJob> jobs, List<List<long>> finalPairsOfIDs, int idA, int idB)
        {
            if (idA != -1 && idB != -1)
            {
                IList<List<int>> finalPairs = transoframIDtoIndex(finalPairsOfIDs, jobs);
                AddSelfOverLapping(finalPairs, jobs.Count);

                foreach (List<int> overlappedJobs in finalPairs)
                {
                    IDictionary<int, int> dict = new Dictionary<int, int>();
                    int key = 0;
                    for (int j = 0; j < overlappedJobs.Count(); j++)
                    {
                        if (dict != null)
                        {
                            bool exist = false;
                            foreach (KeyValuePair<int, int> item in dict)
                            {
                                if (item.Key == jobs[overlappedJobs[j]].SubGroupID)
                                {
                                    exist = true;
                                    break;
                                }
                            }

                            if (!exist)
                            {
                                dict.Add((int)jobs[overlappedJobs[j]].SubGroupID, key++);
                            }
                        }
                    }

                    for (int smaller = 0; smaller < overlappedJobs.Count(); smaller++)
                    {
                        List<ILiteral[]> tmpA = new List<ILiteral[]>();
                        for (int j = 0; j < dict.Count(); j++)
                        {
                            tmpA.Add(new ILiteral[overlappedJobs.Count()]);
                        }
                        for (int greater = 0; greater < overlappedJobs.Count(); greater++)
                        {
                            {
                                if (IsPeriodsOverlaps(jobs[overlappedJobs[greater]].JobStartDate,
                                    jobs[overlappedJobs[greater]].JobEndDate,
                                    jobs[overlappedJobs[smaller]].JobStartDate,
                                    jobs[overlappedJobs[smaller]].JobEndDate))
                                {
                                    tmpA[dict[(int)jobs[overlappedJobs[greater]].SubGroupID]][greater] = matrix[idB, overlappedJobs[greater]];
                                }
                            }
                        }
                        model.Add(LinearExpr.Sum(tmpA[dict[(int)jobs[overlappedJobs[smaller]].SubGroupID]]) == 0).OnlyEnforceIf(matrix[idA, overlappedJobs[smaller]]);
                    }
                }
            }
        }

        private static void ReferenceAreaJob(ILiteral[,] matrix, CpModel model, List<ScheduleJob> jobs, int idA, int idB)
        {
            if (idA != -1 && idB != -1)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    model.Add((IntVar)matrix[idB, j] == 0).OnlyEnforceIf(matrix[idA, j]);
                }
            }
        }

        static void AddSelfOverLapping(IList<List<int>> finalPairs, int count)
        {
            int[] overLapped = new int[count];
            foreach (var pair in finalPairs)
            {
                foreach (int jobPOsition in pair)
                {
                    overLapped[jobPOsition] = -1;
                }
            }

            for (int i = 0; i < overLapped.Count(); i++)
            {
                if (overLapped[i] == 0)
                {
                    List<int> singleJob = new List<int>();
                    singleJob.Add(i);
                    finalPairs.Add(singleJob);
                }
            }

        }

        static List<List<int>> transoframIDtoIndex(List<List<long>> finalPairs, List<ScheduleJob> jobs)
        {
            List<List<int>> indexfinalPairs = new List<List<int>>();
            foreach (List<long> pair in finalPairs)
            {
                List<int> indexPair = new List<int>();
                foreach (int jobID in pair)
                {
                    indexPair.Add(ModelConfiguration.getPositionOfTheJobFromJobID(jobID, jobs));
                }
                indexfinalPairs.Add(indexPair);
            }
            return indexfinalPairs;
        }

        static bool IsPeriodSubIneterval(DateTime sDateOne, DateTime eDateOne, DateTime sDateTwo, DateTime eDateTwo)
        {
            return sDateOne <= sDateTwo && eDateOne >= eDateTwo;
            //return sDateTwo <= sDateOne && eDateOne <= eDateTwo;
        }


        static bool IsPeriodsOverlaps(DateTime sDateOne, DateTime eDateOne, DateTime sDateTwo, DateTime eDateTwo)
        {
            return ((sDateOne <= sDateTwo) && (sDateTwo <= eDateOne))
                || ((sDateTwo <= sDateOne) && (sDateOne <= eDateTwo));
            //return sDateOne <= eDateTwo && sDateTwo <= eDateOne;
        }


    }
}