using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Data;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Middleware;
using ShopSphere.Api.Models;

namespace ShopSphere.Api.Services;

public interface IOrderService
{
    Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderDto dto);
    Task<List<OrderDto>> GetMyOrdersAsync(int userId);
    Task<OrderDto> GetOrderAsync(int userId, int orderId, bool isAdmin);
    Task<OrderDto> CancelAsync(int userId, int orderId);
    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto> UpdateStatusAsync(int orderId, string status);
}

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db) => _db = db;

    // Checkout: validates stock, creates order, reduces stock, clears cart (all in one transaction)
    public async Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderDto dto)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var cartItems = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cartItems.Count == 0)
            throw new AppException("Your cart is empty.", 400);

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = dto.ShippingAddress,
            Status = OrderStatus.Pending
        };

        foreach (var c in cartItems)
        {
            var product = c.Product!;
            if (product.Stock < c.Quantity)
                throw new AppException($"'{product.Name}' has only {product.Stock} unit(s) left.", 400);

            product.Stock -= c.Quantity;
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = c.Quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cartItems);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        // NOTE: Payment (Razorpay) hooks in here: create a Razorpay order for order.TotalAmount,
        // then mark the order as Paid after verifying the payment signature.
        return await GetOrderAsync(userId, order.Id, isAdmin: true);
    }

    public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
    {
        var orders = await BaseQuery().Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate).ToListAsync();
        return orders.Select(ToDto).ToList();
    }

    public async Task<OrderDto> GetOrderAsync(int userId, int orderId, bool isAdmin)
    {
        var order = await BaseQuery().FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new AppException("Order not found.", 404);

        if (!isAdmin && order.UserId != userId)
            throw new AppException("Order not found.", 404);

        return ToDto(order);
    }

    public async Task<OrderDto> CancelAsync(int userId, int orderId)
    {
        var order = await _db.Orders.Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
            ?? throw new AppException("Order not found.", 404);

        if (order.Status is not (OrderStatus.Pending or OrderStatus.Paid))
            throw new AppException("This order can no longer be cancelled.", 400);

        order.Status = OrderStatus.Cancelled;
        foreach (var item in order.Items)
            item.Product!.Stock += item.Quantity; // restock

        await _db.SaveChangesAsync();
        return await GetOrderAsync(userId, orderId, isAdmin: true);
    }

    // ----- Admin -----
    public async Task<List<OrderDto>> GetAllAsync()
    {
        var orders = await BaseQuery().OrderByDescending(o => o.OrderDate).ToListAsync();
        return orders.Select(ToDto).ToList();
    }

    public async Task<OrderDto> UpdateStatusAsync(int orderId, string status)
    {
        if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
            throw new AppException("Invalid status. Use: Pending, Paid, Shipped, Delivered, Cancelled.", 400);

        var order = await _db.Orders.FindAsync(orderId)
            ?? throw new AppException("Order not found.", 404);

        order.Status = newStatus;
        await _db.SaveChangesAsync();
        return await GetOrderAsync(order.UserId, orderId, isAdmin: true);
    }

    private IQueryable<Order> BaseQuery() =>
        _db.Orders.AsNoTracking().Include(o => o.Items).ThenInclude(i => i.Product);

    private static OrderDto ToDto(Order o) => new(
        o.Id, o.OrderDate, o.Status.ToString(), o.TotalAmount, o.ShippingAddress,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.Product?.Name ?? "", i.Quantity, i.UnitPrice)));
}
