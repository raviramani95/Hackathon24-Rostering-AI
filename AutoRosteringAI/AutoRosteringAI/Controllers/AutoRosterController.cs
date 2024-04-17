using eSolver;
using eSolver.Entities.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AutoRosteringAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutoRosterController : Controller
    {                
        public AutoRosterController()
        {
            
        }

        [HttpGet("autosolve")]
        public IActionResult Get(int groupId)
        {
            var t = Task.Run(() => AutoRosterUtils.AutoSolve(groupId));
            t.Wait();
            return Ok(t.Result);
        }
    }
}
