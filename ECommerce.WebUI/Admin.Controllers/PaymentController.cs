using Microsoft.AspNetCore.Mvc;

namespace VOVOS.WebUI.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _httpClient;

        public PaymentController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ECommerceAPI");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            System.Diagnostics.Debug.WriteLine(
    "PAYMENT USER ID: " + (userId?.ToString() ?? "NULL"));

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _httpClient.PostAsync(
                $"api/order/{userId.Value}",
                null
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

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}