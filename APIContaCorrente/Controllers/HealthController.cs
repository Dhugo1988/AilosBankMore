using Microsoft.AspNetCore.Mvc;

namespace APIContaCorrente.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Health check básico
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Status = "Healthy", Service = "Conta Corrente", Timestamp = DateTime.UtcNow });
        }
    }
}
