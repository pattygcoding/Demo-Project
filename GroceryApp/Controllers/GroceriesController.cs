using Microsoft.AspNetCore.Mvc;
using GroceryApp.Services;
using GroceryApp.Models.DTOs;

namespace GroceryApp.Controllers;

/// <summary>
/// API controller that provides endpoints for managing grocery items.
/// Handles HTTP requests for CRUD operations on grocery items and returns appropriate HTTP responses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GroceriesController : ControllerBase
{
    private readonly IGroceryService _groceryService;

    /// <summary>
    /// Initializes a new instance of the GroceriesController class.
    /// </summary>
    /// <param name="groceryService">The service for handling grocery item business logic.</param>
    public GroceriesController(IGroceryService groceryService)
    {
        _groceryService = groceryService;
    }

    /// <summary>
    /// Retrieves all grocery items.
    /// </summary>
    /// <returns>An HTTP 200 OK response containing a collection of grocery item DTOs.</returns>
    /// <response code="200">Returns the list of all grocery items ordered by category and name.</response>
    /// <remarks>
    /// GET /api/groceries
    /// 
    /// Returns all grocery items in the system, ordered alphabetically by category and then by name.
    /// The response contains DTOs that include all item details including pricing and stock information.
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroceryItemDto>>> GetGroceries()
    {
        var groceries = await _groceryService.GetAllGroceriesAsync();
        return Ok(groceries);
    }

    /// <summary>
    /// Retrieves a specific grocery item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the grocery item to retrieve.</param>
    /// <returns>An HTTP 200 OK response containing the grocery item DTO, or HTTP 404 Not Found if the item doesn't exist.</returns>
    /// <response code="200">Returns the grocery item with the specified ID.</response>
    /// <response code="404">The grocery item with the specified ID was not found.</response>
    /// <remarks>
    /// GET /api/groceries/{id}
    /// 
    /// Example: GET /api/groceries/1
    /// </remarks>
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

    /// <summary>
    /// Creates a new grocery item.
    /// </summary>
    /// <param name="createDto">The creation data transfer object containing the grocery item information.</param>
    /// <returns>An HTTP 201 Created response containing the created grocery item DTO, or HTTP 400 Bad Request if validation fails.</returns>
    /// <response code="201">The grocery item was successfully created.</response>
    /// <response code="400">The request data is invalid or model validation failed.</response>
    /// <remarks>
    /// POST /api/groceries
    /// 
    /// Creates a new grocery item with the provided information. The CreatedUtc timestamp is automatically set.
    /// Returns the created item with its assigned ID and a Location header pointing to the new resource.
    /// </remarks>
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

    /// <summary>
    /// Updates an existing grocery item.
    /// </summary>
    /// <param name="id">The unique identifier of the grocery item to update.</param>
    /// <param name="updateDto">The update data transfer object containing the modified grocery item information.</param>
    /// <returns>An HTTP 200 OK response containing the updated grocery item DTO, HTTP 404 Not Found if the item doesn't exist, or HTTP 400 Bad Request if validation fails.</returns>
    /// <response code="200">The grocery item was successfully updated.</response>
    /// <response code="400">The request data is invalid or model validation failed.</response>
    /// <response code="404">The grocery item with the specified ID was not found.</response>
    /// <remarks>
    /// PUT /api/groceries/{id}
    /// 
    /// Example: PUT /api/groceries/1
    /// 
    /// Updates all fields of the grocery item including Name, Category, Price, CostToProduce, and Stock.
    /// </remarks>
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

    /// <summary>
    /// Deletes a grocery item.
    /// </summary>
    /// <param name="id">The unique identifier of the grocery item to delete.</param>
    /// <returns>An HTTP 204 No Content response if successful, or HTTP 404 Not Found if the item doesn't exist.</returns>
    /// <response code="204">The grocery item was successfully deleted.</response>
    /// <response code="404">The grocery item with the specified ID was not found.</response>
    /// <remarks>
    /// DELETE /api/groceries/{id}
    /// 
    /// Example: DELETE /api/groceries/1
    /// 
    /// Permanently deletes the grocery item from the system. This operation cannot be undone.
    /// </remarks>
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