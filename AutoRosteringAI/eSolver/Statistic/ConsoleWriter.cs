using System;
using System.Collections.Generic;
using eSolver.Entities;

namespace eSolver.Statistic
{
    static partial class ConsoleWriter
    {
        static public void ConsoleWriteSolution(int[,] matrix, List<ScheduleJob> jobs, List<Employee> employees, int solutionCount, int mistakeOfOutputMatrix)
        {
            Console.Write("        \t");
            for (int j = 0; j < jobs.Count; j++)
            {
                Console.Write(jobs[j].JobTypeID + " ");
            }
            int totalForEmployee = 0;
            int totalAssigments = 0;
            double totalHours, minHours, maxHOurs, minIndex, maxIndex;
            minIndex = 0;
            maxIndex = 0;
            minHours = 9999999;
            maxHOurs = 0;
            string totalHoursArray = "";
            for (int i = 0; i < employees.Count; i++)
            {
                Console.WriteLine();
                totalForEmployee = 0;
                totalHours = 0;
                Console.Write("Employee " + (i + 1) + "(" + employees[i].JobTypeID + "):\t");
                if (matrix != null)
                {
                    for (int j = 0; j < jobs.Count; j++)
                    {
                        totalForEmployee += matrix[i, j];
                        totalHours += matrix[i, j] != 0 ? jobs[j].HoursT.TotalHours : 0;
                        Console.Write(matrix[i, j] + " ");
                    }
                }
                Console.Write(" tot: " + totalForEmployee);
                Console.Write("  " + Math.Round((decimal)totalHours, 2) + "h");
                totalHoursArray += totalHours + " ";
                if (totalHours < minHours)
                {
                    minHours = totalHours;
                    minIndex = i;
                }
                else if (totalHours > maxHOurs)
                {
                    maxHOurs = totalHours;
                    maxIndex = i;
                }

                totalAssigments += totalForEmployee;
            }
            Console.WriteLine(totalHoursArray);
            Console.WriteLine();
            Console.Write("Sum \t\t");
            int mistake = 0;
            if (matrix != null)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    int sum = 0;
                    for (int e = 0; e < employees.Count; e++)
                    {
                        sum += matrix[e, j];
                    }
                    mistake += (int)Math.Pow((int)jobs[j].NoOfEmployeesRequired - sum, 2);
                    if (sum < 10) Console.Write(sum + "  ");
                    else Console.Write(sum + " ");
                }
            }
            Console.WriteLine();
            Console.WriteLine("this: " + mistake + " best: " + mistakeOfOutputMatrix);
            Console.WriteLine();
            Console.Write("Required \t\t");
            for (int j = 0; j < jobs.Count; j++)
            {
                if (jobs[j].NoOfEmployeesRequired < 10) Console.Write(jobs[j].NoOfEmployeesRequired + "  ");
                else Console.Write(jobs[j].NoOfEmployeesRequired + " ");
            }
            Console.WriteLine();
            Console.WriteLine("minIndex" + minIndex);
            Console.WriteLine("minHours" + minHours);
            Console.WriteLine("maxIndex" + maxIndex);
            Console.WriteLine("maxHOurs" + maxHOurs);
            Console.WriteLine();
            Console.WriteLine("TotalAssigments " + totalAssigments);
        }
    }
}
