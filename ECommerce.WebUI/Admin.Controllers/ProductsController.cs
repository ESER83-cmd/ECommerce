using ECommerce.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductsController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory
                .CreateClient("ECommerceAPI");
        }

        public async Task<IActionResult> Index(string? category)
        {
            var products = await _httpClient
                .GetFromJsonAsync<List<ProductViewModel>>(
                    "api/products");

            products ??= new List<ProductViewModel>();

            if (!string.IsNullOrWhiteSpace(category))
            {
                products = products
                    .Where(p =>
                        !string.IsNullOrWhiteSpace(p.Name) &&
                        p.Name.Contains(
                            category,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.SelectedCategory = category;

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var products = await _httpClient
                .GetFromJsonAsync<List<ProductViewModel>>(
                    "api/products");

            var product = products?
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}