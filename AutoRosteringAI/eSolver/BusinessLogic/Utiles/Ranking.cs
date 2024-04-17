using eSolver.Entities;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eSolver.BusinessLogic.Utiles
{
    public static class Ranking
    {
        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }
        static public List<Employee> SortEmployeesByEmployeeField(string employeeField, bool order, List<Employee> employees)
        {
            // true>false => order by descending, true will be on the top of the list
            switch (employeeField)
            {
                case "Firstname":
                    employees = order ?
                        employees.OrderByDescending(x => x.Firstname).ToList() :
                        employees.OrderBy(x => x.Firstname).ToList();
                    break;
                case "Surname":
                    employees = order ?
                        employees.OrderByDescending(x => x.Surname).ToList() :
                        employees.OrderBy(x => x.Surname).ToList();
                    break;
                case "Address1":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Address1)).ThenByDescending(x => x.Address1).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Address1)).ThenBy(x => x.Address1).ToList();
                    break;
                case "Address2":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Address2)).ThenByDescending(x => x.Address2).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Address2)).ThenBy(x => x.Address2).ToList();
                    break;
                case "Address3":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Address3)).ThenByDescending(x => x.Address3).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Address3)).ThenBy(x => x.Address3).ToList();
                    break;
                case "Address4":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Address4)).ThenByDescending(x => x.Address4).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Address4)).ThenBy(x => x.Address4).ToList();
                    break;
                case "Email":
                    employees = order ?
                        employees.OrderByDescending(x => x.Email).ToList() :
                        employees.OrderBy(x => x.Email).ToList();
                    break;
                case "EmployeeNumber":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.EmployeeNumber)).ThenByDescending(x => x.EmployeeNumber).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.EmployeeNumber)).ThenBy(x => x.EmployeeNumber).ToList();
                    break;
                case "MaxHours1":
                    employees = order ?
                        employees.OrderBy(x => x.MaxHours1 == null).ThenByDescending(x => x.MaxHours1).ToList() :
                        employees.OrderBy(x => x.MaxHours1 == null).ThenBy(x => x.MaxHours1).ToList();
                    break;
                case "MaxHours2":
                    employees = order ?
                        employees.OrderBy(x => x.MaxHours2 == null).ThenByDescending(x => x.MaxHours2).ToList() :
                        employees.OrderBy(x => x.MaxHours2 == null).ThenBy(x => x.MaxHours2).ToList();
                    break;
                case "MaxHours3":
                    employees = order ?
                        employees.OrderBy(x => x.MaxHours3 == null).ThenByDescending(x => x.MaxHours3).ToList() :
                        employees.OrderBy(x => x.MaxHours3 == null).ThenBy(x => x.MaxHours3).ToList();
                    break;
                case "MaxHours4":
                    employees = order ?
                        employees.OrderBy(x => x.MaxHours4 == null).ThenByDescending(x => x.MaxHours4).ToList() :
                        employees.OrderBy(x => x.MaxHours4 == null).ThenBy(x => x.MaxHours4).ToList();
                    break;
                case "MaxHours5":
                    employees = order ?
                         employees.OrderBy(x => x.MaxHours5 == null).ThenByDescending(x => x.MaxHours5).ToList() :
                         employees.OrderBy(x => x.MaxHours5 == null).ThenBy(x => x.MaxHours5).ToList();
                    break;
                case "MobileNumber":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.MobileNumber)).ThenByDescending(x => x.MobileNumber).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.MobileNumber)).ThenBy(x => x.MobileNumber).ToList();
                    break;
                case "PayRate":
                    employees = order ?
                         employees.OrderBy(x => x.PayRate == null).ThenByDescending(x => x.PayRate).ToList() :
                         employees.OrderBy(x => x.PayRate == null).ThenBy(x => x.PayRate).ToList();
                    break;
                case "TelephoneNumber":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.TelephoneNumber)).ThenByDescending(x => x.TelephoneNumber).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.TelephoneNumber)).ThenBy(x => x.TelephoneNumber).ToList();
                    break;
                case "Profile":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Profile)).ThenByDescending(x => x.Profile).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Profile)).ThenBy(x => x.Profile).ToList();
                    break;
                case "Gender":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Gender)).ThenByDescending(x => x.Gender).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Gender)).ThenBy(x => x.Gender).ToList();
                    break;
                case "Location":
                    /*
                    List<int> employeeScoreForSort = new List<int>();

                    for (int e = 0; e < employees.Count; e++)
                    {
                        if(employees[e].Locations == null) // employees[e].Location == null && 
                        {
                            employeeScoreForSort.Add(0);
                        }
                        else
                        {
                            if(order)
                            {
                                for (int i = 0; i < employees[e].Locations.Count; i++)
                                {
                                    if(employees[e].Locations[i] ==  )
                                    
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    */
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Location)).ThenByDescending(x => x.Location).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Location)).ThenBy(x => x.Location).ToList();
                    break;
                case "Department":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Department)).ThenByDescending(x => x.Department).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Department)).ThenBy(x => x.Department).ToList();
                    break;
                case "CostCode":
                    employees = order ?
                        employees.OrderBy(x => x.CostCode == null).ThenByDescending(x => x.CostCode).ToList() :
                         employees.OrderBy(x => x.CostCode == null).ThenBy(x => x.CostCode).ToList();
                    break;
                case "Division":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Division)).ThenByDescending(x => x.Division).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Division)).ThenBy(x => x.Division).ToList();
                    break;
                case "JobTitle":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.JobTitle)).ThenByDescending(x => x.JobTitle).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.JobTitle)).ThenBy(x => x.JobTitle).ToList();
                    break;
                case "Team":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Team)).ThenByDescending(x => x.Team).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Team)).ThenBy(x => x.Team).ToList();
                    break;
                case "Class":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Class)).ThenByDescending(x => x.Class).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Class)).ThenBy(x => x.Class).ToList();
                    break;
                case "TargetRuleGroup":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.TargetRuleGroup)).ThenByDescending(x => x.TargetRuleGroup).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.TargetRuleGroup)).ThenBy(x => x.TargetRuleGroup).ToList();
                    break;
                case "Notification":
                    employees = order ?
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Notification)).ThenByDescending(x => x.Notification).ToList() :
                        employees.OrderBy(x => string.IsNullOrEmpty(x.Notification)).ThenBy(x => x.Notification).ToList();
                    break;
                default:
                    break;
            }
            return employees;
        }

        public static bool IsCompared(RankingRule rankingRule, List<ScheduleCustomData> customData, Employee updatedEmployee, ScheduleJob scheduleJob)
        {
            bool isTrue = true;

            //Console.WriteLine(comparison);
            if (rankingRule.ComparisonMode == "Set Value")
            {
                if (rankingRule.EmployeeField.ToString() == "Address 1")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Address1);
                }
                else if (rankingRule.EmployeeField.ToString() == "Address 2")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Address2);
                }
                else if (rankingRule.EmployeeField.ToString() == "Address 3")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Address3);
                }
                else if (rankingRule.EmployeeField.ToString() == "Address 4")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Address4);
                }
                else if (rankingRule.EmployeeField.ToString() == "Class")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Class);
                }
                else if (rankingRule.EmployeeField.ToLower().ToString() == "cost code")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.CostCode);
                }
                else if (rankingRule.EmployeeField.ToString() == "Department")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Department);
                }
                else if (rankingRule.EmployeeField.ToString() == "Division")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Division);
                }
                else if (rankingRule.EmployeeField.ToString() == "Email")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Email);
                }
                else if (rankingRule.EmployeeField.ToLower().ToString() == "employee number")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.EmployeeNumber);                  
                }
                else if (rankingRule.EmployeeField.ToString() == "Profile")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Profile);
                }
                else if (rankingRule.EmployeeField.ToString() == "Firstname")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Firstname);
                }
                else if (rankingRule.EmployeeField.ToString() == "Gender")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Gender);
                }
                else if (rankingRule.EmployeeField.ToLower().ToString() == "job title")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.JobTitle);
                }
                else if (rankingRule.EmployeeField.ToLower().ToString() == "job type")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.JobTitle);
                }
                else if (rankingRule.EmployeeField.ToString() == "Location")
                {

                    if (updatedEmployee.Locations != null)
                    {
                        foreach (string location in updatedEmployee.Locations)
                        {
                            foreach (string value in rankingRule.ComparisonValues)
                            {
                                if (location == value && rankingRule.Operator.Equals("="))
                                {
                                    isTrue = true;
                                    goto jump_ranking_copmarison_1;
                                }

                                if (location == value && rankingRule.Operator.Equals("<>"))
                                {
                                    isTrue = false;
                                    goto jump_ranking_copmarison_1;
                                }
                            }
                        }
                        if (rankingRule.Operator.Equals("="))
                        {
                            isTrue = false;
                        }
                        else
                        {
                            isTrue = true;
                        }
                    }
                    else
                    {
                        isTrue = true;
                    }
                jump_ranking_copmarison_1:;
                }
                else if (rankingRule.EmployeeField.ToString() == "Manager")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Manager);
                }
                else if (rankingRule.EmployeeField.ToString() == "Max Hours 1")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, Convert.ToString(updatedEmployee.MaxHours1));
                }
                else if (rankingRule.EmployeeField.ToString() == "Max Hours 2")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, Convert.ToString(updatedEmployee.MaxHours2));
                }
                else if (rankingRule.EmployeeField.ToString() == "Max Hours 3")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, Convert.ToString(updatedEmployee.MaxHours3));
                }
                else if (rankingRule.EmployeeField.ToString() == "Max Hours 4")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, Convert.ToString(updatedEmployee.MaxHours4));
                }
                else if (rankingRule.EmployeeField.ToString() == "Max Hours 5")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, Convert.ToString(updatedEmployee.MaxHours5));
                }
                else if (rankingRule.EmployeeField.ToLower().ToString() == "mobile number")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, Convert.ToString(updatedEmployee.MobileNumber));
                }
                else if (rankingRule.EmployeeField.ToString() == "Notification")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Notification);
                }
                else if (rankingRule.EmployeeField.ToString() == "Password")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Password);
                }
                else if (rankingRule.EmployeeField.ToLower().ToString() == "pay rate")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.PayRate);
                }
                else if (rankingRule.EmployeeField.ToString() == "Surname")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Surname);
                }
                else if (rankingRule.EmployeeField.ToString() == "Team")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Team);
                }
                else if (rankingRule.EmployeeField.ToLower().ToString() == "telephone number")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.TelephoneNumber);
                }
                else if (rankingRule.EmployeeField.ToString() == "Username")
                {
                    isTrue = CheckIfComparsionIsValid(rankingRule, updatedEmployee.Username);
                }
            }
            else if (rankingRule.ComparisonMode != null && rankingRule.ComparisonMode.ToString() == "Custom Data")// && constraint.CustomDataFieldID != null)
            {
                if (scheduleJob.JobCustomData.Count == 0)
                {
                    return true;
                }
                foreach (ScheduleCustomData scheduleCustomData in scheduleJob.JobCustomData)
                {
                    /*if ((scheduleCustomData.CustomDataLookupID != null) && (scheduleCustomData.CustomDataLookupContent != null) &&
                    (scheduleCustomData.CustomDataLookupContent != null) &&
                    (scheduleCustomData.NumberValue != null) &&
                    (scheduleCustomData.TextValue != null))*/
                    {


                        if (scheduleCustomData.CustomDataID == rankingRule.CustomDataID)
                        {
                            if (scheduleCustomData.CustomDataLookupID != null)
                            {

                                if (rankingRule.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }

                                if (rankingRule.EmployeeField.ToString() == "Address1")
                                {
                                    if (((updatedEmployee.Address1 != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                            || ((updatedEmployee.Address1 == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {

                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Address2")
                                {
                                    if (((updatedEmployee.Address2 != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                            || ((updatedEmployee.Address2 == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {

                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Address3")
                                {
                                    if (((updatedEmployee.Address3 != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                            || ((updatedEmployee.Address3 == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {

                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Address4")
                                {
                                    if (((updatedEmployee.Address4 != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                            || ((updatedEmployee.Address4 == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {

                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Class")
                                {
                                    if (((updatedEmployee.Class != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Class == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "CostCode")
                                {
                                    if (((updatedEmployee.CostCode != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.CostCode == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Department")
                                {
                                    if (((updatedEmployee.Department != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Department == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Division")
                                {
                                    if (((updatedEmployee.Division != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Division == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Email")
                                {
                                    if (((updatedEmployee.Email != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Email == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "EmployeeNumber")
                                {
                                    if (((updatedEmployee.EmployeeNumber.ToString() != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.EmployeeNumber.ToString() == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Profile")
                                {
                                    if (((updatedEmployee.Profile.ToString() != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Profile.ToString() == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Firstname")
                                {
                                    if (((updatedEmployee.Firstname != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Firstname == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Gender")
                                {
                                    if (((updatedEmployee.Gender != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Gender == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "JobTitle")
                                {
                                    if (((updatedEmployee.JobTitle != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.JobTitle == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Location")
                                {

                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {

                                            if ((location != scheduleCustomData.CustomDataLookupContent && rankingRule.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.CustomDataLookupContent && rankingRule.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump_ranking_custom_data_1;
                                            }
                                        }
                                    }
                                    isTrue = false;
                                jump_ranking_custom_data_1:;
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Manager")
                                {
                                    if (((updatedEmployee.Manager.ToString() != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Manager.ToString() == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "MaxHours1")
                                {
                                    if (((updatedEmployee.MaxHours1 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours1 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "MaxHours2")
                                {
                                    if (((updatedEmployee.MaxHours2 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours2 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "MaxHours3")
                                {
                                    if (((updatedEmployee.MaxHours3 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours3 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "MaxHours4")
                                {
                                    if (((updatedEmployee.MaxHours4 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours4 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "MaxHours5")
                                {
                                    if (((updatedEmployee.MaxHours5 != Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours5 == Int32.Parse(scheduleCustomData.CustomDataLookupContent)) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "MobileNumber")
                                {
                                    if (((updatedEmployee.MobileNumber != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MobileNumber == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Notification")
                                {
                                    if (((updatedEmployee.Notification != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Notification == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Password")
                                {
                                    if (((updatedEmployee.Password != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Password == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "PayRate")
                                {
                                    if (((updatedEmployee.PayRate != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.PayRate == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Surname")
                                {
                                    if (((updatedEmployee.Surname != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Surname == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Team")
                                {
                                    if (((updatedEmployee.Team != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Team == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "TelephoneNumber")
                                {
                                    if (((updatedEmployee.TelephoneNumber != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.TelephoneNumber == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField.ToString() == "Username")
                                {
                                    if (((updatedEmployee.Username != scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Username == scheduleCustomData.CustomDataLookupContent) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                            }
                            else if (scheduleCustomData.CustomDataLookupID == null)
                            {
                                if (rankingRule.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }
                                if (rankingRule.EmployeeField == "Address1")
                                {
                                    if (((updatedEmployee.Address1 != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Address1 == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Address2")
                                {
                                    if (((updatedEmployee.Address2 != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Address2 == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Address3")
                                {
                                    if (((updatedEmployee.Address3 != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Address3 == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Address4")
                                {
                                    if (((updatedEmployee.Address4 != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Address4 == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Class")
                                {
                                    if (((updatedEmployee.Class != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Class == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "CostCode")
                                {
                                    if (((updatedEmployee.CostCode != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.CostCode == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Department")
                                {
                                    if (((updatedEmployee.Department != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Department == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Division")
                                {
                                    if (((updatedEmployee.Division != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Division == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Email")
                                {
                                    if (((updatedEmployee.Email != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Email == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "EmployeeNumber")
                                {
                                    if (((updatedEmployee.EmployeeNumber.ToString() != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.EmployeeNumber.ToString() == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Profile")
                                {
                                    if (((updatedEmployee.Profile.ToString() != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Profile.ToString() == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Firstname")
                                {
                                    if (((updatedEmployee.Firstname != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Firstname == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Gender")
                                {
                                    if (((updatedEmployee.Gender != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Gender == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "JobTitle")
                                {
                                    if (((updatedEmployee.JobTitle != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.JobTitle == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Location")
                                {
                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {

                                            if ((location != scheduleCustomData.CustomDataLookupContent && rankingRule.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.CustomDataLookupContent && rankingRule.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump_ranking_custom_data_lookup_1;
                                            }
                                        }
                                    }
                                    isTrue = false;
                                jump_ranking_custom_data_lookup_1:;
                                }
                                else if (rankingRule.EmployeeField == "Manager")
                                {
                                    if (((updatedEmployee.Manager.ToString() != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Manager.ToString() == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "MaxHours1")
                                {
                                    if (((updatedEmployee.MaxHours1 != scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours1 == scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "MaxHours2")
                                {
                                    if (((updatedEmployee.MaxHours2 != scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours2 == scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "MaxHours3")
                                {
                                    if (((updatedEmployee.MaxHours3 != scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours3 == scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "MaxHours4")
                                {
                                    if (((updatedEmployee.MaxHours4 != scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours4 == scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "MaxHours5")
                                {
                                    if (((updatedEmployee.MaxHours5 != scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MaxHours5 == scheduleCustomData.NumberValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "MobileNumber")
                                {
                                    if (((updatedEmployee.MobileNumber != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.MobileNumber == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Notification")
                                {
                                    if (((updatedEmployee.Notification != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Notification == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Password")
                                {
                                    if (((updatedEmployee.Password != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Password == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "PayRate")
                                {
                                    if (((updatedEmployee.PayRate != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.PayRate == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Surname")
                                {
                                    if (((updatedEmployee.Surname != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Surname == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Team")
                                {
                                    if (((updatedEmployee.Team != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Team == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "TelephoneNumber")
                                {
                                    if (((updatedEmployee.TelephoneNumber != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.TelephoneNumber == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                else if (rankingRule.EmployeeField == "Username")
                                {
                                    if (((updatedEmployee.Username != scheduleCustomData.TextValue) && rankingRule.Operator.Equals("="))
                                        || ((updatedEmployee.Username == scheduleCustomData.TextValue) && rankingRule.Operator.Equals("<>")))
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
            return isTrue;
        }

    
        public static bool CheckIfComparsionIsValid(RankingRule rankingRule,string employeeField)
        {
            float i = 0;
            float j = 0;
            bool isTrue = true;
            bool isEmployeeFieldValueNumeric = false;
            bool isComparisonValueNumeric = false;
            if (rankingRule.Operator.Equals(">") || rankingRule.Operator.Equals("<"))
            {
                isEmployeeFieldValueNumeric = float.TryParse(employeeField, out i);
                isComparisonValueNumeric = float.TryParse(rankingRule.ComparisonValue, out j);
                if (isEmployeeFieldValueNumeric && isComparisonValueNumeric)
                {
                    if (rankingRule.Operator.Equals(">"))
                    {
                        isTrue = i > j;
                    }
                    if (rankingRule.Operator.Equals("<"))
                    {
                        isTrue = i < j;
                    }
                }
            }
            else
            {
                if (((employeeField != rankingRule.ComparisonValue) && rankingRule.Operator.Equals("="))
                    || ((employeeField == rankingRule.ComparisonValue) && rankingRule.Operator.Equals("<>")))
                {
                    isTrue = false;
                }
            }
              
            return isTrue;
        }
    }
}