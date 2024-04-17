using eSolver;
using eSolver.Entities.Responses;
using Newtonsoft.Json;

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

            //string json = JsonConvert.SerializeObject(value, Formatting.None,
            //            new JsonSerializerSettings()
            //            {
            //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //            });
            AutoSolve autoSolveTemp = new AutoSolve();
            string config = File.ReadAllText(pathToJson);
            SolversResponse autoSolveResult = await Task.Run(() => autoSolveTemp.Solve(config));
            return autoSolveResult;
        }
    }
}
