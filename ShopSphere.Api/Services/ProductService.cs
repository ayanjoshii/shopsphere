using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Middleware;
using ShopSphere.Api.Models;
using ShopSphere.Api.Repositories;

namespace ShopSphere.Api.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> SearchAsync(ProductQuery q);
    Task<ProductDto> GetAsync(int id);
    Task<ProductDto> CreateAsync(ProductCreateDto dto);
    Task<ProductDto> UpdateAsync(int id, ProductCreateDto dto);
    Task DeleteAsync(int id);
}

public class ProductService : IProductService
{
    private readonly IRepository<Product> _products;
    private readonly IRepository<Category> _categories;

    public ProductService(IRepository<Product> products, IRepository<Category> categories)
    {
        _products = products;
        _categories = categories;
    }

    public async Task<PagedResult<ProductDto>> SearchAsync(ProductQuery q)
    {
        var page = Math.Max(1, q.Page);
        var pageSize = Math.Clamp(q.PageSize, 1, 50);

        var query = _products.Query().AsNoTracking().Include(p => p.Category).AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(q.Search))
            query = query.Where(p => p.Name.Contains(q.Search) || p.Description.Contains(q.Search));
        if (q.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == q.CategoryId);
        if (q.MinPrice.HasValue)
            query = query.Where(p => p.Price >= q.MinPrice);
        if (q.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= q.MaxPrice);

        // Sorting
        query = (q.SortBy?.ToLowerInvariant(), q.Descending) switch
        {
            ("price", false) => query.OrderBy(p => p.Price),
            ("price", true) => query.OrderByDescending(p => p.Price),
            ("name", true) => query.OrderByDescending(p => p.Name),
            ("name", false) => query.OrderBy(p => p.Name),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync();

        // Pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, p.ImageUrl, p.CategoryId, p.Category!.Name))
            .ToListAsync();

        return new PagedResult<ProductDto>(items, total, page, pageSize);
    }

    public async Task<ProductDto> GetAsync(int id)
    {
        var p = await _products.Query().AsNoTracking().Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new AppException("Product not found.", 404);
        return ToDto(p);
    }

    public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
    {
        await EnsureCategoryExists(dto.CategoryId);

        var p = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description ?? string.Empty,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId
        };
        await _products.AddAsync(p);
        await _products.SaveChangesAsync();
        return await GetAsync(p.Id);
    }

    public async Task<ProductDto> UpdateAsync(int id, ProductCreateDto dto)
    {
        var p = await _products.GetByIdAsync(id) ?? throw new AppException("Product not found.", 404);
        await EnsureCategoryExists(dto.CategoryId);

        p.Name = dto.Name.Trim();
        p.Description = dto.Description ?? string.Empty;
        p.Price = dto.Price;
        p.Stock = dto.Stock;
        p.ImageUrl = dto.ImageUrl;
        p.CategoryId = dto.CategoryId;

        await _products.SaveChangesAsync();
        return await GetAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var p = await _products.GetByIdAsync(id) ?? throw new AppException("Product not found.", 404);
        _products.Remove(p);
        await _products.SaveChangesAsync();
    }

    private async Task EnsureCategoryExists(int categoryId)
    {
        if (await _categories.GetByIdAsync(categoryId) is null)
            throw new AppException("Category does not exist.", 400);
    }

    private static ProductDto ToDto(Product p) =>
        new(p.Id, p.Name, p.Description, p.Price, p.Stock, p.ImageUrl, p.CategoryId, p.Category?.Name ?? "");
}
