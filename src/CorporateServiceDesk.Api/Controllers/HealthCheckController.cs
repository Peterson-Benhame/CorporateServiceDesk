using Microsoft.AspNetCore.Mvc;

namespace CorporateServiceDesk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Test()
        {
            return NoContent();
        }
    }
}
