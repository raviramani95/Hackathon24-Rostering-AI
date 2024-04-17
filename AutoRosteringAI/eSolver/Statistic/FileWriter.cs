using eSolver.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using eSolver.AutoSolverNew.RankingLogic;

namespace eSolver.Statistic
{
    // This class is used for the development purpose
    // It generates file that contains the solution and it's parametars
    // Commented 18/08/2022

    /*
    class FileWriter
    {
        

        //public static string filePath = @"./../../../Statistic/Statistic.txt";
        //public static string filePath = @"./Statistic/Statistic.txt";

        
        static public void FileClearText()
        {
            //try
            //{
            //    // Create the file, or overwrite if the file exists.
            //    using (StreamWriter file = File.CreateText(filePath))// (System.IO.StreamWriter file = new File (filePath)) 
            //    {
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
        }

        static public void FileWriteSolution(int[,] matrix, List<ScheduleJob> jobs, List<Employee> employees, int solutionCount, int mistakeOfOutputMatrix)
        {
            try
            {
                // Create the file, or overwrite if the file exists.
                using (StreamWriter file = File.AppendText(filePath))// (System.IO.StreamWriter file = new File (filePath)) 
                {
                    file.Write("        ");
                    for (int j = 0; j < jobs.Count; j++)
                    {
                        file.Write(jobs[j].JobTypeID + " ");
                    }
                    int totalForEmployee = 0;
                    int totalAssigments = 0;
                    double totalHours, minHours, maxHOurs, minIndex, maxIndex;
                    minIndex = 0;
                    maxIndex = 0;
                    minHours = 999999;
                    maxHOurs = 0;
                    string totalHoursArray = "";
                    for (int i = 0; i < employees.Count; i++)
                    {
                        file.WriteLine();
                        totalForEmployee = 0;
                        totalHours = 0;
                        file.Write("Employee " + (i + 1) + "(" + employees[i].JobTypeID + "):\t");
                        if (matrix != null)
                        {
                            for (int j = 0; j < jobs.Count; j++)
                            {
                                totalForEmployee += matrix[i, j];
                                totalHours += matrix[i, j] != 0 ? jobs[j].HoursT.TotalHours : 0;
                                file.Write(matrix[i, j] + " ");
                            }
                        }
                        file.Write(" tot: " + Math.Round((decimal) totalForEmployee, 2));
                        file.Write("  " + Math.Round(totalHours, 2) + "h");
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
                    file.WriteLine("\n" + totalHoursArray);
                    file.WriteLine("Sum \t\t\t\n");
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
                            if (sum < 10) file.Write(sum + "  ");
                            else file.Write(sum + " ");
                        }
                    }
                    file.WriteLine();
                    file.WriteLine("this: " + mistake + " best: " + mistakeOfOutputMatrix);
                    file.WriteLine();
                    file.WriteLine("Required \t\t");
                    for (int j = 0; j < jobs.Count; j++)
                    {
                        if (jobs[j].NoOfEmployeesRequired < 10) file.Write(jobs[j].NoOfEmployeesRequired + "  ");
                        else file.Write(jobs[j].NoOfEmployeesRequired + " ");
                    }
                    file.WriteLine();
                    file.WriteLine("minIndex" + minIndex);
                    file.WriteLine("minHours" + minHours);
                    file.WriteLine("maxIndex" + maxIndex);
                    file.WriteLine("maxHOurs" + maxHOurs);
                    file.WriteLine();
                    file.WriteLine("TotalAssigments " + totalAssigments);

                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        static public void FileWriteDynamicRankingParametars(List<RankingWrapper> rankingWrappers, int turnForRanking, int turnForCondition, long upperValue, long lowerValue)
        {
            try
            {
                // Create the file, or overwrite if the file exists.
                using (StreamWriter file = File.AppendText(filePath))// (System.IO.StreamWriter file = new File (filePath)) 
                {

                    file.WriteLine("        \t\n");
                    file.WriteLine("\t Ranking :" + rankingWrappers[turnForRanking].rankingName);
                    file.WriteLine("\t Period [" + turnForCondition + "]");
                    file.WriteLine("        \t");
                    if (rankingWrappers[turnForRanking].rankingName == "Hours")
                    {
                        file.WriteLine("Lower Value:\t" + lowerValue + "h");
                        file.WriteLine("Upper Value:\t" + upperValue + "h");
                        file.WriteLine("Lower Value:\t" + (decimal)lowerValue / (decimal)6000 + "h\t" + (decimal)lowerValue / (decimal)60 + "min");
                        file.WriteLine("Upper Value:\t" + (decimal)upperValue / (decimal)6000 + "h\t" + (decimal)upperValue / (decimal)60 + "min");
                    }
                    else
                    {
                        file.WriteLine("Lower Value:\t" + lowerValue);
                        file.WriteLine("Upper Value:\t" + upperValue);
                    }
                    file.WriteLine("        \t");

                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static public void FileWriteStaticRankingParametars()
        {

        }
    }
    */
}
