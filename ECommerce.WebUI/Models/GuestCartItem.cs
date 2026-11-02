namespace ECommerce.WebUI.Models
{
    public class GuestCartItem
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; } = 1;

        public string? ImageUrl { get; set; }
    }
}