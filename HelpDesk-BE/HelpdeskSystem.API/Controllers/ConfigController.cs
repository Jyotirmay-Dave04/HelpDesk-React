using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ConfigController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet("encryptionKey")]
        public IActionResult GetEncryptionKey()
        {
            string? key = _config["Encryption:Key"];
            return Ok(new { key });
        }
    }
}
