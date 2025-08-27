using Microsoft.EntityFrameworkCore;
using GroceryApp.Data;
using GroceryApp.Data.Repositories;
using GroceryApp.Services;
using GroceryApp.Models;
using GroceryApp.Models.DTOs;

namespace GroceryApp.Tests;

public class GroceryServiceTests : IDisposable
{
    private readonly GroceryDbContext _context;
    private readonly IGroceryRepository _repository;
    private readonly IGroceryService _service;

    public GroceryServiceTests()
    {
        var options = new DbContextOptionsBuilder<GroceryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GroceryDbContext(options);
        _repository = new GroceryRepository(_context);
        _service = new GroceryService(_repository);
    }

    [Fact]
    public async Task GetAllGroceriesAsync_ReturnsAllItems()
    {
        // Arrange
        var testItems = new[]
        {
            new GroceryItem { Id = 1, Name = "Apple", Category = Category.Fruit, Price = 1.99m, Stock = 10 },
            new GroceryItem { Id = 2, Name = "Carrot", Category = Category.Vegetable, Price = 0.99m, Stock = 5 }
        };

        await _context.GroceryItems.AddRangeAsync(testItems);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllGroceriesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, g => g.Name == "Apple");
        Assert.Contains(result, g => g.Name == "Carrot");
    }

    [Fact]
    public async Task GetGroceryByIdAsync_ExistingId_ReturnsItem()
    {
        // Arrange
        var testItem = new GroceryItem 
        { 
            Id = 1, 
            Name = "Apple", 
            Category = Category.Fruit, 
            Price = 1.99m, 
            Stock = 10,
            CreatedUtc = DateTime.UtcNow
        };

        await _context.GroceryItems.AddAsync(testItem);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetGroceryByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Apple", result.Name);
        Assert.Equal(Category.Fruit, result.Category);
        Assert.Equal(1.99m, result.Price);
        Assert.Equal(10, result.Stock);
    }

    [Fact]
    public async Task GetGroceryByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _service.GetGroceryByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateGroceryAsync_ValidItem_CreatesAndReturnsItem()
    {
        // Arrange
        var createDto = new CreateGroceryItemDto
        {
            Name = "Test Apple",
            Category = Category.Fruit,
            Price = 2.99m,
            Stock = 15
        };

        // Act
        var result = await _service.CreateGroceryAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test Apple", result.Name);
        Assert.Equal(Category.Fruit, result.Category);
        Assert.Equal(2.99m, result.Price);
        Assert.Equal(15, result.Stock);

        // Verify item was created in database
        var dbItem = await _context.GroceryItems.FindAsync(result.Id);
        Assert.NotNull(dbItem);
        Assert.Equal("Test Apple", dbItem.Name);
    }

    [Fact]
    public async Task UpdateGroceryAsync_ExistingItem_UpdatesAndReturnsItem()
    {
        // Arrange
        var originalItem = new GroceryItem 
        { 
            Id = 1, 
            Name = "Original Apple", 
            Category = Category.Fruit, 
            Price = 1.99m, 
            Stock = 10,
            CreatedUtc = DateTime.UtcNow
        };

        await _context.GroceryItems.AddAsync(originalItem);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateGroceryItemDto
        {
            Name = "Updated Apple",
            Category = Category.Fruit,
            Price = 2.49m,
            Stock = 8
        };

        // Act
        var result = await _service.UpdateGroceryAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Apple", result.Name);
        Assert.Equal(2.49m, result.Price);
        Assert.Equal(8, result.Stock);

        // Verify item was updated in database
        var dbItem = await _context.GroceryItems.FindAsync(1);
        Assert.NotNull(dbItem);
        Assert.Equal("Updated Apple", dbItem.Name);
        Assert.Equal(2.49m, dbItem.Price);
    }

    [Fact]
    public async Task UpdateGroceryAsync_NonExistingItem_ReturnsNull()
    {
        // Arrange
        var updateDto = new UpdateGroceryItemDto
        {
            Name = "Test Item",
            Category = Category.Fruit,
            Price = 1.99m,
            Stock = 10
        };

        // Act
        var result = await _service.UpdateGroceryAsync(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteGroceryAsync_ExistingItem_ReturnsTrue()
    {
        // Arrange
        var testItem = new GroceryItem 
        { 
            Id = 1, 
            Name = "Apple", 
            Category = Category.Fruit, 
            Price = 1.99m, 
            Stock = 10,
            CreatedUtc = DateTime.UtcNow
        };

        await _context.GroceryItems.AddAsync(testItem);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteGroceryAsync(1);

        // Assert
        Assert.True(result);

        // Verify item was deleted from database
        var dbItem = await _context.GroceryItems.FindAsync(1);
        Assert.Null(dbItem);
    }

    [Fact]
    public async Task DeleteGroceryAsync_NonExistingItem_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteGroceryAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GroceryExistsAsync_ExistingItem_ReturnsTrue()
    {
        // Arrange
        var testItem = new GroceryItem 
        { 
            Id = 1, 
            Name = "Apple", 
            Category = Category.Fruit, 
            Price = 1.99m, 
            Stock = 10,
            CreatedUtc = DateTime.UtcNow
        };

        await _context.GroceryItems.AddAsync(testItem);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GroceryExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GroceryExistsAsync_NonExistingItem_ReturnsFalse()
    {
        // Act
        var result = await _service.GroceryExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}