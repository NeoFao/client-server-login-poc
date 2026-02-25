using Microsoft.AspNetCore.Mvc;

namespace ClientServerLoginPoC.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "OK" });
    }

}
