using CornerStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using CornerStore.Models.DTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core and provides dummy value for testing
builder.Services.AddNpgsql<CornerStoreDbContext>(builder.Configuration["CornerStoreDbConnectionString"] ?? "testing");

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//endpoints go here

//Cashier Endpoint
app.MapPost("/api/cashiers", (CornerStoreDbContext db, Cashier cashier) =>
{
    db.Cashiers.Add(cashier);
    db.SaveChanges();
    return Results.Created($"/api/cashiers/{cashier.Id}", cashier);
});

app.MapGet("/api/cashiers/{id}", (CornerStoreDbContext db, int id) =>
{
    try
    {
        Cashier cashierToReturn = db.Cashiers
        .Include(c => c.Orders)
        .ThenInclude(o => o.OrderProducts)
        .ThenInclude(op => op.Product)
        .Where(c => c.Id == id)
        .Single();

        return Results.Ok(
            new CashierDTO
            {
                Id = cashierToReturn.Id,
                FirstName = cashierToReturn.FirstName,
                LastName = cashierToReturn.LastName,
                Orders = cashierToReturn.Orders.Select(o => new OrderDTO
                {
                    Id = o.Id,
                    CashierId = o.CashierId,
                    PaidOnDate = o.PaidOnDate,
                    OrderProducts = o.OrderProducts.Select(op => new OrderProductDTO
                    {
                        Id = op.Id,
                        OrderId = op.OrderId,
                        ProductId = op.ProductId,
                        Product = new ProductDTO
                        {
                            Id = op.Product.Id,
                            ProductName = op.Product.ProductName,
                            Price = op.Product.Price,
                            Brand = op.Product.Brand,
                            CategoryId = op.Product.CategoryId
                        },
                        Quantity = op.Quantity
                    }).ToList()
                }).ToList()
            }
        );
    }
    catch (SystemException)
    {
        return Results.NotFound();
    }
});

//Product Endpoint
app.MapGet("/api/products/", (CornerStoreDbContext db, string? Search) =>
{
    List<Product> productToReturn = db.Products
    .Include(p => p.Category)
    .ToList();

    if (Search != null)
    {
        productToReturn = productToReturn.Where(p => p.ProductName.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    if (productToReturn.Count == 0)
    {
        return Results.NotFound();
    }

    return Results.Ok(
        productToReturn.Select(p => new ProductDTO
        {
            Id = p.Id,
            ProductName = p.ProductName,
            Price = p.Price,
            Brand = p.Brand,
            CategoryId = p.CategoryId,
            Category = new CategoryDTO
            {
                Id = p.Category.Id,
                CategoryName = p.Category.CategoryName
            }
        })
    );

});

app.MapPost("/api/products", (CornerStoreDbContext db, Product product) =>
{
    db.Products.Add(product);
    db.SaveChanges();
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapPut("/api/products/{id}", (CornerStoreDbContext db, int id, Product product) =>
{
    Product productToUpdate = db.Products.SingleOrDefault(p => p.Id == id);
    if (productToUpdate == null)
    {
        return Results.BadRequest();
    }
    productToUpdate.ProductName = product.ProductName;
    productToUpdate.Price = product.Price;
    productToUpdate.Brand = product.Brand;
    productToUpdate.CategoryId = product.CategoryId;

    db.SaveChanges();
    return Results.NoContent();
});

//Orders Endpoint
app.MapGet("/api/orders/{id}", (int id, CornerStoreDbContext db) =>
{
    Order orderToReturn = db.Orders
    .Include(o => o.Cashier)
    .Include(o => o.OrderProducts)
    .ThenInclude(op => op.Product)
    .Where(o => o.Id == id)
    .SingleOrDefault();

    if (orderToReturn == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(
        new OrderDTO
        {
            Id = orderToReturn.Id,
            CashierId = orderToReturn.CashierId,
            Cashier = new CashierDTO
            {
                Id = orderToReturn.Cashier.Id,
                FirstName = orderToReturn.Cashier.FirstName,
                LastName = orderToReturn.Cashier.LastName,
            },
            OrderProducts = orderToReturn.OrderProducts.Select(op => new OrderProductDTO
            {
                Id = op.Id,
                ProductId = op.ProductId,
                Product = new ProductDTO
                {
                    Id = op.Product.Id,
                    ProductName = op.Product.ProductName,
                    Price = op.Product.Price,
                    Brand = op.Product.Brand,
                    CategoryId = op.Product.CategoryId
                },
                OrderId = op.OrderId,
                Quantity = op.Quantity
            }).ToList()
        }
    );

});

app.MapGet("/api/orders", (CornerStoreDbContext db, DateTime? orderDate) =>
{
    List<Order> ordersToReturn = db.Orders
    .Include(o => o.OrderProducts)
    .ThenInclude(op => op.Product)
    .ToList();

    if (orderDate != null)
    {
        ordersToReturn = ordersToReturn.Where(o => o.PaidOnDate == orderDate).ToList();
    }

    if (ordersToReturn.Count == 0)
    {
        return Results.NotFound();
    }

    return Results.Ok(

    ordersToReturn.Select(o => new OrderDTO
    {
        Id = o.Id,
        CashierId = o.CashierId,
        PaidOnDate = o.PaidOnDate,
        OrderProducts = o.OrderProducts.Select(op => new OrderProductDTO
        {
            Id = op.Id,
            ProductId = op.ProductId,
            Product = new ProductDTO
            {
                Id = op.Product.Id,
                ProductName = op.Product.ProductName,
                Price = op.Product.Price,
                Brand = op.Product.Brand,
                CategoryId = op.Product.CategoryId
            },
            OrderId = op.OrderId,
            Quantity = op.Quantity
        }).ToList()
    })
    );

});

app.MapDelete("/api/orders/{id}", (int id, CornerStoreDbContext db) =>
{
    Order orderToDelete = db.Orders.SingleOrDefault(o => o.Id == id);
    if (orderToDelete == null)
    {
        return Results.BadRequest();
    }

    db.Orders.Remove(orderToDelete);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPost("/api/orders", (CornerStoreDbContext db, Order order) =>
{
    db.Orders.Add(order);
    db.SaveChanges();
    return Results.Created($"/api/orders/{order.Id}", order);
});

app.Run();

//don't move or change this!
public partial class Program { }