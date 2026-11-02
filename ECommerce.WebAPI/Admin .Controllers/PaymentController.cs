using ECommerce.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{orderId}")]
        public async Task<IActionResult> CreatePayment(
            int orderId,
            [FromQuery] string paymentMethod = "Card")
        {
            try
            {
                var payment = await _paymentService
                    .CreatePaymentAsync(orderId, paymentMethod);

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}