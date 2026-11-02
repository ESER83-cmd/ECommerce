using System.Net.Http.Json;
using System.Text.Json;
using ECommerce.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebUI.Controllers
{
    public class CartController : Controller
    {
        private const string GuestCartKey = "GuestCart";
        private readonly HttpClient _httpClient;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ECommerceAPI");
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                var guestCart = GetGuestCart();

                ViewBag.CartJson = JsonSerializer.Serialize(
                    guestCart.Select(item => new
                    {
                        id = item.ProductId,
                        product = new
                        {
                            id = item.ProductId,
                            name = item.Name,
                            price = item.Price
                        },
                        quantity = item.Quantity
                    })
                );

                ViewBag.IsGuest = true;

                return View();
            }

            var response = await _httpClient.GetAsync(
                $"api/cart/{userId.Value}"
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage =
                    await response.Content.ReadAsStringAsync();

                return StatusCode(
                    (int)response.StatusCode,
                    errorMessage
                );
            }

            ViewBag.CartJson =
                await response.Content.ReadAsStringAsync();

            ViewBag.IsGuest = false;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            if (productId <= 0)
            {
                return BadRequest("Invalid product.");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                var productResponse = await _httpClient.GetAsync(
                    $"api/products/{productId}"
                );

                if (!productResponse.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)productResponse.StatusCode,
                        "Product could not be loaded."
                    );
                }

                var product = await productResponse.Content
                    .ReadFromJsonAsync<GuestProductDto>(
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (product == null)
                {
                    return BadRequest("Product not found.");
                }

                var cart = GetGuestCart();

                var existingItem = cart.FirstOrDefault(
                    x => x.ProductId == productId
                );

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cart.Add(new GuestCartItem
                    {
                        ProductId = productId,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = 1
                    });
                }

                SaveGuestCart(cart);

                return RedirectToAction("Index", "Products");
            }

            var response = await _httpClient.PostAsync(
                $"api/cart?userId={userId.Value}" +
                $"&productId={productId}&quantity=1",
                null
            );

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(
                    (int)response.StatusCode,
                    await response.Content.ReadAsStringAsync()
                );
            }

            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(
            int cartItemId,
            int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Invalid quantity.");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                var cart = GetGuestCart();

                var item = cart.FirstOrDefault(
                    x => x.ProductId == cartItemId
                );

                if (item == null)
                {
                    return NotFound("Cart item not found.");
                }

                item.Quantity = quantity;

                SaveGuestCart(cart);

                return RedirectToAction("Index");
            }

            var response = await _httpClient.PutAsync(
                $"api/cart/{cartItemId}?quantity={quantity}",
                null
            );

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(
                    (int)response.StatusCode,
                    await response.Content.ReadAsStringAsync()
                );
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            if (cartItemId <= 0)
            {
                return BadRequest("Invalid cart item.");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                var cart = GetGuestCart();

                cart.RemoveAll(
                    x => x.ProductId == cartItemId
                );

                SaveGuestCart(cart);

                return RedirectToAction("Index");
            }

            var response = await _httpClient.DeleteAsync(
                $"api/cart/{cartItemId}"
            );

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(
                    (int)response.StatusCode,
                    await response.Content.ReadAsStringAsync()
                );
            }

            return RedirectToAction("Index");
        }

        private List<GuestCartItem> GetGuestCart()
        {
            var json = HttpContext.Session.GetString(GuestCartKey);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<GuestCartItem>();
            }

            return JsonSerializer.Deserialize<List<GuestCartItem>>(json)
                   ?? new List<GuestCartItem>();
        }

        private void SaveGuestCart(List<GuestCartItem> cart)
        {
            HttpContext.Session.SetString(
                GuestCartKey,
                JsonSerializer.Serialize(cart)
            );
        }

        private sealed class GuestProductDto
        {
            public string Name { get; set; } = string.Empty;

            public decimal Price { get; set; }
        }
    }
}