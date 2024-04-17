using System;
using System.Collections.Generic;
using eSolver.Entities;
using Google.OrTools.Sat;

namespace eSolver.BusinessLogic.ConstraintsLogic
{
    public static class Const_EmployeeLeaverStatus
    {
        public static void ApplyEmployeeLeaverStatus(ILiteral[,] matrix, CpModel model, List<Employee> employees, List<ScheduleJob> jobs, List<ScheduleJob> jobsNonSorted)
        {
            for (int emp = 0; emp < employees.Count; emp++)
            {
                if (employees[emp].LeaveDate != null)
                {
                    for (int j = 0; j < jobs.Count; j++)
                    {
                        if (DateTime.Compare((DateTime)employees[emp].LeaveDate, (DateTime)jobs[j].JobEndDate) < 0)
                        {
                            model.Add((IntVar)matrix[emp, j] == 0);
                        }
                    }
                }
            }
        }
    }
}