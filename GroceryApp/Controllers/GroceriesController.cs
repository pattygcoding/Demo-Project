using Microsoft.AspNetCore.Mvc;
using GroceryApp.Services;
using GroceryApp.Models.DTOs;

namespace GroceryApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroceriesController : ControllerBase
{
    private readonly IGroceryService _groceryService;

    public GroceriesController(IGroceryService groceryService)
    {
        _groceryService = groceryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroceryItemDto>>> GetGroceries()
    {
        var groceries = await _groceryService.GetAllGroceriesAsync();
        return Ok(groceries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GroceryItemDto>> GetGrocery(int id)
    {
        var grocery = await _groceryService.GetGroceryByIdAsync(id);
        if (grocery == null)
        {
            return NotFound();
        }
        return Ok(grocery);
    }

    [HttpPost]
    public async Task<ActionResult<GroceryItemDto>> CreateGrocery(CreateGroceryItemDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdGrocery = await _groceryService.CreateGroceryAsync(createDto);
        return CreatedAtAction(nameof(GetGrocery), new { id = createdGrocery.Id }, createdGrocery);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GroceryItemDto>> UpdateGrocery(int id, UpdateGroceryItemDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedGrocery = await _groceryService.UpdateGroceryAsync(id, updateDto);
        if (updatedGrocery == null)
        {
            return NotFound();
        }

        return Ok(updatedGrocery);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGrocery(int id)
    {
        var result = await _groceryService.DeleteGroceryAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}