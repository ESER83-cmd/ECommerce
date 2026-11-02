using ECommerce.Entities.Entities;

namespace ECommerce.Business.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<List<Product>> SearchProductsAsync(string searchTerm);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Product>> FilterByPriceAsync(decimal minPrice, decimal maxPrice);
    }
}
