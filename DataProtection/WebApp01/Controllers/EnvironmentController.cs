using Microsoft.AspNetCore.Mvc;

namespace DataProtectionTests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnvironmentController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEnvironmentVariables()
        {
            throw new System.NotImplementedException();
        }
    }
}
