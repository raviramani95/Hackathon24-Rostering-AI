using eSolver.AutoSolverNew;
using eSolver.Entities.Responses;
using eSolver.Statistic;
using System;
using System.IO;

namespace eSolver
{
    class MainTest
    {
        static void Main()
        {
            // For the development purpose

            /*
            bool test = false;
            bool generateJson = true;

            string pathToJson;

            if (test)
            {
                int[] numberOfjobs = new int[] { 10, 20, 50, 200};
                int[] numberOfemployees = new int[] { 10, 20, 50, 200 };

                string jsonFileName = "Test1.json"; //"testMNOJTC.json";

                if (generateJson)
                {
                    JsonGenerator generator = new JsonGenerator();
                    generator.Generate(jsonFileName);
                }

                pathToJson = ".\\..\\..\\..\\..\\..\\\\ESolve\\eSolver\\Test\\GeneratedJsons\\" + jsonFileName;
            }
            else 
            {
                pathToJson = ".\\..\\..\\..\\..\\..\\ESolve\\eSolver\\Test\\Jsons\\Test.json";
            }

            AutoSolve autoSolve = new AutoSolve();

            string config = File.ReadAllText(pathToJson);
            SolversResponse solution = autoSolve.Solve(config);

            Console.ReadKey();
            */
        }
    }
}