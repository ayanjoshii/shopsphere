using System.ComponentModel.DataAnnotations;

namespace ShopSphere.Api.DTOs;

// ---------- Auth ----------
public record RegisterDto(
    [Required, StringLength(100)] string Name,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponseDto(string Token, string Name, string Email, string Role);

// ---------- Products ----------
public record ProductDto(int Id, string Name, string Description, decimal Price, int Stock, string? ImageUrl, int CategoryId, string CategoryName);

public record ProductCreateDto(
    [Required, StringLength(200)] string Name,
    [StringLength(2000)] string Description,
    [Range(0.01, 10000000)] decimal Price,
    [Range(0, 100000)] int Stock,
    string? ImageUrl,
    [Range(1, int.MaxValue)] int CategoryId);

public class ProductQuery
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; }      // name, price, newest
    public bool Descending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

// ---------- Cart ----------
public record AddToCartDto(
    [Range(1, int.MaxValue)] int ProductId,
    [Range(1, 100)] int Quantity);

public record CartItemDto(int ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
public record CartDto(IEnumerable<CartItemDto> Items, decimal Total);

// ---------- Orders ----------
public record PlaceOrderDto([Required, StringLength(500)] string ShippingAddress);
public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice);
public record OrderDto(int Id, DateTime OrderDate, string Status, decimal TotalAmount, string ShippingAddress, IEnumerable<OrderItemDto> Items);
public record UpdateOrderStatusDto([Required] string Status);
