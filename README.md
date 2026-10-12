# ShopSphere: E-Commerce Backend (ASP.NET Core 8 + SQL Server)

A RESTful e-commerce backend built with **C#, .NET 8, ASP.NET Core Web API, Entity Framework Core and SQL Server**.
Customers can browse products, manage a cart and place orders. Admins manage products and order status.

## Features
- JWT authentication with role-based authorization (Admin / Customer), BCrypt password hashing
- Product catalog with search, category/price filters, sorting and pagination
- Shopping cart with stock validation
- Checkout in a single database transaction (stock deduction + order creation + cart clear)
- Order lifecycle: Pending, Paid, Shipped, Delivered, Cancelled (cancel restocks items)
- Admin endpoints for products and orders
- Global exception-handling middleware, Swagger UI with Bearer auth
- Code First database design with indexes, unique constraints and seed data

## Architecture
```
Controllers  ->  Services (business logic)  ->  Repositories (generic Repository pattern)  ->  EF Core / SQL Server
                       DTOs for API contracts, Dependency Injection throughout
```
Design patterns used: **Repository, Dependency Injection, Layered architecture, DTO, Middleware (chain of responsibility)**.

## Run locally
Prerequisites: .NET 8 SDK and SQL Server (LocalDB / Express / Docker).

```bash
cd ShopSphere.Api
dotnet restore
dotnet run
```
Open `https://localhost:<port>/swagger`.

Default admin: `admin@shopsphere.com` / `Admin@123` (change this and the JWT key in `appsettings.json`).

SQL Server in Docker (optional):
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_password123" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```
Connection string: `Server=localhost,1433;Database=ShopSphereDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True`

## Use EF Core migrations (recommended instead of EnsureCreated)
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```
Then replace `db.Database.EnsureCreated();` in `Program.cs` with `db.Database.Migrate();`.

## API overview
| Method | Endpoint | Access |
|---|---|---|
| POST | /api/auth/register, /api/auth/login | Public |
| GET | /api/products (search, filter, sort, page) | Public |
| GET | /api/products/{id} | Public |
| POST/PUT/DELETE | /api/products | Admin |
| GET/POST | /api/cart, DELETE /api/cart/{productId} | Customer |
| POST | /api/orders (checkout) | Customer |
| GET | /api/orders, /api/orders/{id} | Customer |
| POST | /api/orders/{id}/cancel | Customer |
| GET | /api/orders/all, PUT /api/orders/{id}/status | Admin |

## Next steps (to extend)
- Razorpay payment integration (hook marked in `OrderService.PlaceOrderAsync`)
- Unit tests (xUnit + Moq), FluentValidation, Serilog, Redis/in-memory caching
- Dockerfile + deploy to Azure App Service
- React / Blazor frontend
