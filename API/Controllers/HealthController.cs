using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [ProducesDefaultResponseType(typeof(object))]
        public IActionResult Get()
        {
            return Ok(new { status = "Healthy", timestamp = DateTime.Now });
        }
    }
}