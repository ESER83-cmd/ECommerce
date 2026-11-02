using ECommerce.Core.Repositories;
using ECommerce.Entities.Entities;

namespace ECommerce.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _unitOfWork.Repository<Product>().GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();

            return products
                .Where(x => x.Name.Contains(
                    searchTerm,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();

            return products
                .Where(x => x.CategoryId == categoryId)
                .ToList();
        }

        public async Task<List<Product>> FilterByPriceAsync(decimal minPrice, decimal maxPrice)
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();

            return products
                .Where(x => x.Price >= minPrice && x.Price <= maxPrice)
                .ToList();
        }
    }
}