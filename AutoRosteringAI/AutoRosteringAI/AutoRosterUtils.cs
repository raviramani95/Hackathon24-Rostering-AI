using eSolver;
using eSolver.Entities;
using eSolver.Entities.Responses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace AutoRosteringAI
{
    public class AutoRosterUtils
    {
        public static async Task<SolversResponse> AutoSolve(int groupId)
        {
            string value = null;
            string pathToJson = null;

            switch (groupId)
            {
                case 1:
                    pathToJson = ".\\..\\eSolver\\Test\\AR.json";
                    break;
                case 2:
                    pathToJson = ".\\..\\eSolver\\Test\\AR2.json";
                    break;
                case 3:
                    pathToJson = ".\\..\\eSolver\\Test\\AR3.json";
                    break;
            }
            
            AutoSolve autoSolveTemp = new AutoSolve();
            string config = File.ReadAllText(pathToJson);
            SolversResponse autoSolveResult = await Task.Run(() => autoSolveTemp.Solve(config));
            Result jsonData = JsonConvert.DeserializeObject<Result>(config);


            string arData = GetData(groupId);
            AREmployeeJobData arEmployeeData = JsonConvert.DeserializeObject<AREmployeeJobData>(arData);


            if (autoSolveResult != null)
            {
                foreach(var i in autoSolveResult.FilledJob)
                {
                    int employeesRequired = jsonData.Schedule.ScheduleJobs.FirstOrDefault(x => x.Id == i.JobID).NoOfEmployeesRequired;
                    i.JobTypeName = arEmployeeData.ScheduleJobs.FirstOrDefault(x => x.Id == i.JobID).JobType[0].JobTypeName;
                    i.JobStartDateTime= arEmployeeData.ScheduleJobs.FirstOrDefault(x => x.Id == i.JobID).JobStartDateTime;
                    i.JobEndDateTime = arEmployeeData.ScheduleJobs.FirstOrDefault(x => x.Id == i.JobID).JobEndDateTime;
                    if (employeesRequired == i.EmployeesID.Count)
                        i.Color = "Green";
                    else if (employeesRequired > i.EmployeesID.Count)
                        i.Color = "Yellow";
                    else if (i.EmployeesID.Count == 0)
                        i.Color = "Red";
                    List<string> assingedEmployeesName = new List<string>();
                    foreach (var employee in i.EmployeesID)
                    {
                        
                        string firstname = jsonData.Employees.FirstOrDefault(x => x.Id == employee).Firstname;
                        string lastname = jsonData.Employees.FirstOrDefault(x => x.Id == employee).Surname;
                        assingedEmployeesName.Add(firstname + " " + lastname);
                    }
                    i.AssignedEmployees = assingedEmployeesName;

                }
            }


            return autoSolveResult;
        }

        public static string GetData(int groupId)
        {
            string pathToJson = null;
            switch (groupId)
            {
                case 1:
                    pathToJson = ".\\..\\eSolver\\ARData1.json";
                    break;
                case 2:
                    pathToJson = ".\\..\\eSolver\\ARData2.json";
                    break;
                case 3:
                    pathToJson = ".\\..\\eSolver\\ARData3.json";
                    break;
            }
            string data = File.ReadAllText(pathToJson);
            return data;
        }
    }
}
