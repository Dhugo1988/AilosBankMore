using Microsoft.AspNetCore.Mvc;

namespace APITransferencia.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Status = "Healthy", Service = "Transferencia", Timestamp = DateTime.UtcNow });
        }
    }
}
