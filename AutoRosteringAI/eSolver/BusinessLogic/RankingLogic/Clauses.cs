using System;
using System.Collections.Generic;
using Google.OrTools.Sat;

namespace eSolver.AutoSolverNew.RankingLogic
{
    /// <summary>
    /// Each ranking has
    /// </summary>
    
    public class RankingWrapper
    {
        public bool isDynamic;
        public int sortOrder;
        public string rankingName;
        public float[,] rang;
        // Hours - Schedule hours
        // Job type
        public long jobTypeID;
        // Coverage


        public long preciseAmout;         // If static
        public List<Condition> conditions;  // If dynamic
    }

    /// <summary>
    /// Each period has
    /// </summary>
    public class Condition
    {
        public double upperAmout;
        public double lowerAmout;

        public List<Clasuses> clasuses;

        public List<DateTime> ListOfThaeDaysOfSepcificWeek;

        // Second depth

        public double upperAmoutImplementation;
        public double lowerAmoutImplementation;
        public List<Clasuses> clasusesSecondDepthImplement;
        public List<Clasuses> clasusesSecondDepthDynamicConstraint;
    }

    /// <summary>
    /// Each Employee has (In case of covarage - each job has) 
    /// </summary>
    /// <example>
    /// externalAmount - for jobs outside of view
    /// </example>
    public class Clasuses
    {
        public double externalAmount;
        public List<int[,]> calsusa;
        public List<long> calsusaHours;
        public IDictionary<long, long> externalJobTypes;
        public List<List<int[,]>> jobsPerDayOfTheWeek; // list of days that contains list of jobs that are on same day of the week
    }
    
}