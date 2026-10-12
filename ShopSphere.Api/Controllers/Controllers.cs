using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSphere.Api.DTOs;
using ShopSphere.Api.Services;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto) =>
        Ok(await _auth.RegisterAsync(dto));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto) =>
        Ok(await _auth.LoginAsync(dto));
}

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;
    public ProductsController(IProductService products) => _products = products;

    // GET api/products?search=mouse&categoryId=1&minPrice=100&sortBy=price&page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> Search([FromQuery] ProductQuery query) =>
        Ok(await _products.SearchAsync(query));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> Get(int id) => Ok(await _products.GetAsync(id));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(ProductCreateDto dto)
    {
        var created = await _products.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, ProductCreateDto dto) =>
        Ok(await _products.UpdateAsync(id, dto));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _products.DeleteAsync(id);
        return NoContent();
    }
}

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cart;
    public CartController(ICartService cart) => _cart = cart;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<CartDto>> Get() => Ok(await _cart.GetCartAsync(UserId));

    [HttpPost]
    public async Task<ActionResult<CartDto>> Add(AddToCartDto dto) => Ok(await _cart.AddAsync(UserId, dto));

    [HttpDelete("{productId:int}")]
    public async Task<ActionResult<CartDto>> Remove(int productId) => Ok(await _cart.RemoveAsync(UserId, productId));
}

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    public OrdersController(IOrderService orders) => _orders = orders;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Place(PlaceOrderDto dto) =>
        Ok(await _orders.PlaceOrderAsync(UserId, dto));

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> Mine() => Ok(await _orders.GetMyOrdersAsync(UserId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> Get(int id) =>
        Ok(await _orders.GetOrderAsync(UserId, id, User.IsInRole("Admin")));

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<OrderDto>> Cancel(int id) => Ok(await _orders.CancelAsync(UserId, id));

    // ----- Admin -----
    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<List<OrderDto>>> All() => Ok(await _orders.GetAllAsync());

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(int id, UpdateOrderStatusDto dto) =>
        Ok(await _orders.UpdateStatusAsync(id, dto.Status));
}
