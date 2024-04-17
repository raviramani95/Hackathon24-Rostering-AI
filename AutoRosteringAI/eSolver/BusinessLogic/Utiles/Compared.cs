using eSolver.Entities.Constraints.Untiles;
using eSolver.Entities;
using eSolver.Entities.OutSideView;
using System;
using System.Collections.Generic;
using eSolver.Entities.Interfaces;

namespace eSolver.BusinessLogic.Utiles
{
    public static class Compared
    {

        public static bool IsCompared(ScheduleActiveConstraint constraint, List<ScheduleCustomData> customData, Employee updatedEmployee, ScheduleJob scheduleJob)
        {
            bool isTrue = true;

            ConstraintCustomData constraintCustom = new ConstraintCustomData();
            ConstraintSetValues constraintSetValue = new ConstraintSetValues();
            if (constraint.ConstraintMNOH != null)
            {
                constraintSetValue = constraint.ConstraintMNOH.ConstraintSetValue;
                constraintCustom = constraint.ConstraintMNOH.ConstraintCustomData;
            }
            else if (constraint.ConstraintMNODOTW != null)
            {
                constraintSetValue = constraint.ConstraintMNODOTW.ConstraintSetValue;
                constraintCustom = constraint.ConstraintMNODOTW.ConstraintCustomData;
            }
            else if (constraint.ConstraintSS != null)
            {
                constraintSetValue = constraint.ConstraintSS.ConstraintSetValue;
                constraintCustom = constraint.ConstraintSS.ConstraintCustomData;
            }
            else if (constraint.ConstraintMNOJT != null)
            {
                constraintSetValue = constraint.ConstraintMNOJT.ConstraintSetValue;
                constraintCustom = constraint.ConstraintMNOJT.ConstraintCustomData;
            }
            else if (constraint.ConstraintMNOJTI != null)
            {
                constraintSetValue = constraint.ConstraintMNOJTI.ConstraintSetValue;
                constraintCustom = constraint.ConstraintMNOJTI.ConstraintCustomData;
            }

            //Console.WriteLine(comparison);
            if (constraintSetValue != null)
            {
                if (constraintSetValue.EmployeeField.Equals("Address 1"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address1);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 2"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address2);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 3"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address3);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 4"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address4);
                }
                else if (constraintSetValue.EmployeeField.Equals("Class"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Class);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("cost code"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.CostCode);
                }
                else if (constraintSetValue.EmployeeField.Equals("Department"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Department);
                }
                else if (constraintSetValue.EmployeeField.Equals("Division"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Division);
                }
                else if (constraintSetValue.EmployeeField.Equals("Email"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Email);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("employee number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.EmployeeNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Firstname"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Firstname);
                }
                else if (constraintSetValue.EmployeeField.Equals("Gender"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Gender);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("job title"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.JobTitle);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Jobtype);
                }
                else if (constraintSetValue.EmployeeField.Equals("Location"))
                {
                    if (updatedEmployee.Locations != null && constraint.ComparisonValues != null)
                    {
                        foreach (string location in updatedEmployee.Locations)
                        {
                            foreach (string value in constraint.ComparisonValues)
                            {
                                if (location == value && constraintSetValue.Operator.Equals("="))
                                {
                                    isTrue = true;
                                    goto jump1;
                                }    
                                
                                if(location == value && constraintSetValue.Operator.Equals("<>"))
                                {
                                    isTrue = false;
                                    goto jump1;
                                }
                            }
                        }
                        if (constraintSetValue.Operator.Equals("="))
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
                jump1:;
                }
                else if (constraintSetValue.EmployeeField.Equals("Manager"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Manager);
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 1"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours1));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 2"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours2));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 3"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours3));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 4"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours4));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 5"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours5));
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("mobile number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.MobileNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Notification"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Notification);
                }
                else if (constraintSetValue.EmployeeField.Equals("Password"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Password);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("pay rate"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.PayRate);
                }
                else if (constraintSetValue.EmployeeField.Equals("Surname"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Surname);
                }
                else if (constraintSetValue.EmployeeField.Equals("Team"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Team);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("telephone number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.TelephoneNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Username"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Username);
                }
            }

            else if (constraintCustom != null)// && constraint.CustomDataFieldID != null)
            {
                if (scheduleJob.JobCustomData.Count == 0)
                {
                    return false;
                }

                foreach (ScheduleCustomData scheduleCustomData in scheduleJob.JobCustomData)
                {
                    {
                        if (scheduleCustomData.CustomDataID == constraintCustom.CustomDataID)
                        {

                            if (scheduleCustomData.CustomDataLookupID != null)
                            {

                                if (constraintCustom.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }

                                if (constraintCustom.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                    break;
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);                                    
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);                                   
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);                                    
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);                                    
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Location"))
                                {
                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.CustomDataLookupContent && constraintSetValue.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.CustomDataLookupContent && constraintSetValue.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump2;
                                            }
                                        }
                                    }
                                    if (constraintSetValue.Operator.Equals("="))
                                    {
                                        isTrue = false;
                                    }
                                    else
                                    {
                                        isTrue = true;
                                    }
                                jump2:;
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("mobile number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("PayRate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("TelephoneNumber"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                            }
                            else if (scheduleCustomData.CustomDataLookupID == null)
                            {
                                if (constraintCustom.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }
                                if (constraintCustom.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.TextValue, constraintCustom.Operator);                                    
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Location"))
                                {
                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.TextValue && constraintSetValue.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.TextValue && constraintSetValue.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump3;
                                            }
                                        }
                                    }
                                    if (constraintSetValue.Operator.Equals("="))
                                    {
                                        isTrue = false;
                                    }
                                    else
                                    {
                                        isTrue = true;
                                    }
                                jump3:;
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("MobileNumber"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("pay rate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("telephone number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                            }

                        }
                    }
                }
            }
            return isTrue;
        }

        public static bool IsCompared(ScheduleActiveConstraint constraint, List<ScheduleCustomData> customData, Employee updatedEmployee, JobFromOutside jobsFromOutside)
        {
            bool isTrue = true;

            ConstraintCustomData constraintCustom = new ConstraintCustomData();
            ConstraintSetValues constraintSetValue = new ConstraintSetValues();
            if (constraint.ConstraintMNOH != null)
            {
                constraintSetValue = constraint.ConstraintSetValues;
                constraintCustom = constraint.ConstraintMNOH.ConstraintCustomData;
            }
            else if (constraint.ConstraintMNODOTW != null)
            {
                constraintSetValue = constraint.ConstraintMNODOTW.ConstraintSetValue;
                constraintCustom = constraint.ConstraintMNODOTW.ConstraintCustomData;
            }
            else if (constraint.ConstraintSS != null)
            {
                constraintSetValue = constraint.ConstraintSS.ConstraintSetValue;
                constraintCustom = constraint.ConstraintSS.ConstraintCustomData;
            }

            //Console.WriteLine(comparison);
            if (constraintSetValue != null)
            {
                if (constraintSetValue.EmployeeField.Equals("Address 1"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address1);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 2"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address2);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 3"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address3);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 4"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address4);
                }
                else if (constraintSetValue.EmployeeField.Equals("Class"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Class);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("cost code"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.CostCode);
                }
                else if (constraintSetValue.EmployeeField.Equals("Department"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Department);
                }
                else if (constraintSetValue.EmployeeField.Equals("Division"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Division);
                }
                else if (constraintSetValue.EmployeeField.Equals("Email"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Email);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("employee number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.EmployeeNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Firstname"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Firstname);
                }
                else if (constraintSetValue.EmployeeField.Equals("Gender"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Gender);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("job title"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.JobTitle);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Jobtype);
                }
                else if (constraintSetValue.EmployeeField.Equals("Location"))
                {
                    if (updatedEmployee.Locations != null && constraint.ComparisonValues != null)
                    {
                        foreach (string location in updatedEmployee.Locations)
                        {
                            foreach (string value in constraint.ComparisonValues)
                            {
                                if (location == value && constraintSetValue.Operator.Equals("="))
                                {
                                    isTrue = true;
                                    goto jump4;
                                }

                                if (location == value && constraintSetValue.Operator.Equals("<>"))
                                {
                                    isTrue = false;
                                    goto jump4;
                                }
                            }
                        }
                        if (constraintSetValue.Operator.Equals("="))
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
                jump4:;
                }
                else if (constraintSetValue.EmployeeField.Equals("Manager"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Manager);
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 1"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours1));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 2"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours2));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 3"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours3));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 4"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours4));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 5"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours5));
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("mobile number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.MobileNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Notification"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Notification);
                }
                else if (constraintSetValue.EmployeeField.Equals("Password"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Password);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("pay rate"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.PayRate);
                }
                else if (constraintSetValue.EmployeeField.Equals("Surname"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Surname);
                }
                else if (constraintSetValue.EmployeeField.Equals("Team"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Team);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("telephone number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.TelephoneNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Username"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Username);
                }
            }

            else if (constraintCustom != null)// && constraint.CustomDataFieldID != null)
            {
                if (jobsFromOutside.JobCustomData.Count == 0)
                {
                    return true;
                }

                foreach (ScheduleCustomData scheduleCustomData in jobsFromOutside.JobCustomData)
                {
                    {
                        if (scheduleCustomData.CustomDataID == constraintCustom.CustomDataID)
                        {

                            if (scheduleCustomData.CustomDataLookupID != null)
                            {

                                if (constraintCustom.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }

                                if (constraintCustom.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);                                    
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Location"))
                                {

                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.CustomDataLookupContent && constraintSetValue.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.CustomDataLookupContent && constraintSetValue.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump5;
                                            }
                                        }
                                    }
                                    if (constraintSetValue.Operator.Equals("="))
                                    {
                                        isTrue = false;
                                    }
                                    else
                                    {
                                        isTrue = true;
                                    }
                                jump5:;
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("mobile number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("pay rate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("telephone number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                            }
                            else if (scheduleCustomData.CustomDataLookupID == null)
                            {
                                if (constraintCustom.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }
                                if (constraintCustom.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.TextValue, constraintCustom.Operator);                                    
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Location"))
                                {
                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.TextValue && constraintSetValue.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.TextValue && constraintSetValue.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump6;
                                            }
                                        }
                                    }
                                    if (constraintSetValue.Operator.Equals("="))
                                    {
                                        isTrue = false;
                                    }
                                    else
                                    {
                                        isTrue = true;
                                    }
                                jump6:;
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("mobile number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("pay rate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("telephone number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                            }
                        }
                    }
                }
            }
            return isTrue;
        }

        public static bool IsCompared(ITmpConstraint constraint, Employee updatedEmployee, JobFromOutside jobsFromOutside)
        {
            bool isTrue = true;

            ConstraintCustomData constraintCustom = constraint.ConstraintCustomData;
            ConstraintSetValues constraintSetValue = constraint.ConstraintSetValue;

            //Console.WriteLine(comparison);
            if (constraintSetValue != null)
            {
                if (constraintSetValue.EmployeeField.Equals("Address 1"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address1);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 2"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address2);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 3"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address3);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 4"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address4);
                }
                else if (constraintSetValue.EmployeeField.Equals("Class"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Class);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("cost code"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.CostCode);
                }
                else if (constraintSetValue.EmployeeField.Equals("Department"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Department);
                }
                else if (constraintSetValue.EmployeeField.Equals("Division"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Division);
                }
                else if (constraintSetValue.EmployeeField.Equals("Email"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Email);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("employee number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.EmployeeNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Firstname"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Firstname);
                }
                else if (constraintSetValue.EmployeeField.Equals("Gender"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Gender);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("job title"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.JobTitle);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Jobtype);
                }
                else if (constraintSetValue.EmployeeField.Equals("Location"))
                {
                    if (updatedEmployee.Locations != null && constraint.ComparisonValues != null)
                    {
                        foreach (string location in updatedEmployee.Locations)
                        {
                            foreach (string value in constraint.ComparisonValues)
                            {
                                if (location == value && constraintSetValue.Operator.Equals("="))
                                {
                                    isTrue = true;
                                    goto jump7;
                                }

                                if (location == value && constraintSetValue.Operator.Equals("<>"))
                                {
                                    isTrue = false;
                                    goto jump7;
                                }
                            }
                        }
                        if (constraintSetValue.Operator.Equals("="))
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
                jump7:;
                }
                else if (constraintSetValue.EmployeeField.Equals("Manager"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Manager);
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 1"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours1));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 2"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours2));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 3"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours3));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 4"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours4));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 5"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours5));
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("mobile number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.MobileNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Notification"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Notification);
                }
                else if (constraintSetValue.EmployeeField.Equals("Password"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Password);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("pay rate"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.PayRate);
                }
                else if (constraintSetValue.EmployeeField.Equals("Surname"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Surname);
                }
                else if (constraintSetValue.EmployeeField.Equals("Team"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Team);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("telephone number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.TelephoneNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Username"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Username);
                }
            }

            else if (constraintCustom != null)// && constraint.CustomDataFieldID != null)
            {
                if (jobsFromOutside.JobCustomData.Count == 0)
                {
                    return true;
                }

                foreach (ScheduleCustomData scheduleCustomData in jobsFromOutside.JobCustomData)
                {
                    {
                        if (scheduleCustomData.CustomDataID == constraintCustom.CustomDataID)
                        {

                            if (scheduleCustomData.CustomDataLookupID != null)
                            {

                                if (constraintCustom.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }

                                if (constraintCustom.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);                                    
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, updatedEmployee.JobTitle, updatedEmployee.JobTitle);
                                }
                                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, updatedEmployee.JobTitle, updatedEmployee.JobTitle);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Location"))
                                {

                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.CustomDataLookupContent && constraintSetValue.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.CustomDataLookupContent && constraintSetValue.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump8;
                                            }
                                        }
                                    }
                                    if (constraintSetValue.Operator.Equals("="))
                                    {
                                        isTrue = false;
                                    }
                                    else
                                    {
                                        isTrue = true;
                                    }
                                jump8:;
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("mobile number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("pay rate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("telephone number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                            }
                            else if (scheduleCustomData.CustomDataLookupID == null)
                            {
                                if (constraintCustom.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }
                                if (constraintCustom.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Location"))
                                {
                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.TextValue && constraintSetValue.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.TextValue && constraintSetValue.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump9;
                                            }
                                        }
                                    }
                                    if (constraintSetValue.Operator.Equals("="))
                                    {
                                        isTrue = false;
                                    }
                                    else
                                    {
                                        isTrue = true;
                                    }
                                jump9:;
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("mobile number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("pay rate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("telephone number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                            }

                        }
                    }
                }


            }
            return isTrue;
        }

        public static bool IsCompared(ITmpConstraint constraint, Employee updatedEmployee, ScheduleJob scheduleJob)
        {
            bool isTrue = true;

            ConstraintCustomData constraintCustom = constraint.ConstraintCustomData;
            ConstraintSetValues constraintSetValue = constraint.ConstraintSetValue;

            //Console.WriteLine(comparison);
            if (constraintSetValue != null)
            {
                if (constraintSetValue.EmployeeField.Equals("Address 1"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address1);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 2"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address2);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 3"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address3);
                }
                else if (constraintSetValue.EmployeeField.Equals("Address 4"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Address4);
                }
                else if (constraintSetValue.EmployeeField.Equals("Class"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Class);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("cost code"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.CostCode);
                }
                else if (constraintSetValue.EmployeeField.Equals("Department"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Department);
                }
                else if (constraintSetValue.EmployeeField.Equals("Division"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Division);
                }
                else if (constraintSetValue.EmployeeField.Equals("Email"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Email);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("employee number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.EmployeeNumber) ;
                }
                else if (constraintSetValue.EmployeeField.Equals("Firstname"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Firstname);
                }
                else if (constraintSetValue.EmployeeField.Equals("Gender"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Gender);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("job title"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.JobTitle);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Jobtype);
                }
                else if (constraintSetValue.EmployeeField.Equals("Location"))
                {
                    if (updatedEmployee.Locations != null && constraint.ComparisonValues != null)
                    {
                        foreach (string location in updatedEmployee.Locations)
                        {
                            foreach (string value in constraint.ComparisonValues)
                            {
                                if (location == value && constraintSetValue.Operator.Equals("="))
                                {
                                    isTrue = true;
                                    goto jump10;
                                }

                                if (location == value && constraintSetValue.Operator.Equals("<>"))
                                {
                                    isTrue = false;
                                    goto jump10;
                                }
                            }
                        }
                        if (constraintSetValue.Operator.Equals("="))
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
                jump10:;
                }
                else if (constraintSetValue.EmployeeField.Equals("Manager"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Manager);
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 1"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours1));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 2"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours2));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 3"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours3));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 4"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours4));
                }
                else if (constraintSetValue.EmployeeField.Equals("Max Hours 5"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, Convert.ToString(updatedEmployee.MaxHours5));
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("mobile number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.MobileNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Notification"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Notification);
                }
                else if (constraintSetValue.EmployeeField.Equals("Password"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Password);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("pay rate"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.PayRate);
                }
                else if (constraintSetValue.EmployeeField.Equals("Surname"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Surname);
                }
                else if (constraintSetValue.EmployeeField.Equals("Team"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Team);
                }
                else if (constraintSetValue.EmployeeField.ToLower().Equals("telephone number"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.TelephoneNumber);
                }
                else if (constraintSetValue.EmployeeField.Equals("Username"))
                {
                    isTrue = CheckIfComparsionIsValid(constraintSetValue, updatedEmployee.Username);
                }
            }

            else if (constraintCustom != null)// && constraint.CustomDataFieldID != null)
            {
                if (scheduleJob.JobCustomData.Count == 0)
                {
                    return true;
                }

                foreach (ScheduleCustomData scheduleCustomData in scheduleJob.JobCustomData)
                {
                    {
                        if (scheduleCustomData.CustomDataID == constraintCustom.CustomDataID)
                        {

                            if (scheduleCustomData.CustomDataLookupID != null)
                            {

                                if (constraintCustom.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }

                                if (constraintCustom.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, updatedEmployee.JobTitle, updatedEmployee.JobTitle);
                                }
                                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, updatedEmployee.JobTitle, updatedEmployee.JobTitle);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Location"))
                                {

                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.CustomDataLookupContent && constraintSetValue.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.CustomDataLookupContent && constraintSetValue.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump11;
                                            }
                                        }
                                    }
                                    if (constraintSetValue.Operator.Equals("="))
                                    {
                                        isTrue = false;
                                    }
                                    else
                                    {
                                        isTrue = true;
                                    }
                                jump11:;
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("mobile number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("pay rate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("telephone number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.CustomDataLookupContent, constraintCustom.Operator);
                                }
                            }
                            else if (scheduleCustomData.CustomDataLookupID == null)
                            {
                                if (constraintCustom.EmployeeField == null)
                                {
                                    isTrue = false;
                                    break;
                                }
                                if (constraintCustom.EmployeeField.ToString().Equals("Address 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address1, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address2, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address3, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Address 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Address4, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Class"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Class, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("cost code"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.CostCode, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Department"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Department, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Division"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Division, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Email"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Email, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("employee number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.EmployeeNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Firstname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Firstname, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Gender"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Gender, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("job title"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.JobTitle, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintSetValue.EmployeeField.ToLower().Equals("job type"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Jobtype, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Location"))
                                {
                                    if (updatedEmployee.Locations != null)
                                    {
                                        foreach (string location in updatedEmployee.Locations)
                                        {
                                            if ((location != scheduleCustomData.TextValue && constraintSetValue.Operator.Equals("=")) ||
                                                (location == scheduleCustomData.TextValue && constraintSetValue.Operator.Equals("<>")))
                                            {
                                                isTrue = true;
                                                goto jump12;
                                            }
                                        }
                                    }
                                    if (constraintSetValue.Operator.Equals("="))
                                    {
                                        isTrue = false;
                                    }
                                    else
                                    {
                                        isTrue = true;
                                    }
                                jump12:;
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Manager"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Manager, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 1"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours1), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 2"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours2), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 3"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours3), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 4"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours4), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Max Hours 5"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(Convert.ToString(updatedEmployee.MaxHours5), Convert.ToString(scheduleCustomData.NumberValue), constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("mobile number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.MobileNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Notification"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Notification, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Password"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Password, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("pay rate"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.PayRate, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Surname"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Surname, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Team"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Team, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToLower().ToString().Equals("telephone number"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.TelephoneNumber, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                                else if (constraintCustom.EmployeeField.ToString().Equals("Username"))
                                {
                                    isTrue = CheckIfComparsionIsValidCustom(updatedEmployee.Username, scheduleCustomData.TextValue, constraintCustom.Operator);
                                }
                            }
                        }
                    }
                }
            }
            return isTrue;
        }

        public static bool CheckIfComparsionIsValid(ConstraintSetValues setValue, string employeeField)
        {            
            bool isTrue = true;
            
            if (setValue.Operator.Equals(">") || setValue.Operator.Equals("<"))
            {
                bool isEmployeeFieldValueNumeric = float.TryParse(employeeField, out float i);
                bool isComparisonValueNumeric = float.TryParse(setValue.ComparisonValue, out float j);

                if (isEmployeeFieldValueNumeric && isComparisonValueNumeric)
                {
                    if(setValue.Operator.Equals(">"))
                    {
                        isTrue = i > j;                        
                    }
                    if (setValue.Operator.Equals("<"))
                    {
                        isTrue = i < j;
                    }                    
                }
            }
            else
            {
                if (((employeeField != setValue.ComparisonValue) && setValue.Operator.Equals("="))
                    || ((employeeField == setValue.ComparisonValue) && setValue.Operator.Equals("<>")))
                {
                    isTrue = false;
                }
            }

            return isTrue;
        }

        public static bool CheckIfComparsionIsValidCustom(string employeeFieldValue, string customDataValue, string comparsionOperator)
        {
            if (((employeeFieldValue != customDataValue) && comparsionOperator.Equals("=")) || 
                ((employeeFieldValue == customDataValue) && comparsionOperator.Equals("<>")))
            {
                return false;
            }
            else
                return true;
        }
    }
}
