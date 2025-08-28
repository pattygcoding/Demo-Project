using Microsoft.EntityFrameworkCore;
using GroceryApp.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace GroceryApp.Data;

/// <summary>
/// Entity Framework Core database context for the Grocery application.
/// Manages database connections, entity configurations, and provides access to grocery items data.
/// </summary>
public class GroceryDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the GroceryDbContext class.
    /// </summary>
    /// <param name="options">The database context options including connection string configuration.</param>
    public GroceryDbContext(DbContextOptions<GroceryDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet for grocery items in the database.
    /// Provides access to all CRUD operations on grocery items through Entity Framework Core.
    /// </summary>
    public DbSet<GroceryItem> GroceryItems { get; set; } = null!;

    /// <summary>
    /// Configures the database model and entity relationships using the model builder.
    /// Defines entity configurations, constraints, and seeds initial data into the database.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to construct the database model.</param>
    /// <remarks>
    /// Configures the GroceryItem entity with:
    /// - Primary key on Id property
    /// - Required Name field with 100 character limit
    /// - Required Category field with enum conversion
    /// - Required Price and CostToProduce fields with 18,2 decimal precision
    /// - Required Stock and CreatedUtc fields
    /// Also seeds the database with initial grocery items from embedded JSON data.
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GroceryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasConversion<string>();
            entity.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.CostToProduce).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Stock).IsRequired();
            entity.Property(e => e.CreatedUtc).IsRequired();
        });

        // Seed data
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Seeds the database with initial grocery item data from an embedded JSON file.
    /// Loads seed data from SeedData.json and creates grocery items for initial database population.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to seed entity data.</param>
    /// <remarks>
    /// Reads seed data from the embedded resource file "GroceryApp.Data.SeedData.json".
    /// Each seed item is converted to a GroceryItem entity with auto-generated IDs and UTC timestamps.
    /// Only processes items with valid category enum values to ensure data integrity.
    /// </remarks>
    private void SeedData(ModelBuilder modelBuilder)
    {
        var groceryItems = new List<GroceryItem>();
        var id = 1;

        // Read seed data from JSON file
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "GroceryApp.Data.SeedData.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var seedItems = JsonConvert.DeserializeObject<List<SeedGroceryItem>>(json);

            if (seedItems != null)
            {
                foreach (var seedItem in seedItems)
                {
                    if (Enum.TryParse<Category>(seedItem.Category, out var category))
                    {
                        groceryItems.Add(new GroceryItem
                        {
                            Id = id++,
                            Name = seedItem.Name,
                            Category = category,
                            Price = seedItem.Price,
                            CostToProduce = seedItem.CostToProduce,
                            Stock = seedItem.Stock,
                            CreatedUtc = DateTime.UtcNow
                        });
                    }
                }
            }
        }

        // Fallback to hardcoded data if JSON reading fails
        if (!groceryItems.Any())
        {
            // Original hardcoded seeding as fallback
            var fruits = new[]
            {
                new { Name = "Apple", Price = 0.99m, CostToProduce = 0.45m },
                new { Name = "Banana", Price = 0.59m, CostToProduce = 0.25m },
                new { Name = "Orange", Price = 0.89m, CostToProduce = 0.40m },
                new { Name = "Strawberry", Price = 2.99m, CostToProduce = 1.50m },
                new { Name = "Grapes", Price = 3.49m, CostToProduce = 1.75m },
                new { Name = "Pineapple", Price = 2.99m, CostToProduce = 1.20m },
                new { Name = "Mango", Price = 1.99m, CostToProduce = 0.95m },
                new { Name = "Kiwi", Price = 0.79m, CostToProduce = 0.35m },
                new { Name = "Blueberry", Price = 4.99m, CostToProduce = 2.50m },
                new { Name = "Peach", Price = 1.49m, CostToProduce = 0.70m }
            };

            foreach (var fruit in fruits)
            {
                groceryItems.Add(new GroceryItem
                {
                    Id = id++,
                    Name = fruit.Name,
                    Category = Category.Fruit,
                    Price = fruit.Price,
                    CostToProduce = fruit.CostToProduce,
                    Stock = 10,
                    CreatedUtc = DateTime.UtcNow
                });
            }

            var vegetables = new[]
            {
                new { Name = "Carrot", Price = 0.89m, CostToProduce = 0.30m },
                new { Name = "Broccoli", Price = 1.99m, CostToProduce = 0.80m },
                new { Name = "Spinach", Price = 2.49m, CostToProduce = 1.00m },
                new { Name = "Tomato", Price = 1.29m, CostToProduce = 0.60m },
                new { Name = "Cucumber", Price = 0.99m, CostToProduce = 0.40m },
                new { Name = "Bell Pepper", Price = 1.79m, CostToProduce = 0.85m },
                new { Name = "Onion", Price = 0.79m, CostToProduce = 0.25m },
                new { Name = "Garlic", Price = 0.99m, CostToProduce = 0.40m },
                new { Name = "Potato", Price = 0.69m, CostToProduce = 0.20m },
                new { Name = "Lettuce", Price = 1.49m, CostToProduce = 0.60m }
            };

            foreach (var vegetable in vegetables)
            {
                groceryItems.Add(new GroceryItem
                {
                    Id = id++,
                    Name = vegetable.Name,
                    Category = Category.Vegetable,
                    Price = vegetable.Price,
                    CostToProduce = vegetable.CostToProduce,
                    Stock = 10,
                    CreatedUtc = DateTime.UtcNow
                });
            }

            var meats = new[]
            {
                new { Name = "Chicken Breast", Price = 5.99m, CostToProduce = 3.50m },
                new { Name = "Ground Beef", Price = 4.99m, CostToProduce = 2.75m },
                new { Name = "Salmon Fillet", Price = 8.99m, CostToProduce = 5.50m },
                new { Name = "Pork Chops", Price = 6.49m, CostToProduce = 3.25m },
                new { Name = "Turkey Breast", Price = 7.99m, CostToProduce = 4.50m },
                new { Name = "Beef Steak", Price = 12.99m, CostToProduce = 8.00m },
                new { Name = "Ground Turkey", Price = 5.49m, CostToProduce = 3.00m },
                new { Name = "Bacon", Price = 4.99m, CostToProduce = 2.50m },
                new { Name = "Ham", Price = 6.99m, CostToProduce = 3.75m },
                new { Name = "Lamb Chops", Price = 15.99m, CostToProduce = 10.00m }
            };

            foreach (var meat in meats)
            {
                groceryItems.Add(new GroceryItem
                {
                    Id = id++,
                    Name = meat.Name,
                    Category = Category.Meat,
                    Price = meat.Price,
                    CostToProduce = meat.CostToProduce,
                    Stock = 10,
                    CreatedUtc = DateTime.UtcNow
                });
            }

            var cheeses = new[]
            {
                new { Name = "Cheddar Cheese", Price = 3.99m, CostToProduce = 2.00m },
                new { Name = "Mozzarella Cheese", Price = 3.49m, CostToProduce = 1.75m },
                new { Name = "Swiss Cheese", Price = 4.99m, CostToProduce = 2.50m },
                new { Name = "Brie Cheese", Price = 5.99m, CostToProduce = 3.50m },
                new { Name = "Gouda Cheese", Price = 4.49m, CostToProduce = 2.25m },
                new { Name = "Parmesan Cheese", Price = 6.99m, CostToProduce = 4.00m },
                new { Name = "Blue Cheese", Price = 5.49m, CostToProduce = 3.25m },
                new { Name = "Feta Cheese", Price = 3.99m, CostToProduce = 2.00m },
                new { Name = "Cream Cheese", Price = 2.99m, CostToProduce = 1.50m },
                new { Name = "Cottage Cheese", Price = 2.49m, CostToProduce = 1.25m }
            };

            foreach (var cheese in cheeses)
            {
                groceryItems.Add(new GroceryItem
                {
                    Id = id++,
                    Name = cheese.Name,
                    Category = Category.Cheese,
                    Price = cheese.Price,
                    CostToProduce = cheese.CostToProduce,
                    Stock = 10,
                    CreatedUtc = DateTime.UtcNow
                });
            }

            var breads = new[]
            {
                new { Name = "White Bread", Price = 2.49m, CostToProduce = 0.75m },
                new { Name = "Whole Wheat Bread", Price = 2.99m, CostToProduce = 1.00m },
                new { Name = "Sourdough Bread", Price = 3.99m, CostToProduce = 1.50m },
                new { Name = "Rye Bread", Price = 3.49m, CostToProduce = 1.25m },
                new { Name = "Baguette", Price = 2.99m, CostToProduce = 1.00m },
                new { Name = "Ciabatta", Price = 3.49m, CostToProduce = 1.25m },
                new { Name = "Pita Bread", Price = 2.99m, CostToProduce = 1.00m },
                new { Name = "Bagels", Price = 3.99m, CostToProduce = 1.50m },
                new { Name = "Croissant", Price = 1.99m, CostToProduce = 0.75m },
                new { Name = "Dinner Rolls", Price = 2.99m, CostToProduce = 1.00m }
            };

            foreach (var bread in breads)
            {
                groceryItems.Add(new GroceryItem
                {
                    Id = id++,
                    Name = bread.Name,
                    Category = Category.Bread,
                    Price = bread.Price,
                    CostToProduce = bread.CostToProduce,
                    Stock = 10,
                    CreatedUtc = DateTime.UtcNow
                });
            }
        }

        modelBuilder.Entity<GroceryItem>().HasData(groceryItems);
    }
}
