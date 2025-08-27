using Microsoft.AspNetCore.Mvc;
using GroceryApp.Services;

namespace GroceryApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IGroceryService _groceryService;
    private readonly IExcelExportService _excelExportService;

    public ReportsController(IGroceryService groceryService, IExcelExportService excelExportService)
    {
        _groceryService = groceryService;
        _excelExportService = excelExportService;
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportExcel()
    {
        try
        {
            var groceries = await _groceryService.GetAllGroceriesAsync();
            var excelData = await _excelExportService.GenerateReportExcelAsync(groceries);

            var fileName = $"GroceryReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(excelData, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating Excel report: {ex.Message}");
        }
    }
}