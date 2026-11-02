using ECommerce.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
            public async Task<IActionResult> GetCart(int userId)
            {
                var cartItems = await _cartService.GetCartByUserIdAsync(userId);

                return Ok(cartItems);
        }

        [HttpPost]
            public async Task<IActionResult> AddToCart(
    int userId,
    int productId,
    int quantity)
            {
                await _cartService.AddToCartAsync(userId, productId, quantity);

                return Ok();
        }

        [HttpPut("{cartItemId}")]
            public async Task<IActionResult> UpdateQuantity(
    int cartItemId,
    int quantity)
            {
                await _cartService.UpdateQuantityAsync(cartItemId, quantity);

                return Ok();
        }

        [HttpDelete("{cartItemId}")]
            public async Task<IActionResult> RemoveFromCart(int cartItemId)
            {
                await _cartService.RemoveFromCartAsync(cartItemId);

                return NoContent();
            }
        }
  }
     
 
    

