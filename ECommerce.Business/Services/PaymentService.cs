using ECommerce.DataAccess.Context;
using ECommerce.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(int orderId, string paymentMethod)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (existingPayment != null)
                throw new Exception("This order has already been paid.");

            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.TotalPrice,
                PaymentMethod = paymentMethod,
                Status = "Completed",
                PaymentDate = DateTime.Now
            };

            _context.Payments.Add(payment);

            order.Status = "Paid";

            await _context.SaveChangesAsync();

            return payment;
        }
    
    }
}