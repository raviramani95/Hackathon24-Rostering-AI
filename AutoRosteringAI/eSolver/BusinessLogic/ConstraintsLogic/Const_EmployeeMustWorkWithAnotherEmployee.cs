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
    public static class Const_EmployeeMustWorkWithAnotherEmployee
    {
        /// <summary>
        /// 3.1.6  Employee must work with another employee
        /// </summary>
        public static void ApplEmployeeMustWorkWithAnotherEmployee(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, ScheduleActiveConstraint scheduleActiveConstraint, List<List<long>> finalPairsOfIDs, List<JobFromOutside> jobsFromOutside)
        {
            int idA, idB;
            IList<List<int>> finalPairs;
            FindEmployeeWithIDs(employees, scheduleActiveConstraint, out idA, out idB, out finalPairs);

            if (idA != -1 || idB != -1)
            {
                switch (scheduleActiveConstraint.ConstraintEMWWAEs.ReferenceArea)
                {
                    case "Job": // Same job
                        {
                            ReferenceAreaJob(matrix, model, jobs, idA, idB);
                            break;
                        }
                    case "Subgroup": // Same sub-schedule
                        {
                            ReferenceAreaSubGroup(matrix, model, jobs, finalPairsOfIDs, idA, idB, finalPairs);
                            break;
                        }
                    case "Schedule": // Same schedule
                        {
                            ReferenceAreaSchedule(matrix, model, jobs, finalPairsOfIDs, idA, idB, finalPairs);
                            break;
                        }
                    case "All Schedules": // All schedule
                        {
                            ReferenceAreaScheduleAllSchedules(matrix, model, jobs, scheduleActiveConstraint, finalPairsOfIDs, jobsFromOutside, ref idA, ref idB);
                            break;
                        }
                }
            }
        }

        private static void FindEmployeeWithIDs(List<Employee> employees, ScheduleActiveConstraint scheduleActiveConstraint, out int idA, out int idB, out IList<List<int>> finalPairs)
        {
            idA = -1;
            idB = -1;

            // A i B employees are SWITCHED!
            for (int i = 0; i < employees.Count; i++)
            {
                if (employees[i].Id == scheduleActiveConstraint.ConstraintEMWWAEs.EmployeeAID)
                {
                    idB = i;
                }

                if (employees[i].Id == scheduleActiveConstraint.ConstraintEMWWAEs.EmployeeBID)
                {
                    idA = i;
                }
            }
            finalPairs = new List<List<int>>();
        }

        private static void ReferenceAreaScheduleAllSchedules(ILiteral[,] matrix, CpModel model, List<ScheduleJob> jobs, ScheduleActiveConstraint scheduleActiveConstraint, List<List<long>> finalPairsOfIDs, List<JobFromOutside> jobsFromOutside, ref int idA, ref int idB)
        {
            IList<List<int>> finalPairs = transoframIDtoIndex(finalPairsOfIDs, jobs);
            AddSelfOverLapping(finalPairs, jobs.Count);

            //TODO Switch them back
            int tmp = idA;
            idA = idB;
            idB = tmp;

            List<int> jobsIDNeedsMentor = new List<int>();

            // Find all Mentors - outside of the schedule [BID]
            if (jobsFromOutside != null)
            {
                foreach (JobFromOutside jobFromOutside in jobsFromOutside)
                {
                    List<int> empBSouldWork = new List<int>();


                    for (int j = jobs.Count() - 1; j >= 0; j--)
                    {

                        if (IsPeriodSubIneterval(jobFromOutside.JobStartDate, jobFromOutside.JobEndDate, jobs[j].JobStartDate, jobs[j].JobEndDate))
                        {
                            //if (scheduleActiveConstraint.EmployeeAID == employeeAssigment.EmployeeID) 20.5
                            foreach (int empID in jobFromOutside.EmployeeAssigments)
                            {
                                if (scheduleActiveConstraint.ConstraintEMWWAEs.EmployeeBID == empID)
                                {
                                    empBSouldWork.Add(j);
                                }
                            }
                        }
                    }

                    ILiteral[] tmpMustWork = new ILiteral[empBSouldWork.Count];

                    for (int i = 0; i < empBSouldWork.Count; i++)
                    {
                        //tmpMustWork[counter++] = matrix[idA, empBSouldWork[i]];
                        tmpMustWork[i] = matrix[idA, empBSouldWork[i]];
                    }

                    if (empBSouldWork.Count > 0)
                    {
                        model.Add(LinearExpr.Sum(tmpMustWork) >= 1);
                    }
                }
            }


            // Find all Mentors - outside od range [AID]
            foreach (List<int> overlappedJobs in finalPairs)
            {
                //IntVar[] tmpA = new IntVar[overlappedJobs.Count()];

                if (jobsFromOutside != null)
                {
                    foreach (JobFromOutside jobFromOutside in jobsFromOutside)
                    {
                        for (int j = overlappedJobs.Count() - 1; j >= 0; j--)
                        {
                            foreach (int empID in jobFromOutside.EmployeeAssigments)
                            {
                                if (scheduleActiveConstraint.ConstraintEMWWAEs.EmployeeAID == empID)
                                {
                                    overlappedJobs.Remove(overlappedJobs[j]);
                                    goto nextAssigment;
                                }
                            }
                        nextAssigment:;
                        }
                    }
                }

                for (int smaller = 0; smaller < overlappedJobs.Count(); smaller++)
                {
                    ILiteral[] tmpA = new ILiteral[overlappedJobs.Count()];
                    for (int greater = 0; greater < overlappedJobs.Count(); greater++)
                    {
                        {
                            if (IsPeriodSubIneterval(jobs[overlappedJobs[greater]].JobStartDate,
                                jobs[overlappedJobs[greater]].JobEndDate,
                                jobs[overlappedJobs[smaller]].JobStartDate,
                                jobs[overlappedJobs[smaller]].JobEndDate))
                            {
                                tmpA[greater] = matrix[idA, overlappedJobs[greater]];
                            }
                        }
                    }
                    model.Add(LinearExpr.Sum(tmpA) >= 1).OnlyEnforceIf(matrix[idB, overlappedJobs[smaller]]);
                }
            }
        }

        private static void ReferenceAreaSchedule(ILiteral[,] matrix, CpModel model, List<ScheduleJob> jobs, List<List<long>> finalPairsOfIDs, int idA, int idB, IList<List<int>> finalPairs)
        {
            if (idA != -1 && idB != -1)
            {
                finalPairs = transoframIDtoIndex(finalPairsOfIDs, jobs);
                AddSelfOverLapping(finalPairs, jobs.Count);

                foreach (List<int> overlappedJobs in finalPairs)
                {
                    for (int smaller = 0; smaller < overlappedJobs.Count(); smaller++)
                    {
                        ILiteral[] tmpA = new ILiteral[overlappedJobs.Count()];
                        for (int greater = 0; greater < overlappedJobs.Count(); greater++)
                        {
                            if (IsPeriodSubIneterval(jobs[overlappedJobs[greater]].JobStartDate,
                                jobs[overlappedJobs[greater]].JobEndDate,
                                jobs[overlappedJobs[smaller]].JobStartDate,
                                jobs[overlappedJobs[smaller]].JobEndDate))
                            {
                                tmpA[greater] = matrix[idB, overlappedJobs[greater]];
                            }
                        }
                        model.Add(LinearExpr.Sum(tmpA) >= 1).OnlyEnforceIf(matrix[idA, overlappedJobs[smaller]]);
                    }
                }
            }
            else if (idA != -1)
            {
                EmployeeNotFound(matrix, model, jobs, idA);
            }
        }

        private static void EmployeeNotFound(ILiteral[,] matrix, CpModel model, List<ScheduleJob> jobs, int idA)
        {
            for (int j = 0; j < jobs.Count; j++)
            {
                model.Add((IntVar)matrix[idA, j] == 0);
            }
        }

        private static void ReferenceAreaSubGroup(ILiteral[,] matrix, CpModel model, List<ScheduleJob> jobs, List<List<long>> finalPairsOfIDs, int idA, int idB, IList<List<int>> finalPairs)
        {
            if (idA != -1 && idB != -1)
            {
                finalPairs = transoframIDtoIndex(finalPairsOfIDs, jobs);
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
                                if (IsPeriodSubIneterval(jobs[overlappedJobs[greater]].JobStartDate,
                                    jobs[overlappedJobs[greater]].JobEndDate,
                                    jobs[overlappedJobs[smaller]].JobStartDate,
                                    jobs[overlappedJobs[smaller]].JobEndDate))
                                {
                                    tmpA[dict[(int)jobs[overlappedJobs[greater]].SubGroupID]][greater] = matrix[idB, overlappedJobs[greater]];
                                }
                            }
                        }
                        model.Add(LinearExpr.Sum(tmpA[dict[(int)jobs[overlappedJobs[smaller]].SubGroupID]]) >= 1).OnlyEnforceIf(matrix[idA, overlappedJobs[smaller]]);
                    }
                }
            }
            else if (idA != -1)
            {
                EmployeeNotFound(matrix, model, jobs, idA);
            }
        }

        private static void ReferenceAreaJob(ILiteral[,] matrix, CpModel model, List<ScheduleJob> jobs, int idA, int idB)
        {

            if (idA != -1 && idB != -1)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    model.Add((IntVar)matrix[idB, j] == 1).OnlyEnforceIf(matrix[idA, j]);
                }
            }
            else if (idA != -1)
            {
                EmployeeNotFound(matrix, model, jobs, idA);
            }
            //model.Add(matrix[idB, j] == 1).OnlyEnforceIf(matrix[idA, j]);
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
                foreach (long jobID in pair)
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
        }

    }

}