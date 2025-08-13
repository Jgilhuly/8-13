using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly ILogger<PaymentGatewayController> _logger;
        public PaymentGatewayController(ILogger<PaymentGatewayController> logger)
        {
            _logger = logger;
        }

        [HttpPost("charge")]
        public IActionResult Charge([FromBody] PaymentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var txId = Guid.NewGuid().ToString();
            _logger.LogInformation("[PaymentGateway] Charge request for Order {OrderId} amount {Amount:C}. TxId {TxId}", request.OrderId, request.Amount, txId);

            return Ok(new { success = true, transactionId = txId });
        }
    }
} 