using Microsoft.EntityFrameworkCore;
using GroceryApp.Models;

namespace GroceryApp.Data.Repositories;

public class GroceryRepository : IGroceryRepository
{
    private readonly GroceryDbContext _context;

    public GroceryRepository(GroceryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GroceryItem>> GetAllAsync()
    {
        return await _context.GroceryItems.OrderBy(g => g.Category).ThenBy(g => g.Name).ToListAsync();
    }

    public async Task<GroceryItem?> GetByIdAsync(int id)
    {
        return await _context.GroceryItems.FindAsync(id);
    }

    public async Task<GroceryItem> CreateAsync(GroceryItem item)
    {
        item.CreatedUtc = DateTime.UtcNow;
        _context.GroceryItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<GroceryItem?> UpdateAsync(int id, GroceryItem item)
    {
        var existingItem = await _context.GroceryItems.FindAsync(id);
        if (existingItem == null)
            return null;

        existingItem.Name = item.Name;
        existingItem.Category = item.Category;
        existingItem.Price = item.Price;
        existingItem.CostToProduce = item.CostToProduce;
        existingItem.Stock = item.Stock;

        await _context.SaveChangesAsync();
        return existingItem;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.GroceryItems.FindAsync(id);
        if (item == null)
            return false;

        _context.GroceryItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.GroceryItems.AnyAsync(g => g.Id == id);
    }
}