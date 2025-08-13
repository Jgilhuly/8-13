using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RestaurantOps.Legacy.Controllers
{
    [ApiController]
    [Route("api/kitchen")]
    public class KitchenDisplayController : ControllerBase
    {
        private readonly ILogger<KitchenDisplayController> _logger;
        public KitchenDisplayController(ILogger<KitchenDisplayController> logger)
        {
            _logger = logger;
        }

        // Receives order details when submitted to kitchen
        [HttpPost("order")]
        public IActionResult ReceiveOrder([FromBody] object payload)
        {
            _logger.LogInformation("[KitchenDisplay] Order received: {Payload}", payload);
            return Ok(new { status = "received" });
        }
    }
} 