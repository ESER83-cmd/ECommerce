namespace ECommerce.Entities.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = "Card";

        public string Status { get; set; } = "Pending";

        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}