using System;
using System.Collections.Generic;
using eSolver.Entities;
using static eSolver.AutoSolverNew.RankingLogic.Ranking_EmployeeField;
using static eSolver.AutoSolverNew.RankingLogic.Ranking_ScheduledHours;
using static eSolver.AutoSolverNew.RankingLogic.Ranking_JobFreq;
using static eSolver.AutoSolverNew.RankingLogic.Ranking_WeekFreq;
using static eSolver.AutoSolverNew.RankingLogic.Ranking_DefaultJob;
using static eSolver.AutoSolverNew.RankingLogic.Ranking_Skills;
using static eSolver.BusinessLogic.Utiles.Compared;
using eSolver.Entities.OutSideView;
using eSolver.Entities.Interfaces;
using eSolver.AutoSolverNew.RankingLogic;

namespace eSolver.BusinessLogic.Managers
{
    public class RankingManager
    {
        private List<Employee> employees;
        private List<ScheduleJob> jobs;
        private List<ScheduleRanking> scheduleRankings;
        private List<RankingRule> rankingRules;
        private List<RankingType> rankingTypes;
        private PayPeriodDTO payPeriodDTO;
        private string startDayOfTheWeek;
        private List<ScheduleCustomData> scheduleCustomData;
        private List<JobTypeNonEssentialSkill> nonEssentialSkills;
        private List<SkillMatrix> skillMatrixList;
        private List<JobFromOutside> jobFromOutsides;

        public List<Tuple<int, JobFromOutside>> assignedEmployeeJob;
        IDictionary<int, int> dict = new Dictionary<int, int>();

        public int maxSortOrder;

        // TODO
        public List<RankingWrapper> rankingWrappers;

        public RankingManager(List<Employee> employees, List<ScheduleJob> jobs, List<ScheduleRanking> scheduleRankings, List<RankingRule> rankingRules, List<RankingType> rankingTypes, PayPeriodDTO payPeriodDTO, string startDayOfTheWeek, List<RankingWrapper> rankingWrappers, List<JobFromOutside> jobsFromOutside, List<JobTypeNonEssentialSkill> nonEssentialSkills, List<SkillMatrix> skillMatrixList)
        {
            scheduleCustomData = new List<ScheduleCustomData>();
            this.employees = employees;
            this.jobs = jobs;
            this.scheduleRankings = scheduleRankings;
            this.rankingRules = rankingRules;
            this.rankingTypes = rankingTypes;
            this.payPeriodDTO = payPeriodDTO;
            this.startDayOfTheWeek = startDayOfTheWeek;
            this.rankingWrappers = rankingWrappers;
            this.maxSortOrder = 1;
            this.nonEssentialSkills = nonEssentialSkills;
            this.skillMatrixList = skillMatrixList;
            this.jobFromOutsides = jobsFromOutside;
            OutOfView(employees, jobs, jobsFromOutside);
        }

        private void OutOfView(List<Employee> employees, List<ScheduleJob> jobs, List<JobFromOutside> jobsFromOutside)
        {
            this.assignedEmployeeJob = new List<Tuple<int, JobFromOutside>>();

            if (jobsFromOutside != null && employees != null)
            {
                for (int a = 0; a < jobsFromOutside.Count; a++)
                {
                    for (int e = 0; e < employees.Count; e++)
                    {
                        for (int j = 0; j < jobsFromOutside[a].EmployeeAssigments.Count; j++)
                        {
                            if (employees[e].Id == jobsFromOutside[a].EmployeeAssigments[j])
                            {
                                this.assignedEmployeeJob.Add(Tuple.Create(e, jobsFromOutside[a]));
                            }
                        }
                    }
                }
            }

            int key = 0;
            for (int j = 0; j < jobs.Count; j++)
            {
                if (dict != null)
                {
                    //Console.WriteLine("-----");
                    bool exist = false;
                    foreach (KeyValuePair<int, int> item in dict)
                    {
                        //Console.WriteLine("Key: {0}, Value: {1}", item.Key, item.Value);
                        if (item.Key == jobs[j].JobTypeID)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        //Console.WriteLine("Adding " + jobs[j].JobtypeID);
                        this.dict.Add((int)jobs[j].JobTypeID, key++);
                    }

                }
            }

            if (jobsFromOutside != null)
            {
                for (int j = 0; j < jobsFromOutside.Count; j++)
                {
                    if (this.dict != null)
                    {
                        //Console.WriteLine("-----");
                        bool exist = false;
                        foreach (KeyValuePair<int, int> item in this.dict)
                        {
                            //Console.WriteLine("Key: {0}, Value: {1}", item.Key, item.Value);
                            if (item.Key == jobsFromOutside[j].JobTypeID)
                            {
                                exist = true;
                                break;
                            }
                        }

                        if (!exist)
                        {
                            //Console.WriteLine("Adding " + jobsForSolvers[j].JobTypeId);
                            this.dict.Add((int)jobsFromOutside[j].JobTypeID, key++);
                        }

                    }
                }
            }
        }

        internal static int CountAllCOnditions(List<RankingWrapper> rankingWrappers)
        {
            int count = 0;
            foreach (var rankingWrapper in rankingWrappers)
            {
                if (rankingWrapper.isDynamic == true)
                {
                    count += rankingWrapper.conditions.Count;
                }
            }
            return count;
        }

        internal static int CountStartingIndexOfDynamicConstraintForDynamicConstraint(List<RankingWrapper> rankingWrappers, int turnDynamicConstrint)
        {
            int count = 0;
            for (int turn = 0; turn < turnDynamicConstrint; turn++)
            {
                if (rankingWrappers[turn].isDynamic == true)
                {
                    count += rankingWrappers[turn].conditions.Count;
                }
            }
            return count;
        }

        public void FindIndexes(int[,] availabilityMatrix)
        {
            if (rankingRules != null && employees != null && rankingTypes != null && scheduleRankings != null)
            {
                foreach (ScheduleRanking scheduleRanking in scheduleRankings)
                {
                    if ((scheduleRanking.RankingTypeID != null) && (scheduleRanking.RankingRuleID != null))
                    {
                        foreach (RankingType rankingType in rankingTypes)
                        {
                            if (rankingType.Id == scheduleRanking.RankingTypeID)
                            {
                                foreach (RankingRule rankingRule in rankingRules)
                                {
                                    if (rankingRule.Id == scheduleRanking.RankingRuleID)
                                    {
                                        maxSortOrder = maxSortOrder > scheduleRanking.SortOrder ? maxSortOrder : (int) scheduleRanking.SortOrder;
                                        RankingWrapper rankingWrapper = new RankingWrapper();
                                        switch (rankingType.Name)
                                        {
                                            case "Default job":
                                                rankingWrapper.rankingName = "Default";
                                                rankingWrapper.sortOrder = (int)scheduleRanking.SortOrder;
                                                rankingWrapper.isDynamic = false;
                                                FindForDefaultJob(rankingWrapper, scheduleRanking, rankingRule, employees, jobs, rankingRule.ReverseOrder, scheduleRanking.SortOrder, scheduleCustomData, rankingWrappers);
                                                break;

                                            case "Job Type Frequency":
                                                List<long> AllJobTypes = FindAllJobTypes(employees, jobs, jobFromOutsides);
                                                FindForJobFreq(scheduleRanking, rankingRule, AllJobTypes, scheduleCustomData, rankingWrappers, employees, jobs, assignedEmployeeJob, dict, payPeriodDTO, startDayOfTheWeek);
                                                break;

                                            case "Day of week frequency":
                                                if (rankingRule.DayOfWeek != null)
                                                {
                                                    rankingWrapper.rankingName = "Week Freq";
                                                    rankingWrapper.sortOrder = (int)scheduleRanking.SortOrder;
                                                    rankingWrapper.isDynamic = true;
                                                    FindForWeekFreq(rankingWrapper, scheduleRanking, rankingRule, scheduleCustomData, rankingWrappers, employees, jobs, assignedEmployeeJob, dict, payPeriodDTO, startDayOfTheWeek);
                                                }
                                                break;

                                            case "Scheduled Hours":
                                                rankingWrapper.rankingName = "Hours";
                                                rankingWrapper.sortOrder = (int)scheduleRanking.SortOrder;
                                                rankingWrapper.isDynamic = true;
                                                FindForScheduledHours(rankingWrapper, scheduleRanking, rankingRule, availabilityMatrix, scheduleCustomData, rankingWrappers, employees, jobs, assignedEmployeeJob, payPeriodDTO, startDayOfTheWeek);
                                                break;

                                            case "Employee field value":
                                                rankingWrapper.rankingName = "Employee Field";
                                                rankingWrapper.sortOrder = (int)scheduleRanking.SortOrder;
                                                rankingWrapper.isDynamic = false;
                                                FindForEmployeeField(rankingWrapper, scheduleRanking, rankingRule, employees, jobs, rankingRule.ReverseOrder, scheduleRanking.SortOrder, scheduleCustomData, rankingWrappers);
                                                break;

                                            case "Skills":
                                                rankingWrapper.rankingName = "Skills";
                                                rankingWrapper.sortOrder = (int)scheduleRanking.SortOrder;
                                                rankingWrapper.isDynamic = false;
                                                FindSkillEmployee(rankingWrapper, scheduleRanking, rankingRule, employees, jobs, rankingRule.ReverseOrder, scheduleRanking.SortOrder, scheduleCustomData, rankingWrappers, nonEssentialSkills, skillMatrixList);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Method will return all distinct JobTypes from passed Collection
        /// </summary>
        /// <param name="jobBases">Collection of type IJobBase</param>
        /// <returns></returns>
        public static List<long> FindAllJobTypes<T>(List<Employee> employees, List<T> jobBases, List<JobFromOutside> jobsFromOutsides) where T : IJobBase
        {

            // TODO Optimize - find somewhere earlier while iterating 
            List<long> retValue = new List<long>();
            foreach (IJobBase job in jobBases)
            {
                if (!retValue.Contains(job.JobTypeID))
                {
                    retValue.Add(job.JobTypeID);
                }
            }

            foreach(IJobBase outsideJob in jobsFromOutsides)
            {
                if (!retValue.Contains(outsideJob.JobTypeID))
                {
                    retValue.Add(outsideJob.JobTypeID);
                }
            }

			//ROST-10021-Employees were not getting assigned when ranking is present
			//Commenting the code for now
            /*foreach (Employee employee in employees)
            {
                if (employee.JobTypeID != null)
                {
                    if (!retValue.Contains((long)employee.JobTypeID))
                    {
                        retValue.Add((long)employee.JobTypeID);
                    }
                }
            }*/

            return retValue;
        }
        public void SortPerOrder()
        {
            rankingWrappers.Sort((x, y) => x.sortOrder.CompareTo(y.sortOrder));
        }

        public void ConverageIndexes(int[,] availabilityMatrix)
        {
            RankingWrapper rankingWrapper = new RankingWrapper();
            rankingWrapper.conditions = new List<Condition>();

            Condition condition = new Condition();
            condition.clasuses = new List<Clasuses>(jobs.Count);
            
            for (int j = 0; j < jobs.Count; j++)
            {
                condition.clasuses.Add(new Clasuses());
                condition.clasuses[j].calsusa = new List<int[,]>();
                for (int e = 0; e < employees.Count; e++)
                {
                    if (availabilityMatrix[e, j] >= -1)
                    {
                        condition.clasuses[j].calsusa.Add(new int[e, j]);
                    }
                }
            }

            rankingWrapper.conditions.Add(condition);
            rankingWrapper.isDynamic = true;
            rankingWrapper.rankingName = "Coverage";
            rankingWrappers.Add(rankingWrapper);
            
        }


    }

}
