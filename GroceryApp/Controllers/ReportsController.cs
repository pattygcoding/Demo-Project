using Microsoft.AspNetCore.Mvc;
using GroceryApp.Services;

namespace GroceryApp.Controllers;

/// <summary>
/// API controller that provides endpoints for generating and exporting various reports.
/// Handles HTTP requests for report generation including Excel exports with comprehensive analytics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IGroceryService _groceryService;
    private readonly IExcelExportService _excelExportService;

    /// <summary>
    /// Initializes a new instance of the ReportsController class.
    /// </summary>
    /// <param name="groceryService">The service for retrieving grocery item data.</param>
    /// <param name="excelExportService">The service for generating Excel reports.</param>
    public ReportsController(IGroceryService groceryService, IExcelExportService excelExportService)
    {
        _groceryService = groceryService;
        _excelExportService = excelExportService;
    }

    /// <summary>
    /// Generates and exports a comprehensive Excel report containing grocery item analytics.
    /// </summary>
    /// <returns>A file download response containing the generated Excel report, or HTTP 400 Bad Request if generation fails.</returns>
    /// <response code="200">Returns the Excel file as a download with appropriate content headers.</response>
    /// <response code="400">Report generation failed due to an error.</response>
    /// <remarks>
    /// GET /api/reports/export-excel
    /// 
    /// Generates a comprehensive Excel report with 5 worksheets:
    /// 1. Key Metrics - Overall business metrics and totals
    /// 2. Profit by Category - Profit analysis grouped by category
    /// 3. Top Profitable Items - Highest profit margin items
    /// 4. Stock Analysis - Current stock levels and availability
    /// 5. Price Range Analysis - Distribution of items by price ranges
    /// 
    /// The file is automatically named with a timestamp: GroceryReport_YYYYMMDD_HHMMSS.xlsx
    /// Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
    /// </remarks>
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