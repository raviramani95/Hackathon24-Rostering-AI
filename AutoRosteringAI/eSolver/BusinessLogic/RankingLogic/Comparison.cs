using eSolver.Entities.Constraints.Untiles;
using static eSolver.BusinessLogic.Utiles.Compared;
using eSolver.Entities;
using System;
using System.Collections.Generic;
using eSolver.Entities.Interfaces;

namespace eSolver.AutoSolverNew.RankingLogic
{
    static partial class Comparison
    {
        public static bool IsCompared(RankingRule rankingRule, List<ScheduleCustomData> customData, Employee updatedEmployee, IJobBase scheduleJob)
        {
            bool isTrue = true;

            //Console.WriteLine(comparison);

            if (rankingRule.ComparisonMode != null && rankingRule.ComparisonMode.Equals("Set Value"))
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
            else if (rankingRule != null)// && constraint.CustomDataFieldID != null)
            {
                if (scheduleJob.JobCustomData.Count == 0)
                {
                    return true;
                }

                foreach (ScheduleCustomData scheduleCustomData in scheduleJob.JobCustomData)
                {
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

                                if (rankingRule.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Profile"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Profile, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, updatedEmployee.JobTitle, updatedEmployee.JobTitle);
                                }
                                else if (rankingRule.EmployeeField.ToLower().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, updatedEmployee.JobTitle, updatedEmployee.JobTitle);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Location"))
                                {

                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.CustomDataLookupContent && rankingRule.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.CustomDataLookupContent && rankingRule.Operator.Equals("<>")))
                                            {
                                                isTrue = false;
                                                goto jump2;
                                            }
                                        }
                                    }
                                    
                                jump2:;
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("mobile number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("pay rate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("telephone number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.CustomDataLookupContent, rankingRule.Operator);
                                }
                            }
                            else if (scheduleCustomData.CustomDataLookupID == null)
                            {
                                if (rankingRule.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }
                                if (rankingRule.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Profile"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Profile, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Location"))
                                {
                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.TextValue && rankingRule.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.TextValue && rankingRule.Operator.Equals("<>")))
                                            {
                                                isTrue = false;
                                                goto jump3;
                                            }
                                        }
                                    }
                                    
                                jump3:;
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), Convert.ToString(scheduleCustomData.NumberValue), rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), Convert.ToString(scheduleCustomData.NumberValue), rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), Convert.ToString(scheduleCustomData.NumberValue), rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), Convert.ToString(scheduleCustomData.NumberValue), rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), Convert.ToString(scheduleCustomData.NumberValue), rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("mobile number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("pay rate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToLower().ToString().Equals("telephone number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                                else if (rankingRule.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.TextValue, rankingRule.Operator);
                                }
                            }

                        }
                    }
                }


            }
            return isTrue;
        }

        public static bool CheckIfComparsionIsValid(RankingRule rankingRule, string employeeField)
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