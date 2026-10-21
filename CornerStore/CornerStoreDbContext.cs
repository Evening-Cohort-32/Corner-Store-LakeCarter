using Microsoft.EntityFrameworkCore;
using CornerStore.Models;
public class CornerStoreDbContext : DbContext
{
    public DbSet<Cashier> Cashiers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<Product> Products { get; set; }

    public CornerStoreDbContext(DbContextOptions<CornerStoreDbContext> context) : base(context)
    {

    }

    //allows us to configure the schema when migrating as well as seed data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Seed Cashiers
        modelBuilder.Entity<Cashier>().HasData(new Cashier[]
        {
           new Cashier {Id = 1, FirstName = "Lake", LastName = "Carter"},
           new Cashier {Id = 2, FirstName = "Bob", LastName = "Joe"},
           new Cashier {Id = 3, FirstName = "Gran", LastName = "Mof"}
        });

        //Seed Category
        modelBuilder.Entity<Category>().HasData(new Category[]
        {
            new Category {Id = 1, CategoryName="Food"},
            new Category {Id = 2, CategoryName="Drink"},
            new Category {Id = 3, CategoryName="Cleaning Supplies"},
            new Category {Id = 4, CategoryName="Hygiene"},
            new Category {Id = 5, CategoryName="Home"}
        });

        //Seed Order
        modelBuilder.Entity<Order>().HasData(new Order[]
        {
           new Order {Id = 1, CashierId = 1, PaidOnDate= new DateTime(2026,2,23)},
           new Order {Id = 2, CashierId = 2, PaidOnDate= new DateTime(2026,4,12)},
           new Order {Id = 3, CashierId = 3, PaidOnDate= new DateTime(2026,5,11)}
        });

        //Seed Product
        modelBuilder.Entity<Product>().HasData(new Product[]
        {
           new Product {Id = 1, ProductName="Chips", Price=1.25m, Brand="Lays", CategoryId=1},
           new Product {Id = 2, ProductName="Soda", Price=1m, Brand="Sprite", CategoryId=2},
           new Product {Id = 3, ProductName="Detergent", Price=7.99m, Brand="Tide", CategoryId=3},
           new Product {Id = 4, ProductName="Shampoo", Price=3.99m, Brand="Old Spice", CategoryId=4},
           new Product {Id = 5, ProductName="Lamp", Price=15m, Brand="Rampoo", CategoryId=5},
        });

        // Seed OrderProduct
        modelBuilder.Entity<OrderProduct>().HasData(new OrderProduct[]
        {
            new OrderProduct {Id = 1, OrderId= 1, ProductId=1, Quantity=2},
            new OrderProduct {Id = 2, OrderId= 2, ProductId=2, Quantity=1},
            new OrderProduct {Id = 3, OrderId= 3, ProductId=4, Quantity=2},
        });
    }
}