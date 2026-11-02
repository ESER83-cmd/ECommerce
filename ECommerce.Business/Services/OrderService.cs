using ECommerce.DataAccess.Context;
using ECommerce.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(x => x.UserId == userId)
                .Include(x => x.Product)
                .ToListAsync();

            if (cartItems.Count == 0)
                throw new Exception("Cart is empty.");

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Product == null)
                    throw new Exception("Product not found.");

                if (cartItem.Product.Stock < cartItem.Quantity)
                    throw new Exception(
                        $"Not enough stock for {cartItem.Product.Name}. Available stock: {cartItem.Product.Stock}"
                    );
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = cartItems.Sum(x => x.Product!.Price * x.Quantity)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product!.Price
                };

                _context.OrderItems.Add(orderItem);

                cartItem.Product.Stock -= cartItem.Quantity;
            }

            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Where(x => x.UserId == userId)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var allowedStatuses = new[]
            {
                "Pending",
                "Paid",
                "Shipped",
                "Cancelled"
            };

            if (!allowedStatuses.Contains(status))
                throw new Exception("Invalid order status.");

            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status == "Cancelled")
                throw new Exception("Cancelled order status cannot be changed.");

            if (status == "Cancelled")
            {
                foreach (var orderItem in order.OrderItems)
                {
                    if (orderItem.Product != null)
                    {
                        orderItem.Product.Stock += orderItem.Quantity;
                    }
                }
            }

            order.Status = status;

            await _context.SaveChangesAsync();
        }
    }
}