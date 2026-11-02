using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Entities.Entities;

namespace ECommerce.Business.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int userId);

        Task<List<Order>> GetUserOrdersAsync(int userId);

        Task UpdateOrderStatusAsync(int orderId, string status);

        Task<List<Order>> GetAllOrdersAsync();
    }
}

