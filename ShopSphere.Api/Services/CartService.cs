using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Middleware;
using ShopSphere.Api.Models;
using ShopSphere.Api.Repositories;

namespace ShopSphere.Api.Services;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId);
    Task<CartDto> AddAsync(int userId, AddToCartDto dto);
    Task<CartDto> RemoveAsync(int userId, int productId);
}

public class CartService : ICartService
{
    private readonly IRepository<CartItem> _cart;
    private readonly IRepository<Product> _products;

    public CartService(IRepository<CartItem> cart, IRepository<Product> products)
    {
        _cart = cart;
        _products = products;
    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var items = await _cart.Query().AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new CartItemDto(c.ProductId, c.Product!.Name, c.Product.Price, c.Quantity, c.Product.Price * c.Quantity))
            .ToListAsync();

        return new CartDto(items, items.Sum(i => i.LineTotal));
    }

    public async Task<CartDto> AddAsync(int userId, AddToCartDto dto)
    {
        var product = await _products.GetByIdAsync(dto.ProductId)
            ?? throw new AppException("Product not found.", 404);

        var existing = await _cart.Query()
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        var newQty = (existing?.Quantity ?? 0) + dto.Quantity;
        if (newQty > product.Stock)
            throw new AppException($"Only {product.Stock} unit(s) available in stock.", 400);

        if (existing is null)
            await _cart.AddAsync(new CartItem { UserId = userId, ProductId = dto.ProductId, Quantity = dto.Quantity });
        else
            existing.Quantity = newQty;

        await _cart.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task<CartDto> RemoveAsync(int userId, int productId)
    {
        var item = await _cart.Query()
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId)
            ?? throw new AppException("Item not in cart.", 404);

        _cart.Remove(item);
        await _cart.SaveChangesAsync();
        return await GetCartAsync(userId);
    }
}
