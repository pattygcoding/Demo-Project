using GroceryApp.Models;
using GroceryApp.Models.DTOs;

namespace GroceryApp.Services;

public interface IGroceryService
{
    Task<IEnumerable<GroceryItemDto>> GetAllGroceriesAsync();
    Task<GroceryItemDto?> GetGroceryByIdAsync(int id);
    Task<GroceryItemDto> CreateGroceryAsync(CreateGroceryItemDto createDto);
    Task<GroceryItemDto?> UpdateGroceryAsync(int id, UpdateGroceryItemDto updateDto);
    Task<bool> DeleteGroceryAsync(int id);
    Task<bool> GroceryExistsAsync(int id);
    Task<GroceryItemDto?> AdjustStockAsync(int id, int adjustment);
}
