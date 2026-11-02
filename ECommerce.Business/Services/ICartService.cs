using ECommerce.Entities.Entities;

namespace ECommerce.Business.Services
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> GetCartByUserIdAsync(int userId);

        Task AddToCartAsync(int userId, int productId, int quantity);

        Task UpdateQuantityAsync(int cartItemId, int quantity);

        Task RemoveFromCartAsync(int cartItemId);
    }
}
