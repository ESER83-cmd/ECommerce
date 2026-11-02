using ECommerce.Entities.Entities;

namespace ECommerce.Business.Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(int orderId, string paymentMethod);
    }
}