using eSolver.AutoSolverNew.RankingLogic;
using eSolver.BusinessLogic.Managers;
using eSolver.Entities;
using eSolver.Entities.Interfaces;
using eSolver.Entities.OutSideView;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;


namespace eSolver
{
    public class ModelConfiguration
    {
        /// <summary>
        /// Creating model for the solver 
        /// The model is going to be matrix of booleans with size of the lenght of the employees and jobs
        /// Then it will fill the model depending of already assigned employees and
        /// it's going to apply not already assigned - constraint.
        /// Last part is going to set character of the way of the solving [auto-solve/optimaze]
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="jobs"></param>
        /// <param name="setOfEmployees"></param>
        /// <param name="setOfJobs"></param>
        public ModelConfiguration(List<Employee> employees, List<ScheduleJob> jobs, IEnumerable<int> setOfEmployees, IEnumerable<int> setOfJobs)
        {
            matrix = new ILiteral[employees.Count, jobs.Count];
            model = new CpModel();
            this.employees = employees;
            this.jobs = jobs;
            this.setOfEmployees = setOfEmployees;
            this.setOfJobs = setOfJobs;            

            uLimit = new IntVar[employees.Count()];
            lLimit = new IntVar[employees.Count()];
            limit = model.NewIntVar(0, jobs.Count * 10000, "Upper limit");
            limitLower = model.NewIntVar(0, jobs.Count * 10000, "Upper limit2");
            limitCoverage = model.NewIntVar(0, jobs.Count * 10000, "Upper limit3");
            limitCoverageUpper = model.NewIntVar(0, jobs.Count * 10000, "Upper limit4");
        }

        /// <summary>
        /// Matrix of booleans for the model
        /// If the boolean has value true, that means employee with id of row works on the job 
        /// which id is value of the coloumas where boolean belong.
        /// </summary>
        /// <example>
        ///             0   1   2   3   4
        ///          _____________________
        ///     0   |   0   0   1   1   0
        ///     1   |   1   0   1  [1]  0
        ///     2   |  [0]  1   1   1   1
        ///     
        /// Employee with id 1 works on job with id of the 3
        /// Employee with id 2 does not work on job with id of the 0
        /// etc.
        /// 
        /// </example>
        public static ILiteral[,] matrix;
        public static IntVar[] dynamicConstraintUpper;
        public static IntVar[] dynamicConstraintLower;
        private readonly List<Employee> employees;
        private readonly List<ScheduleJob> jobs;
        public static CpModel model;        

        IEnumerable<int> setOfJobs;
        IEnumerable<int> setOfEmployees;

        public static IntVar[] uLimit;
        public static IntVar[] lLimit;
        public static IntVar limit;
        public static IntVar limitLower;
        public static IntVar limitCoverage;
        public static IntVar limitCoverageUpper;

        public void InitialModel()
        {
            foreach (int e in setOfEmployees)
            {
                foreach (int j in setOfJobs)
                {
                    matrix[e, j] = model.NewBoolVar($"On Job:{j} , works employee:{e}");                    
                }
            }
        }

        public void InitDynamic(List<RankingWrapper> rankingWrappers)
        {
            dynamicConstraintUpper = new IntVar[RankingManager.CountAllCOnditions(rankingWrappers) * 4 + 2];
            dynamicConstraintLower = new IntVar[RankingManager.CountAllCOnditions(rankingWrappers) * 4 + 2];
            
            IntVar intVar = new IntVar(model.Model, 0);
            for(int i = 0; i < dynamicConstraintLower.Count(); i++)
            {
                dynamicConstraintLower[i] = intVar;
            }
            for (int i = 0; i < dynamicConstraintUpper.Count(); i++)
            {
                dynamicConstraintUpper[i] = intVar;
            }
        }

        public void AssignAvailableEmployees(List<Employee> employees, int[,] availabilityMatrix, bool ignoreAlreadyAssignedJobs)
        {
            for (int j = 0; j < jobs.Count; j++)
            {
                if (jobs[j].UnavailableEmployeesForJob != null)
                {
                    for (int a = 0; a < jobs[j].UnavailableEmployeesForJob.Count; a++)
                    {
                        for (int i = 0; i < employees.Count; i++)
                        {
                            if (employees[i].Id == jobs[j].UnavailableEmployeesForJob[a])
                            {
                                availabilityMatrix[i, j] = -2;
                                model.Add((IntVar)(matrix[i, j]) == 0);
                                break;
                            }
                        }
                    }
                }
                if (!ignoreAlreadyAssignedJobs)
                {
                    if (jobs[j].AlreadyAssignedEmployeeOnJob != null)
                    {
                        for (int b = 0; b < jobs[j].AlreadyAssignedEmployeeOnJob.Count; b++)
                        {
                            for (int i = 0; i < employees.Count; i++)
                            {
                                if (employees[i].Id == jobs[j].AlreadyAssignedEmployeeOnJob[b])
                                {
                                    availabilityMatrix[i, j] = -1;                                    
                                    model.Add((IntVar)(matrix[i, j]) == 1);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool IsCompared(List<ComparisonRule> comparisonRules, Employee updatedEmployee, List<ScheduleCustomData> listOlistfAllCustomData)
        {
            bool isTrue = true;
            if (comparisonRules != null)
            {
                foreach (ComparisonRule comparison in comparisonRules)
                {
                    if (comparison.ComparisonMode.ToString() == "Set Value")
                    {
                        if (comparison.EmployeeField.ToString() == "Class")
                        {
                            if (updatedEmployee.Class == null)
                                continue;
                            if ((updatedEmployee.Class.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Class.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "CostCode")
                        {
                            if (updatedEmployee.CostCode == null)
                                continue;
                            if ((updatedEmployee.CostCode != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.CostCode == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Department")
                        {
                            if (updatedEmployee.Department == null)
                                continue;
                            if ((updatedEmployee.Department.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Department.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Division")
                        {
                            if (updatedEmployee.Division == null)
                                continue;
                            if ((updatedEmployee.Division.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Division.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Email")
                        {
                            if (updatedEmployee.Email == null)
                                continue;
                            if ((updatedEmployee.Email.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Email.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "EmployeeNumber")
                        {
                            if (updatedEmployee.EmployeeNumber == null)
                                continue;
                            if ((updatedEmployee.EmployeeNumber.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.EmployeeNumber.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Profile")
                        {
                            if (updatedEmployee.Profile == null)
                                continue;
                            if ((updatedEmployee.Profile.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Profile.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Firstname")
                        {
                            if (updatedEmployee.Firstname == null)
                                continue;
                            if ((updatedEmployee.Firstname.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Firstname.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Gender")
                        {
                            if (updatedEmployee.Gender == null)
                                continue;
                            if ((updatedEmployee.Gender.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Gender.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "JobTitle")
                        {
                            if (updatedEmployee.JobTitle == null)
                                continue;
                            if ((updatedEmployee.JobTitle.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.JobTitle.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Location")
                        {
                            if (updatedEmployee.Location == null)
                                continue;
                            if ((updatedEmployee.Location.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Location.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Manager")
                        {
                            if (updatedEmployee.Manager == null)
                                continue;
                            if ((updatedEmployee.Manager.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Manager.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "MaxHours1")
                        {
                            if (updatedEmployee.MaxHours1 == null)
                                continue;
                            if ((updatedEmployee.MaxHours1 != comparison.NumberValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.MaxHours1 == comparison.NumberValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "MaxHours2")
                        {
                            if (updatedEmployee.MaxHours2 == null)
                                continue;
                            if ((updatedEmployee.MaxHours2 != comparison.NumberValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.MaxHours2 == comparison.NumberValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "MaxHours3")
                        {
                            if (updatedEmployee.MaxHours3 == null)
                                continue;
                            if ((updatedEmployee.MaxHours3 != comparison.NumberValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.MaxHours3 == comparison.NumberValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "MaxHours4")
                        {
                            if (updatedEmployee.MaxHours4 == null)
                                continue;
                            if ((updatedEmployee.MaxHours4 != comparison.NumberValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.MaxHours4 == comparison.NumberValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "MaxHours5")
                        {
                            if (updatedEmployee.MaxHours5 == null)
                                continue;
                            if ((updatedEmployee.MaxHours5 != comparison.NumberValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.MaxHours5 == comparison.NumberValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "MobileNumber")
                        {
                            if (updatedEmployee.MobileNumber == null)
                                continue;
                            if ((updatedEmployee.MobileNumber.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.MobileNumber.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Notification")
                        {
                            if (updatedEmployee.Notification == null)
                                continue;
                            if ((updatedEmployee.Notification != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Notification == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Password")
                        {
                            if (updatedEmployee.Password == null)
                                continue;
                            if ((updatedEmployee.Password.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Password.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "PayRate")
                        {
                            if (updatedEmployee.PayRate == null)
                                continue;
                            if ((updatedEmployee.PayRate.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.PayRate.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Surname")
                        {
                            if (updatedEmployee.Surname == null)
                                continue;
                            if ((updatedEmployee.Surname.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Surname.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Team")
                        {
                            if (updatedEmployee.Team == null)
                                continue;
                            if ((updatedEmployee.Team.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Team.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "TelephoneNumber")
                        {
                            if (updatedEmployee.TelephoneNumber == null)
                                continue;
                            if ((updatedEmployee.TelephoneNumber.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.TelephoneNumber.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                        else if (comparison.EmployeeField.ToString() == "Username")
                        {
                            if (updatedEmployee.Username == null)
                                continue;
                            if ((updatedEmployee.Username.ToString() != comparison.TextValue && comparison.Operator.Equals("="))
                                || (updatedEmployee.Username.ToString() == comparison.TextValue && comparison.Operator.Equals("<>")))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                    }
                    else if (comparison.ComparisonMode == "Custom Data" && comparison.CustomDataID != null)
                    {
                        foreach (ScheduleCustomData scheduleCustomData in listOlistfAllCustomData)
                        {
                            if (scheduleCustomData.CustomDataID == comparison.CustomDataID)
                            {
                                //Console.WriteLine(scheduleCustomData.CustomDataID + " == " + comparison.CustomDataID);
                                if (scheduleCustomData.CustomDataLookupID != null)
                                {

                                    if (comparison.EmployeeField.ToString() == "Class")
                                    {
                                        if (updatedEmployee.Class == null)
                                            continue;
                                        if (((updatedEmployee.Class != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Class == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Address1")
                                    {
                                        if (updatedEmployee.Address1 == null)
                                            continue;
                                        if (((updatedEmployee.Address1 != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Address1 == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Address2")
                                    {
                                        if (updatedEmployee.Address2 == null)
                                            continue;
                                        if (((updatedEmployee.Address2 != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Address2 == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Address3")
                                    {
                                        if (updatedEmployee.Address3 == null)
                                            continue;
                                        if (((updatedEmployee.Address3 != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Address3 == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Address4")
                                    {
                                        if (updatedEmployee.Address4 == null)
                                            continue;
                                        if (((updatedEmployee.Address4 != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Address4 == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "CostCode")
                                    {
                                        if (updatedEmployee.CostCode == null)
                                            continue;
                                        if (((updatedEmployee.CostCode != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.CostCode == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Department")
                                    {
                                        if (updatedEmployee.Department == null)
                                            continue;
                                        if (((updatedEmployee.Department != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Department == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Division")
                                    {
                                        if (updatedEmployee.Division == null)
                                            continue;
                                        if (((updatedEmployee.Division != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Division == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Email")
                                    {
                                        if (updatedEmployee.Email == null)
                                            continue;
                                        if (((updatedEmployee.Email != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Email == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "EmployeeNumber")
                                    {
                                        if (updatedEmployee.EmployeeNumber == null)
                                            continue;
                                        if (((updatedEmployee.EmployeeNumber.ToString() != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.EmployeeNumber.ToString() == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Profile")
                                    {
                                        if (updatedEmployee.Profile == null)
                                            continue;
                                        if (((updatedEmployee.Profile.ToString() != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Profile.ToString() == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Firstname")
                                    {
                                        if (updatedEmployee.Firstname == null)
                                            continue;
                                        if (((updatedEmployee.Firstname != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Firstname == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Gender")
                                    {
                                        if (updatedEmployee.Gender == null)
                                            continue;
                                        if (((updatedEmployee.Gender != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Gender == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "JobTitle")
                                    {
                                        if (updatedEmployee.JobTitle == null)
                                            continue;
                                        if (((updatedEmployee.JobTitle != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.JobTitle == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Location")
                                    {
                                        if (updatedEmployee.Location == null)
                                            continue;
                                        if (((updatedEmployee.Location != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Location == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Manager")
                                    {
                                        if (updatedEmployee.Manager == null)
                                            continue;
                                        if (((updatedEmployee.Manager.ToString() != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Manager.ToString() == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "MaxHours1")
                                    {
                                        if (updatedEmployee.MaxHours1 == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours1 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours1 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "MaxHours2")
                                    {
                                        if (updatedEmployee.MaxHours2 == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours2 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours2 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "MaxHours3")
                                    {
                                        if (updatedEmployee.MaxHours3 == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours3 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours3 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "MaxHours4")
                                    {
                                        if (updatedEmployee.MaxHours4 == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours4 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours4 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "MaxHours5")
                                    {
                                        if (updatedEmployee.MaxHours5 == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours5 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours5 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "MobileNumber")
                                    {
                                        if (updatedEmployee.MobileNumber == null)
                                            continue;
                                        if (((updatedEmployee.MobileNumber != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MobileNumber == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Notification")
                                    {
                                        if (updatedEmployee.Notification == null)
                                            continue;
                                        if (((updatedEmployee.Notification != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Notification == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Password")
                                    {
                                        if (updatedEmployee.Password == null)
                                            continue;
                                        if (((updatedEmployee.Password != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Password == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "PayRate")
                                    {
                                        if (updatedEmployee.PayRate == null)
                                            continue;
                                        if (((updatedEmployee.PayRate != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.PayRate == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Surname")
                                    {
                                        if (updatedEmployee.Surname == null)
                                            continue;
                                        if (((updatedEmployee.Surname != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Surname == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Team")
                                    {
                                        if (updatedEmployee.Team == null)
                                            continue;
                                        if (((updatedEmployee.Team != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Team == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "TelephoneNumber")
                                    {
                                        if (updatedEmployee.TelephoneNumber == null)
                                            continue;
                                        if (((updatedEmployee.TelephoneNumber != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.TelephoneNumber == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField.ToString() == "Username")
                                    {
                                        if (updatedEmployee.Username == null)
                                            continue;
                                        if (((updatedEmployee.Username != scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Username == scheduleCustomData.CustomDataLookupContent) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                }
                                else if (scheduleCustomData.CustomDataLookupID == null)
                                {
                                    if (comparison.EmployeeField == "Address1")
                                    {
                                        if (updatedEmployee.Address1 == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Address1 != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Address1 == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Address2")
                                    {
                                        if (updatedEmployee.Address2 == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Address2 != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Address2 == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Address3")
                                    {
                                        if (updatedEmployee.Address3 == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Address3 != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Address3 == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Address4")
                                    {
                                        if (updatedEmployee.Address4 == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Address4 != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Address4 == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Class")
                                    {
                                        if (updatedEmployee.Class == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Class != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Class == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "CostCode")
                                    {
                                        if (updatedEmployee.CostCode == null || scheduleCustomData.NumberValue == null)
                                            continue;
                                        if (((updatedEmployee.CostCode != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.CostCode == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Department")
                                    {
                                        if (updatedEmployee.Department == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Department != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Department == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Division")
                                    {
                                        if (updatedEmployee.Division == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Division != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Division == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Email")
                                    {
                                        if (updatedEmployee.Email == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Email != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Email == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "EmployeeNumber")
                                    {
                                        if (updatedEmployee.EmployeeNumber == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.EmployeeNumber.ToString() != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.EmployeeNumber.ToString() == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Profile")
                                    {
                                        if (updatedEmployee.Profile == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Profile.ToString() != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Profile.ToString() == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Firstname")
                                    {
                                        if (updatedEmployee.Firstname == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Firstname != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Firstname == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Gender")
                                    {
                                        if (updatedEmployee.Gender == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Gender != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Gender == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "JobTitle")
                                    {
                                        if (updatedEmployee.JobTitle == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.JobTitle != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.JobTitle == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Location")
                                    {
                                        if (updatedEmployee.Location == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Location != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Location == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Manager")
                                    {
                                        if (updatedEmployee.Manager == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Manager.ToString() != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Manager.ToString() == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "MaxHours1")
                                    {
                                        if (updatedEmployee.MaxHours1 == null || scheduleCustomData.NumberValue == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours1 != scheduleCustomData.NumberValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours1 == scheduleCustomData.NumberValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "MaxHours2")
                                    {
                                        if (updatedEmployee.MaxHours2 == null || scheduleCustomData.NumberValue == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours2 != scheduleCustomData.NumberValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours2 == scheduleCustomData.NumberValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "MaxHours3")
                                    {
                                        if (updatedEmployee.MaxHours3 == null || scheduleCustomData.NumberValue == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours3 != scheduleCustomData.NumberValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours3 == scheduleCustomData.NumberValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "MaxHours4")
                                    {
                                        if (updatedEmployee.MaxHours4 == null || scheduleCustomData.NumberValue == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours4 != scheduleCustomData.NumberValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours4 == scheduleCustomData.NumberValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "MaxHours5")
                                    {
                                        if (updatedEmployee.MaxHours5 == null || scheduleCustomData.NumberValue == null)
                                            continue;
                                        if (((updatedEmployee.MaxHours5 != scheduleCustomData.NumberValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MaxHours5 == scheduleCustomData.NumberValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "MobileNumber")
                                    {
                                        if (updatedEmployee.MobileNumber == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.MobileNumber != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.MobileNumber == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Notification")
                                    {
                                        if (updatedEmployee.Notification == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Notification != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Notification == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Password")
                                    {
                                        if (updatedEmployee.Password == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Password != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Password == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "PayRate")
                                    {
                                        if (updatedEmployee.PayRate == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.PayRate != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.PayRate == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Surname")
                                    {
                                        if (updatedEmployee.Surname == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Surname != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Surname == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Team")
                                    {
                                        if (updatedEmployee.Team == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Team != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Team == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "TelephoneNumber")
                                    {
                                        if (updatedEmployee.TelephoneNumber == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.TelephoneNumber != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.TelephoneNumber == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                    else if (comparison.EmployeeField == "Username")
                                    {
                                        if (updatedEmployee.Username == null || scheduleCustomData.TextValue == null)
                                            continue;
                                        if (((updatedEmployee.Username != scheduleCustomData.TextValue) && comparison.Operator.Equals("="))
                                            || ((updatedEmployee.Username == scheduleCustomData.TextValue) && comparison.Operator.Equals("<>")))
                                        {
                                            isTrue = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return isTrue;
        }

        /// <summary>
        /// Constraint for overlapping
        /// It check all pairs from funcion FindOverLappingNIntervals
        /// Employee works on only one job is same as the some of corresponding elements is less then 1
        /// [03.1.1] Not already assigned
        /// </summary>
        /// <param name="finalPairs">Pairs of id's which overlap</param>
        /// <param name="maxNumOfJobsAtSameTime">Number represent on how many jobs, employee can work at same time</param>
        public void NotAlreadyAssigned(List<List<long>> finalPairs, int maxNumOfJobsAtSameTime, List<Employee> employees, List<ScheduleJob> scheduleJobs)
        {
            if (finalPairs != null)
            {
                for (int oneArray = 0; oneArray < finalPairs.Count; oneArray++)
                {
                    for (int e = 0; e < employees.Count; e++)
                    {
                        ILiteral[] tmp = new ILiteral[finalPairs[oneArray].Count];
                        for (int oneInt = 0; oneInt < finalPairs[oneArray].Count; oneInt++)
                        {
                            for (int sj = 0; sj < scheduleJobs.Count; sj++)
                            {
                                if (jobs[sj].JobID == finalPairs[oneArray][oneInt])
                                {
                                    tmp[oneInt] = matrix[e, sj];
                                    break; // 22-02-2021
                                }
                            }
                        }
                        model.Add(LinearExpr.Sum(tmp) <= maxNumOfJobsAtSameTime);
                    }
                }
            }
        }

        public static int getPositionOfTheJobFromJobID(long jobID, List<ScheduleJob> scheduleJobs)
        {
            for (int sj = 0; sj < scheduleJobs.Count; sj++)
            {
                if (scheduleJobs[sj].JobID == jobID)
                {
                    return sj;
                }
            }
            return -1;
        }

        /// <summary>
        /// Set the maximum number of the workers per job
        /// </summary>
        /// <param name="setRestictedNumberOfEmployees">If it's restircted then value is in the [noOfEmployeesRequired, onCallSlots], else [0, onCallSlots]</param>
        public void ApplyRequired(bool setRestictedNumberOfEmployees)
        {
            foreach (int j in setOfJobs)
            {
                ILiteral[] sum = new ILiteral[employees.Count * jobs.Count];
                ILiteral[] tmp = new ILiteral[employees.Count];
                foreach (int e in setOfEmployees)
                {
                    tmp[e] = matrix[e, j];
                    sum[(j * employees.Count) + e] = matrix[e, j];
                }
                if (setRestictedNumberOfEmployees)
                {
                    model.Add(LinearExpr.Sum(tmp) <= (int)jobs[j].NoOfEmployeesRequired);
                }
                else
                {
                    model.Add(LinearExpr.Sum(tmp) <= (int)jobs[j].NoOfEmployeesRequired);
                }
            }
            /*
            foreach (int e in setOfEmployees)
            {
                IntVar[] sum = new IntVar[employees.Count * jobs.Count];
                IntVar[] tmp = new IntVar[jobs.Count];
                foreach (int j in setOfJobs) {
                    tmp[j] = matrix[e, j];
                }
                //model.Add(jobs[j].NoOfEmployeesRequired <= LinearExpr.Sum(tmp) <= jobs[j].NoOfEmployeesRequired + jobs[j]. onCallSlots);
                model.Add(LinearExpr.Sum(tmp) <= 4);
            }*/

            //foreach (int e in setOfEmployees)
            //{
            //    ILiteral[] sum = new ILiteral[employees.Count * jobs.Count];
            //    ILiteral[] tmp = new ILiteral[jobs.Count];
            //    foreach (int j in setOfJobs)
            //    {
            //        tmp[j] = matrix[e, j];
            //    }
                //model.Add(LinearExpr.Sum(tmp) <= 4);
                //model.Add(jobs[j].NoOfEmployeesRequired <= LinearExpr.Sum(tmp) <= jobs[j].NoOfEmployeesRequired + jobs[j]. onCallSlots);
                //model.Add(LinearExpr.Sum(tmp) != 1);
                //model.Add(LinearExpr.Sum(tmp) != 2);
                //model.Add(LinearExpr.Sum(tmp) != 3);
                //model.Add(LinearExpr.Sum(tmp) != 3);
                //model.Add(LinearExpr.Sum(tmp) != 3);
                //model.Add(LinearExpr.Sum(tmp) != 4);
                //model.Add(LinearExpr.Sum(tmp) != 5);
                //model.Add(LinearExpr.Sum(tmp) != 6);
                //model.Add(LinearExpr.Sum(tmp) != 7);
                //model.Add(LinearExpr.Sum(tmp) != 8);
                //model.Add(LinearExpr.Sum(tmp) >= 5);
            //}

        }

        /// <summary>
        /// Setting already assigned employees to the existing jobs
        /// </summary>
        /// <param name="ignoreAleardyAssignedEmployees">If it's ignored then all employees will be reassigned</param>
        public void Assigned(List<EmployeeAssigment> jobsAssignments, bool ignoreAleardyAssignedEmployees)
        {
            if (!ignoreAleardyAssignedEmployees)
            {
                foreach (EmployeeAssigment jobsAssignment in jobsAssignments)
                {
                    for (int jobID = 0; jobID < jobs.Count; jobID++)
                    {
                        if (jobs[jobID].JobID == jobsAssignment.JobID)
                        {
                            ILiteral[] tmp = new ILiteral[1];
                            tmp[0] = matrix[jobsAssignment.EmployeeID, jobID];
                            model.Add(LinearExpr.Sum(tmp) == 1);
                            break;
                        }
                    }
                }
            }
        }

        public void AssignedOnTheOtherSchedule(List<EmployeeAssigment> jobsAssignments, List<JobFromOutside> jobsForSolvers)
        {
            for (int jobID = 0; jobID < jobs.Count; jobID++)
            {
                if (jobsForSolvers != null)
                {
                    for (int jobOnOtherScheduleID = 0; jobOnOtherScheduleID < jobsForSolvers.Count; jobOnOtherScheduleID++)
                    {
                        foreach (EmployeeAssigment jobsAssignment in jobsAssignments)
                        {
                            if (jobsAssignment.JobID == jobsForSolvers[jobOnOtherScheduleID].JobID)
                            {
                                if ((DateTime.Compare(jobs[jobID].JobStartDate, jobsAssignment.JobStartDate) >= 0
                                  && DateTime.Compare(jobs[jobID].JobStartDate, jobsAssignment.JobEndDate) <= 0)
                                  ||
                                  (DateTime.Compare(jobs[jobID].JobEndDate, jobsAssignment.JobStartDate) >= 0
                                  && DateTime.Compare(jobs[jobID].JobEndDate, jobsAssignment.JobEndDate) <= 0)
                                  ||
                                  (DateTime.Compare(jobs[jobID].JobStartDate, jobsAssignment.JobStartDate) <= 0
                                  && (DateTime.Compare(jobs[jobID].JobEndDate, jobsAssignment.JobStartDate) >= 0)))
                                {
                                    for (int e = 0; e < employees.Count; e++)
                                    {
                                        if (employees[e].Id == jobsAssignment.EmployeeID)
                                        {
                                            ILiteral[] tmp = new ILiteral[1];
                                            tmp[0] = matrix[e, jobID];
                                            model.Add(LinearExpr.Sum(tmp) == 0);
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
        /// OPTIMAZE    
        /// AUTO-SOLVE  
        /// </summary>
        /// <remarks>
        /// For the fitting better solutions there will be
        /// some other type of the OPTIMAZE
        /// [Combionations of OPTIMAZE and AUTO-SOLVE]
        /// </remarks>
        /// <param name="maximaze">Fill as many as employees to work as it has free space</param>
        public void Optimaze(bool maximaze, List<ScheduleRanking> scheduleRankings, List<RankingRule> rankingRules, List<RankingType> rankingTypes, List<RankingWrapper> rankingWrappers, int maxEmployeesWorking, bool implement, bool largeDateSet, int turnForRanking, long k, int maxSortOrder)
        {
            /// <summary>
            /// Summing all elements for the optimaze option
            /// </summary>

            LinearExpr[] linearExpr = new LinearExpr[jobs.Count * employees.Count];
            LinearExpr[] linearExprRang = new LinearExpr[jobs.Count * employees.Count];
            long[] CofficientsOfRang = new long[jobs.Count * employees.Count];


            if (largeDateSet)
            {
                ApplyOneSolverApproach(rankingWrappers, linearExprRang, CofficientsOfRang, implement, k, maxSortOrder);
            }
            else
            {
                ApplyIterativSolverApproach(rankingWrappers, implement, turnForRanking, linearExprRang, CofficientsOfRang, maxSortOrder);
            }

            //List<ILiteral> StrategyList = new List<ILiteral>();

            for (int j = 0; j < jobs.Count; j++)
            {
                for (int e = 0; e < employees.Count; e++)
                {
                    linearExpr[(j * employees.Count) + e] = (LinearExpr)(matrix[e, j]);
                    //StrategyList.Add((ILiteral)(matrix[e, j]));
                }
            }

            if (!largeDateSet)
            {
                //? Up

                if (maxEmployeesWorking != -1)
                {
                    model.Add(LinearExpr.Sum(linearExpr) == maxEmployeesWorking);
                }
            }

            for (int j = 0; j < jobs.Count; j++)
            {
                for (int e = 0; e < employees.Count; e++)
                {
                    //StrategyList.Add(matrix[e, j]);

                    if (rankingWrappers.Count > 0 && rankingWrappers[0].isDynamic == true)
                    {
                        if (jobs[j].AlreadyAssignedEmployeeOnJob.Count() > 0)
                        {
                            if (jobs[j].AlreadyAssignedEmployeeOnJob.Any(x => x == employees[e].Id))
                            {
                                linearExprRang[(j * employees.Count) + e] = LinearExpr.Term(matrix[e, j], CofficientsOfRang[(j * employees.Count) + e] + 1001 /*+ jobs.Count*/ + j);
                            }
                            else
                            {
                                linearExprRang[(j * employees.Count) + e] = LinearExpr.Term(matrix[e, j], CofficientsOfRang[(j * employees.Count) + e] + 1000 - j);
                            }
                        }
                        else
                            linearExprRang[(j * employees.Count) + e] = LinearExpr.Term(matrix[e, j], CofficientsOfRang[(j * employees.Count) + e] + 1000 - j);
                    }
                    else
                    {
                        linearExprRang[(j * employees.Count) + e] = LinearExpr.Term(matrix[e, j], (CofficientsOfRang[(j * employees.Count) + e]) + 1);
                    }
                }
            }

            int scaleLower, scaleUpper, scaleRanking;
            if (largeDateSet)
            {
                if (rankingWrappers.Count > 0)
                {
                    if ((rankingWrappers[0].isDynamic == true))
                    {
                        scaleLower = 100; //500;
                        scaleUpper = 100; //500;
                        scaleRanking = 1;
                    }
                    else
                    {
                        scaleLower = 5; //10
                        scaleUpper = 5; //10
                        scaleRanking = 10; //5
                    }
                }
                else
                {
                    scaleLower = 0; //10
                    scaleUpper = 0; //10
                    scaleRanking = 1; //5
                }
            }
            else
            {
                scaleLower = 1;
                scaleUpper = 1;
                scaleRanking = 5;
            }

            // CUSTOM

            //scaleLower = 10; //10
            //scaleUpper = 2; //10
            //scaleRanking = 1000; //5
            //model.Add(LinearExpr.Sum(linearExpr) >= 200);

            //model.AddDecisionStrategy(StrategyList, DecisionStrategyProto.Types.VariableSelectionStrategy.ChooseHighestMax, DecisionStrategyProto.Types.DomainReductionStrategy.SelectMinValue);

            //model.AddDecisionStrategy(linearExprRang, DecisionStrategyProto.Types.VariableSelectionStrategy.ChooseHighestMax, DecisionStrategyProto.Types.DomainReductionStrategy.SelectMinValue);

            // Vratiti
            if ((rankingWrappers.Count > 0 && rankingWrappers[0].isDynamic == true) || ((rankingWrappers.Count > 1 && rankingWrappers[1].isDynamic == true)))
            {
                if (dynamicConstraintLower[0] != null)
                {
                    foreach (var item in dynamicConstraintLower)
                    {
                        if (item != null)
                        {
                            model.AddDecisionStrategy(new List<IntVar> { item }, DecisionStrategyProto.Types.VariableSelectionStrategy.ChooseHighestMax, DecisionStrategyProto.Types.DomainReductionStrategy.SelectMinValue);
                        }
                    }
                }
            }

            if ((rankingWrappers.Count == 0 || ((rankingWrappers.Count == 1 && rankingWrappers[0].rankingName == "Coverage"))))
            {
                //1000 * LinearExpr.Sum(linearExpr) + 
                model.Maximize(LinearExpr.Sum(dynamicConstraintLower) - LinearExpr.Sum(dynamicConstraintUpper) + scaleRanking * LinearExpr.Sum(linearExprRang));
               //model.Maximize(LinearExpr.Sum(linearExprRang));
            }
            else
            {
                if (largeDateSet)
                {
                    if ((rankingWrappers[0].isDynamic == true) || (rankingWrappers.Count > 2 && rankingWrappers[1].isDynamic == true))
                    {
                        //100 * LinearExpr.Sum(linearExpr) + 
                        model.Maximize(scaleLower * LinearExpr.Sum(dynamicConstraintLower) - scaleUpper * LinearExpr.Sum(dynamicConstraintUpper) + scaleRanking * LinearExpr.Sum(linearExprRang));//
                    }
                    else
                    {
                        //LinearExpr.Sum(linearExpr)
                        model.Maximize(scaleLower * LinearExpr.Sum(dynamicConstraintLower) - scaleUpper * LinearExpr.Sum(dynamicConstraintUpper) + scaleRanking * LinearExpr.Sum(linearExprRang));
                    }
                }
                else
                {
                    if (turnForRanking == 0)
                    {
                        // 22/09/2020
                        // 1000 * LinearExpr.Sum(linearExpr) +  
                        if (rankingWrappers[0].isDynamic)
                        {
                            model.Maximize(100000 * LinearExpr.Sum(linearExpr) + scaleLower * LinearExpr.Sum(dynamicConstraintLower) - scaleUpper * LinearExpr.Sum(dynamicConstraintUpper) + scaleRanking * LinearExpr.Sum(linearExprRang));
                        }
                        else
                        {
                            model.Maximize(scaleLower * LinearExpr.Sum(dynamicConstraintLower) - scaleUpper * LinearExpr.Sum(dynamicConstraintUpper) + scaleRanking * LinearExpr.Sum(linearExprRang));
                        }
                    }
                    else
                    {
                        if (rankingWrappers[turnForRanking].isDynamic == true)
                        {
                            model.Maximize(scaleLower * LinearExpr.Sum(dynamicConstraintLower) - scaleUpper * LinearExpr.Sum(dynamicConstraintUpper) + LinearExpr.Sum(linearExprRang));
                        }
                        else
                        {
                            model.Maximize(LinearExpr.Sum(linearExprRang));
                        }
                    }
                }
            }
        }

        private void ApplyIterativSolverApproach(List<RankingWrapper> rankingWrappers, bool implement, int turnForRanking, LinearExpr[] linearExprRang, long[] cofficientsOfRang, int maxSortOrder)
        {
            int startingPositionForClausa = 0;
            bool includeSortOrder = false;
            for (int solveRanking = 0; solveRanking < turnForRanking + 1; solveRanking++)
            {
                if (!implement)
                {
                    if (solveRanking != turnForRanking)
                    {
                        DynamicConstraintImplement(model, rankingWrappers, employees.Count, solveRanking, startingPositionForClausa, linearExprRang, false, includeSortOrder, maxSortOrder);

                        if (rankingWrappers[solveRanking].isDynamic == true)
                        {
                            startingPositionForClausa += rankingWrappers[solveRanking].conditions.Count();
                        }
                    }
                    else
                    {
                        DynamicConstraint(model, rankingWrappers, employees.Count, solveRanking, startingPositionForClausa, linearExprRang, cofficientsOfRang, includeSortOrder, maxSortOrder);
                    }
                }
                else
                {
                    DynamicConstraintImplement(model, rankingWrappers, employees.Count, solveRanking, startingPositionForClausa, linearExprRang, false, includeSortOrder, maxSortOrder);

                    if (rankingWrappers[solveRanking].isDynamic == true)
                    {
                        startingPositionForClausa += rankingWrappers[solveRanking].conditions.Count();
                    }
                }
            }
        }

        private void ApplyOneSolverApproach(List<RankingWrapper> rankingWrappers, LinearExpr[] linearExprRang, long[] cofficientsOfRang, bool implement, long k, int maxSortOrder)
        {
            /*
            int startingPositionForClausa = 0;
            for (int solveRanking = 0; solveRanking < rankingWrappers.Count; solveRanking++)
            {
                if (!implement)
                {
                    DynamicConstraint(model, rankingWrappers, employees.Count, solveRanking, startingPositionForClausa, linearExprRang, cofficientsOfRang);
                    if (rankingWrappers[solveRanking].isDynamic == true)
                    {
                        startingPositionForClausa++;
                    }
                }
            }*/

            int startingPositionForClausa = 0;
            bool includeSortOrder = true;
            for (int solveRanking = 0; solveRanking < rankingWrappers.Count; solveRanking++)
            {
                if (implement)
                {
                    if (solveRanking != k)
                    {
                        DynamicConstraintImplement(model, rankingWrappers, employees.Count, solveRanking, startingPositionForClausa, linearExprRang, false, includeSortOrder, maxSortOrder);

                        if (rankingWrappers[solveRanking].isDynamic == true)
                        {
                            startingPositionForClausa += rankingWrappers[solveRanking].conditions.Count();
                        }
                    }
                    else
                    {
                        DynamicConstraintImplement(model, rankingWrappers, employees.Count, solveRanking, startingPositionForClausa, linearExprRang, true, includeSortOrder, maxSortOrder);
                        DynamicConstraint(model, rankingWrappers, employees.Count, solveRanking, startingPositionForClausa, linearExprRang, cofficientsOfRang, includeSortOrder, maxSortOrder);

                        if (rankingWrappers[solveRanking].isDynamic == true)
                        {
                            startingPositionForClausa += rankingWrappers[solveRanking].conditions.Count();
                        }
                    }
                }
                else
                {
                    DynamicConstraint(model, rankingWrappers, employees.Count, solveRanking, startingPositionForClausa, linearExprRang, cofficientsOfRang, includeSortOrder, maxSortOrder);
                    /*
                    if (rankingWrappers[solveRanking].isDynamic == true)
                    {
                        startingPositionForClausa++;
                    }*/
                    if (rankingWrappers[solveRanking].isDynamic == true)
                    {
                        startingPositionForClausa += rankingWrappers[solveRanking].conditions.Count();
                    }
                }
            }
        }

        /// <summary>
        /// Adding dynamic constraint that is used for dynamic rankings
        /// </summary>
        /// <param name="model"></param>
        /// <param name="rankingWrappers"></param>
        /// <param name="emplCount"></param>
        /// <param name="turnForRanking"></param>
        /// <param name="startingPositionForClausa"></param>
        /// <param name="linearExprRang"></param>
        /// <param name="cofficientsOfRang"></param>
        /// <param name="includeSortOrder"></param>
        private void DynamicConstraint(CpModel model, List<RankingWrapper> rankingWrappers, int emplCount, int turnForRanking, int startingPositionForClausa, LinearExpr[] linearExprRang, long[] cofficientsOfRang, bool includeSortOrder, int maxSortOrder)
        {
            int scaleSortOrder = includeSortOrder ? (int)Math.Pow(10, maxSortOrder - rankingWrappers[turnForRanking].sortOrder) : 1;

            if (rankingWrappers[turnForRanking].isDynamic == true)
            {
                // Ranking is dynamic
                for (int con = 0; con < rankingWrappers[turnForRanking].conditions.Count; con++)
                {
                    if (rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthDynamicConstraint == null)
                    {
                        // There is no second lvl implemented
                        dynamicConstraintUpper[startingPositionForClausa + con] = model.NewIntVar(0, jobs.Count * 1000000, "Upper limit claus: " + turnForRanking);
                        dynamicConstraintLower[startingPositionForClausa + con] = model.NewIntVar(0, jobs.Count * 1000000, "Lower limit claus: " + turnForRanking);

                        if (rankingWrappers[turnForRanking].rankingName == "Week Freq")
                        {
                            for (int cla = 0; cla < rankingWrappers[turnForRanking].conditions[con].clasuses.Count; cla++)
                            {
                                ILiteral[] limitEmployeesWork = new ILiteral[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek.Count];

                                for (int dateWeek = 0; dateWeek < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek.Count; dateWeek++)
                                {
                                    List<ILiteral> jobsOnTheSameDayWeek = new List<ILiteral>();
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek].Count; assigment++)
                                    {
                                        jobsOnTheSameDayWeek.Add(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek][assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek][assigment].GetValue(0, 1)]);
                                    }

                                    limitEmployeesWork[dateWeek] = model.NewBoolVar("If employee " + cla + " works on " + dateWeek);
                                    model.Add(LinearExpr.Sum(jobsOnTheSameDayWeek) == 0).OnlyEnforceIf(limitEmployeesWork[dateWeek].Not());
                                    model.Add(LinearExpr.Sum(jobsOnTheSameDayWeek) >= 1).OnlyEnforceIf(limitEmployeesWork[dateWeek]);
                                }

                                model.Add(dynamicConstraintUpper[startingPositionForClausa + con] >= LinearExpr.Term(LinearExpr.Sum(limitEmployeesWork), 100 * scaleSortOrder) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100 * scaleSortOrder));
                                model.Add(dynamicConstraintLower[startingPositionForClausa + con] <= LinearExpr.Term(LinearExpr.Sum(limitEmployeesWork), 100 * scaleSortOrder) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100 * scaleSortOrder));
                            }
                        }
                        else
                        {
                            for (int cla = 0; cla < rankingWrappers[turnForRanking].conditions[con].clasuses.Count; cla++)
                            {
                                LinearExpr[] limitEmployeesWork = new LinearExpr[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count];

                                if (rankingWrappers[turnForRanking].rankingName == "Hours")
                                {
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                    {
                                        limitEmployeesWork[assigment] = LinearExpr.Term(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 1)], rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusaHours[assigment] * scaleSortOrder);
                                        //limitEmployeesWork[assigment] = LinearExpr.Term(limitEmployeesWork[assigment], scaleSortOrder);
                                    }
                                }

                                else if (rankingWrappers[turnForRanking].rankingName == "Job Freq")
                                {
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                    {
                                        limitEmployeesWork[assigment] = LinearExpr.Term(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 1)], 100);
                                        limitEmployeesWork[assigment] = LinearExpr.Term(limitEmployeesWork[assigment], scaleSortOrder);
                                    }

                                    model.Add(dynamicConstraintUpper[startingPositionForClausa + con] >= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalJobTypes[rankingWrappers[turnForRanking].jobTypeID] * 100)));
                                    model.Add(dynamicConstraintLower[startingPositionForClausa + con] <= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalJobTypes[rankingWrappers[turnForRanking].jobTypeID] * 100)));

                                    continue;
                                }

                                else if (rankingWrappers[turnForRanking].rankingName == "Coverage")
                                {
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                    {
                                        limitEmployeesWork[assigment] = LinearExpr.Term(matrix[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetLength(0), rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetLength(1)], 100);
                                        limitEmployeesWork[assigment] = LinearExpr.Term(limitEmployeesWork[assigment], scaleSortOrder);
                                    }

                                    goto onlyLowerImpement;
                                    //Console.WriteLine(DateTime.Parse(jobs[ranking.conditions[con].clasuses[cla].calsusa[assigment].GetLength(1)].Hours).Hour);
                                    //limitEmployeesWork[assigment] = LinearExpr.Term(matrix[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetLength(0), rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetLength(1)], jobs[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetLength(1)].HoursT.Hours * 100);
                                }

                                //for (int i = 0; i < 4; i++)

                                //Console.WriteLine("startingPositionForClausa");
                                //Console.WriteLine(startingPositionForClausa + con);
                                model.Add(dynamicConstraintUpper[startingPositionForClausa + con] >= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * scaleSortOrder)));

                            onlyLowerImpement:
                                model.Add(dynamicConstraintLower[startingPositionForClausa + con] <= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * scaleSortOrder)));

                            }

                        }
                    }
                    else
                    {
                        // There is second lvl implemented
                        dynamicConstraintUpper[startingPositionForClausa + con] = model.NewIntVar((long)(rankingWrappers[turnForRanking].conditions[con].lowerAmoutImplementation), (long)(rankingWrappers[turnForRanking].conditions[con].upperAmoutImplementation), "Upper limit claus: " + turnForRanking);
                        dynamicConstraintLower[startingPositionForClausa + con] = model.NewIntVar((long)(rankingWrappers[turnForRanking].conditions[con].lowerAmoutImplementation), (long)(rankingWrappers[turnForRanking].conditions[con].upperAmoutImplementation), "Lower limit claus: " + turnForRanking);

                        if (rankingWrappers[turnForRanking].rankingName == "Week Freq")
                        {
                            for (int cla = 0; cla < rankingWrappers[turnForRanking].conditions[con].clasuses.Count; cla++)
                            {
                                ILiteral[] limitEmployeesWork = new ILiteral[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek.Count];

                                for (int dateWeek = 0; dateWeek < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek.Count; dateWeek++)
                                {
                                    List<ILiteral> jobsOnTheSameDayWeek = new List<ILiteral>();
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek].Count; assigment++)
                                    {
                                        jobsOnTheSameDayWeek.Add(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek][assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek][assigment].GetValue(0, 1)]);
                                    }

                                    limitEmployeesWork[dateWeek] = model.NewBoolVar("If employee " + cla + " works on " + dateWeek);
                                    model.Add(LinearExpr.Sum(jobsOnTheSameDayWeek) == 0).OnlyEnforceIf(limitEmployeesWork[dateWeek].Not());
                                    model.Add(LinearExpr.Sum(jobsOnTheSameDayWeek) >= 1).OnlyEnforceIf(limitEmployeesWork[dateWeek]);
                                }

                                model.Add(LinearExpr.Sum(limitEmployeesWork) >= Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmoutImplementation) - (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100));
                                model.Add(LinearExpr.Sum(limitEmployeesWork) <= Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].upperAmoutImplementation) - (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100));
                            }
                        }
                        else
                        {
                            for (int cla = 0; cla < rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthImplement.Count; cla++)
                            {
                                LinearExpr[] limitEmployeesWorkImplement = new LinearExpr[rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthImplement[cla].calsusa.Count];

                                if (rankingWrappers[turnForRanking].rankingName == "Hours")
                                {
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthImplement[cla].calsusa.Count; assigment++)
                                    {
                                        limitEmployeesWorkImplement[assigment] = LinearExpr.Term(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthImplement[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthImplement[cla].calsusa[assigment].GetValue(0, 1)], rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthImplement[cla].calsusaHours[assigment] * scaleSortOrder);
                                    }
                                }

                                else if (rankingWrappers[turnForRanking].rankingName == "Job Freq")
                                {
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                    {
                                        limitEmployeesWorkImplement[assigment] = LinearExpr.Term(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 1)], 100);
                                    }


                                    model.Add(LinearExpr.Sum(limitEmployeesWorkImplement) >= Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmoutImplementation - (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalJobTypes[rankingWrappers[turnForRanking].jobTypeID] * 100)));
                                    model.Add(LinearExpr.Sum(limitEmployeesWorkImplement) <= Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].upperAmoutImplementation - (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalJobTypes[rankingWrappers[turnForRanking].jobTypeID] * 100)));

                                    continue;
                                }

                                else if (rankingWrappers[turnForRanking].rankingName == "Coverage")
                                {
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                    {
                                        //limitEmployeesWork[assigment] = LinearExpr.Term(matrix[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetLength(0), rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetLength(1)], 100);
                                    }

                                    goto onlyLowerImpement2i;
                                }

                                //for (int i = 0; i < 4; i++)

                                //model.Add(dynamicConstraintUpper[startingPositionForClausa + con] >= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100)));
                                model.Add(LinearExpr.Sum(limitEmployeesWorkImplement) >= Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmoutImplementation - (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100)));

                            onlyLowerImpement2i:
                                //model.Add(dynamicConstraintLower[startingPositionForClausa + con] <= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100)));

                                model.Add(LinearExpr.Sum(limitEmployeesWorkImplement) <= Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].upperAmoutImplementation - (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100)));

                            }
                        }

                        if (rankingWrappers[turnForRanking].rankingName == "Week Freq")
                        {
                            for (int cla = 0; cla < rankingWrappers[turnForRanking].conditions[con].clasuses.Count; cla++)
                            {
                                ILiteral[] limitEmployeesWork = new ILiteral[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek.Count];

                                for (int dateWeek = 0; dateWeek < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek.Count; dateWeek++)
                                {
                                    List<ILiteral> jobsOnTheSameDayWeek = new List<ILiteral>();
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek].Count; assigment++)
                                    {
                                        jobsOnTheSameDayWeek.Add(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek][assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek][assigment].GetValue(0, 1)]);
                                    }

                                    limitEmployeesWork[dateWeek] = model.NewBoolVar("If employee " + cla + " works on " + dateWeek);
                                    model.Add(LinearExpr.Sum(jobsOnTheSameDayWeek) == 0).OnlyEnforceIf(limitEmployeesWork[dateWeek].Not());
                                    model.Add(LinearExpr.Sum(jobsOnTheSameDayWeek) >= 1).OnlyEnforceIf(limitEmployeesWork[dateWeek]);
                                }

                                model.Add(dynamicConstraintUpper[startingPositionForClausa + con] >= LinearExpr.Term(LinearExpr.Sum(limitEmployeesWork), 100) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100));
                                model.Add(dynamicConstraintLower[startingPositionForClausa + con] <= LinearExpr.Term(LinearExpr.Sum(limitEmployeesWork), 100) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100));

                            }
                        }
                        else
                        {
                            for (int cla = 0; cla < rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthDynamicConstraint.Count; cla++)
                            {
                                LinearExpr[] limitEmployeesWork = new LinearExpr[rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthDynamicConstraint[cla].calsusa.Count];

                                if (rankingWrappers[turnForRanking].rankingName == "Hours")
                                {
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthDynamicConstraint[cla].calsusa.Count; assigment++)
                                    {
                                        limitEmployeesWork[assigment] = LinearExpr.Term(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthDynamicConstraint[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthDynamicConstraint[cla].calsusa[assigment].GetValue(0, 1)], rankingWrappers[turnForRanking].conditions[con].clasusesSecondDepthDynamicConstraint[cla].calsusaHours[assigment] * scaleSortOrder);
                                    }

                                }
                                else if (rankingWrappers[turnForRanking].rankingName == "Job Freq")
                                {
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                    {
                                        limitEmployeesWork[assigment] = LinearExpr.Term(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 1)], 100);
                                    }

                                    model.Add(dynamicConstraintUpper[startingPositionForClausa + con] >= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalJobTypes[rankingWrappers[turnForRanking].jobTypeID] * 100)));
                                    model.Add(dynamicConstraintLower[startingPositionForClausa + con] <= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalJobTypes[rankingWrappers[turnForRanking].jobTypeID] * 100)));
                                    continue;
                                }

                                else if (rankingWrappers[turnForRanking].rankingName == "Coverage")
                                {
                                    for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                    {
                                        limitEmployeesWork[assigment] = LinearExpr.Term(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 1)], 100);
                                    }

                                    goto onlyLowerImpement2;
                                }

                                model.Add(dynamicConstraintUpper[startingPositionForClausa + con] >= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100)));

                            onlyLowerImpement2:
                                model.Add(dynamicConstraintLower[startingPositionForClausa + con] <= (LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100)));

                            }
                        }
                    }
                }
            }
            else
            {
                // Ranking is static
                if (rankingWrappers[turnForRanking].rankingName == "Default")
                {
                    for (int e = 0; e < employees.Count; e++)
                    {
                        for (int j = 0; j < jobs.Count; j++)
                        {
                            cofficientsOfRang[(j * employees.Count) + e] += TansformDef(rankingWrappers[turnForRanking].rang[e, j]);
                        }
                    }
                }
                else if (rankingWrappers[turnForRanking].rankingName == "Employee Field")
                {
                    for (int j = 0; j < jobs.Count; j++)
                    {
                        for (int e = 0; e < employees.Count; e++)
                        {
                            cofficientsOfRang[(j * employees.Count) + e] += (long)Math.Round(Math.Round(rankingWrappers[turnForRanking].rang[e, j], 4) * 10000);
                        }
                    }
                }

                else if (rankingWrappers[turnForRanking].rankingName == "Skills")
                {
                    for (int j = 0; j < jobs.Count; j++)
                    {
                        for (int e = 0; e < employees.Count; e++)
                        {
                            cofficientsOfRang[(j * employees.Count) + e] += TansformDef(rankingWrappers[turnForRanking].rang[e, j]);
                        }
                    }
                }
            }
        }

        public long TansformDef(float rang)
        {
            return (long)Math.Round(Math.Round(rang, 4) * 10000);
        }

        private void DynamicConstraintImplement(CpModel model, List<RankingWrapper> rankingWrappers, int emplCount, int turnForRanking, int startingPositionForClausa, LinearExpr[] linearExprRang, bool resticted, bool includeSortOrder, int maxSortOrder)
        {
            int scaleSortOrder = includeSortOrder ? (int)Math.Pow(10, maxSortOrder - rankingWrappers[turnForRanking].sortOrder) : 1;
            if (rankingWrappers[turnForRanking].isDynamic == true)
            {
                for (int con = 0; con < rankingWrappers[turnForRanking].conditions.Count; con++)
                {
                    // 07/08

                    //dynamicConstraintLower[startingPositionForClausa + con] = model.NewIntVar(1000, 1000, "Lower limit claus: " + turnForRanking);
                    //dynamicConstraintUpper[startingPositionForClausa + con] = model.NewIntVar(1000, 1000, "Upper limit claus: " + turnForRanking);

                    // 29/06/2020

                    dynamicConstraintLower[startingPositionForClausa + con] = model.NewIntVar(0, jobs.Count * 200000, "Lower limit claus: " + turnForRanking);
                    dynamicConstraintUpper[startingPositionForClausa + con] = model.NewIntVar(0, jobs.Count * 200000, "Upper limit claus: " + turnForRanking);

                    //dynamicConstraintLower[startingPositionForClausa + con] = model.NewConstant(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmout));
                    //dynamicConstraintUpper[startingPositionForClausa + con] = model.NewConstant(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].upperAmout));

                    if (rankingWrappers[turnForRanking].rankingName == "Week Freq")
                    {
                        for (int cla = 0; cla < rankingWrappers[turnForRanking].conditions[con].clasuses.Count; cla++)
                        {
                            ILiteral[] limitEmployeesWork = new ILiteral[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek.Count];

                            for (int dateWeek = 0; dateWeek < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek.Count; dateWeek++)
                            {
                                List<ILiteral> jobsOnTheSameDayWeek = new List<ILiteral>();
                                for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek].Count; assigment++)
                                {
                                    jobsOnTheSameDayWeek.Add(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek][assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].jobsPerDayOfTheWeek[dateWeek][assigment].GetValue(0, 1)]);
                                }

                                limitEmployeesWork[dateWeek] = model.NewBoolVar("If employee " + cla + " works on " + dateWeek);
                                model.Add(LinearExpr.Sum(jobsOnTheSameDayWeek) == 0).OnlyEnforceIf(limitEmployeesWork[dateWeek].Not());
                                model.Add(LinearExpr.Sum(jobsOnTheSameDayWeek) >= 1).OnlyEnforceIf(limitEmployeesWork[dateWeek]);
                            }

                            if (!resticted)
                            {
                                model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].upperAmout) >= (LinearExpr.Term(LinearExpr.Sum(limitEmployeesWork), 100 * scaleSortOrder) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100 * scaleSortOrder)));
                            }
                            else
                            {
                                model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].upperAmout) > (LinearExpr.Term(LinearExpr.Sum(limitEmployeesWork), 100 * scaleSortOrder) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100 * scaleSortOrder)));
                            }

                            if (!resticted)
                            {
                                model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmout) <= (LinearExpr.Term(LinearExpr.Sum(limitEmployeesWork), 100 * scaleSortOrder) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100 * scaleSortOrder)));
                            }
                            else
                            {
                                model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmout) < (LinearExpr.Term(LinearExpr.Sum(limitEmployeesWork), 100 * scaleSortOrder) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100)) * scaleSortOrder);
                            }

                        }
                    }
                    else
                    {
                        for (int cla = 0; cla < rankingWrappers[turnForRanking].conditions[con].clasuses.Count; cla++)
                        {


                            LinearExpr[] limitEmployeesWork = new LinearExpr[rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count];

                            if (rankingWrappers[turnForRanking].rankingName == "Hours")
                            {
                                for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                {
                                    limitEmployeesWork[assigment] = LinearExpr.Term(matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 1)], rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusaHours[assigment] * scaleSortOrder);
                                }
                            }
                            else if (rankingWrappers[turnForRanking].rankingName == "Job Freq")
                            {
                                for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                {
                                    limitEmployeesWork[assigment] = LinearExpr.Term(matrix[(int)rankingWrappers[(int)turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 1)], 100);
                                    limitEmployeesWork[assigment] = LinearExpr.Term(limitEmployeesWork[assigment], scaleSortOrder);
                                }

                                if (!resticted)
                                {
                                    model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmout) <= LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalJobTypes[rankingWrappers[turnForRanking].jobTypeID] * 100));
                                }
                                else
                                {
                                    model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmout) < LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalJobTypes[rankingWrappers[turnForRanking].jobTypeID] * 100));
                                }

                                continue;
                            }
                            else if (rankingWrappers[turnForRanking].rankingName == "Coverage")
                            {
                                for (int assigment = 0; assigment < rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa.Count; assigment++)
                                {
                                    limitEmployeesWork[assigment] = (LinearExpr)matrix[(int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 0), (int)rankingWrappers[turnForRanking].conditions[con].clasuses[cla].calsusa[assigment].GetValue(0, 1)];
                                    limitEmployeesWork[assigment] = LinearExpr.Term(limitEmployeesWork[assigment], scaleSortOrder);
                                }
                                goto onlyLower;
                            }

                            //model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].upperAmout) >= LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100));

                            if (!resticted)
                            {
                                model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].upperAmout) >= LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100));
                            }
                            else
                            {
                                model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].upperAmout) > LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100));
                            }
                        onlyLower:
                            if (!resticted)
                            {
                                model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmout) <= LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100));
                            }
                            else
                            {
                                model.Add(Convert.ToInt32(rankingWrappers[turnForRanking].conditions[con].lowerAmout) < LinearExpr.Sum(limitEmployeesWork) + (long)(rankingWrappers[turnForRanking].conditions[con].clasuses[cla].externalAmount * 100));
                            }
                        }
                    }
                }
            }
            else
            {
                linearExprRang = new LinearExpr[jobs.Count * employees.Count];

                for (int j = 0; j < jobs.Count; j++)
                {
                    for (int e = 0; e < employees.Count; e++)
                    {
                        linearExprRang[(j * employees.Count) + e] = LinearExpr.Term(matrix[e, j], TansformDef(rankingWrappers[turnForRanking].rang[e, j]) * scaleSortOrder);
                    }
                }

                model.Add(LinearExpr.Sum(linearExprRang) == rankingWrappers[turnForRanking].preciseAmout);
            }
        }

        /// <summary>
        /// Setting numer of workers which is going to be applied 
        /// </summary>
        /// <remarks>
        /// It does have seance to apply after finding maximum of possible applies.
        /// Workers will be rotate so it has uniform distibution of the missing slots.
        /// </remarks>
        public void SetMaxEmployeeForSchedule(int maxEmoloyeeNumber)
        {
            ILiteral[] tmpLin = new ILiteral[jobs.Count * employees.Count];
            for (int j = 0; j < jobs.Count; j++)
            {
                for (int e = 0; e < employees.Count; e++)
                {
                    tmpLin[(j * employees.Count) + e] = matrix[e, j];
                }
            }

            model.Add(LinearExpr.Sum(tmpLin) == maxEmoloyeeNumber);
        }

        public bool? IsDynamicOfRanking(ScheduleRanking scheduleRanking, List<RankingRule> rankingRules, List<RankingType> rankingTypes)
        {
            bool? isDynamicOfRanking = null;
            foreach (RankingRule rankingRule in rankingRules)
            {
                foreach (RankingType rankingType in rankingTypes)
                {
                    if ((scheduleRanking.RankingRuleID == rankingRule.Id) && (scheduleRanking.RankingTypeID == rankingType.Id))
                    {
                        switch (rankingType.Name)
                        {
                            case "Default job":
                                isDynamicOfRanking = false;
                                goto labelofend;
                            case "Scheduled Hours":
                                isDynamicOfRanking = true;
                                goto labelofend;
                            case "Job Type Frequency":
                                isDynamicOfRanking = true;
                                goto labelofend;
                            case "Day of week frequency":
                                isDynamicOfRanking = true;
                                goto labelofend;
                            case "Skills":
                                isDynamicOfRanking = false;
                                goto labelofend;
                        }
                    }
                }

            }
        labelofend:
            return isDynamicOfRanking;
        }

        private static long CalculateTime<T>(T scheduleJob, DateTime[] dateTime) where T : IJobBase
        {
            long totalMinutes = 0;
            if (scheduleJob.JobStartDate.Date != scheduleJob.JobEndDate.Date)
            {
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(0) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(0) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(0) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT1.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(1) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(1) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(1) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT2.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(2) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(2) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(2) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT3.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(3) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(3) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(3) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT4.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(4) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(4) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(4) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT5.TotalMinutes : 0;
                totalMinutes += (scheduleJob.JobStartDate.Date.AddDays(5) >= dateTime[0]) && (scheduleJob.JobStartDate.Date.AddDays(5) <= dateTime[1]) && (scheduleJob.JobStartDate.Date.AddDays(5) <= scheduleJob.JobEndDate) ? (long)scheduleJob.HoursT6.TotalMinutes : 0;

                return totalMinutes;
            }
            else
            {
                return ((scheduleJob.JobStartDate >= dateTime[0]) && (scheduleJob.JobEndDate <= dateTime[1])) ? (long)scheduleJob.HoursT.TotalMinutes : 0;
            }
        }
        /// <summary>
        /// Setting max hours per employeee
        /// For the purpose of testing [It is not constraint!]
        /// This create diffrence between auto-solve and optimaze
        /// </summary>
        /// <code>
        /// foreach (int e in setOfEmployees)
        /// {
        ///     IntVar[] tmp = new IntVar[jobs.Count];
        ///     foreach (int j in setOfJobs)
        ///     {
        ///         tmp[j] = matrix[e, j];
        ///     }
        ///      model.Add(jobs.Count * 2 / employees.Count <= LinearExpr.Sum(tmp) <= jobs.Count / 2);
        ///      model.Add(LinearExpr.Sum(tmp) <= jobs.Count / 2);
        /// }
        /// </code>

    }
}
