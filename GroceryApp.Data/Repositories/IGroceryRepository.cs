using GroceryApp.Models;

namespace GroceryApp.Data.Repositories;

public interface IGroceryRepository
{
    Task<IEnumerable<GroceryItem>> GetAllAsync();
    Task<GroceryItem?> GetByIdAsync(int id);
    Task<GroceryItem> CreateAsync(GroceryItem item);
    Task<GroceryItem?> UpdateAsync(int id, GroceryItem item);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}