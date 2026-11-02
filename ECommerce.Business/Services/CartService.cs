
using ECommerce.Core.Repositories;
using ECommerce.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Business.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CartItem>> GetCartByUserIdAsync(int userId)
        {
            var cartItems = await _unitOfWork
                .Repository<CartItem>()
                .GetAllAsync();

            var products = await _unitOfWork
                .Repository<Product>()
                .GetAllAsync();

            var userCart = cartItems
                .Where(x => x.UserId == userId)
                .ToList();

            foreach (var item in userCart)
            {
                item.Product = products.FirstOrDefault(
                    p => p.Id == item.ProductId
                );
            }

            return userCart;
        }

        public async Task AddToCartAsync(
            int userId,
            int productId,
            int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero."
                );
            }

            var product = await _unitOfWork
                .Repository<Product>()
                .GetByIdAsync(productId);

            if (product == null)
            {
                throw new InvalidOperationException(
                    "Product not found."
                );
            }

            var cartItems = await _unitOfWork
                .Repository<CartItem>()
                .GetAllAsync();

            var existingItems = cartItems
                .Where(x =>
                    x.UserId == userId &&
                    x.ProductId == productId)
                .ToList();

            var currentQuantity = existingItems
                .Sum(x => x.Quantity);

            if (currentQuantity + quantity > product.Stock)
            {
                throw new InvalidOperationException(
                    "Insufficient stock."
                );
            }

            if (existingItems.Any())
            {
                var existingItem = existingItems.First();

                existingItem.Quantity = currentQuantity + quantity;

                _unitOfWork.Repository<CartItem>()
                    .Update(existingItem);

                foreach (var duplicate in existingItems.Skip(1))
                {
                    _unitOfWork.Repository<CartItem>()
                        .Delete(duplicate);
                }
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };

                await _unitOfWork.Repository<CartItem>()
                    .AddAsync(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(
            int cartItemId,
            int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero."
                );
            }

            var cartItem = await _unitOfWork
                .Repository<CartItem>()
                .GetByIdAsync(cartItemId);

            if (cartItem == null)
            {
                return;
            }

            var product = await _unitOfWork
                .Repository<Product>()
                .GetByIdAsync(cartItem.ProductId);

            if (product == null)
            {
                throw new InvalidOperationException(
                    "Product not found."
                );
            }

            if (quantity > product.Stock)
            {
                throw new InvalidOperationException(
                    "Insufficient stock."
                );
            }

            cartItem.Quantity = quantity;

            _unitOfWork.Repository<CartItem>()
                .Update(cartItem);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _unitOfWork
                .Repository<CartItem>()
                .GetByIdAsync(cartItemId);

            if (cartItem == null)
            {
                return;
            }

            _unitOfWork.Repository<CartItem>()
                .Delete(cartItem);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}