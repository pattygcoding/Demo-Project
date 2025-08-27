using ClosedXML.Excel;
using GroceryApp.Models.DTOs;
using GroceryApp.Models;

namespace GroceryApp.Services;

public interface IExcelExportService
{
    Task<byte[]> GenerateReportExcelAsync(IEnumerable<GroceryItemDto> groceries);
}

public class ExcelExportService : IExcelExportService
{
    public async Task<byte[]> GenerateReportExcelAsync(IEnumerable<GroceryItemDto> groceries)
    {
        using var workbook = new XLWorkbook();
        var groceryList = groceries.ToList();

        // Sheet 1: Key Metrics Summary
        CreateKeyMetricsSheet(workbook, groceryList);

        // Sheet 2: Profit by Category
        CreateProfitByCategorySheet(workbook, groceryList);

        // Sheet 3: Top Profitable Items
        CreateTopProfitableItemsSheet(workbook, groceryList);

        // Sheet 4: Stock Analysis
        CreateStockAnalysisSheet(workbook, groceryList);

        // Sheet 5: Price Range Analysis
        CreatePriceRangeAnalysisSheet(workbook, groceryList);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return await Task.FromResult(stream.ToArray());
    }

    private void CreateKeyMetricsSheet(XLWorkbook workbook, List<GroceryItemDto> groceries)
    {
        var worksheet = workbook.Worksheets.Add("Key Metrics");
        
        // Title
        worksheet.Cell(1, 1).Value = "Grocery Reports - Key Metrics Summary";
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Range(1, 1, 1, 3).Merge();
        
        // Calculate metrics (same as in Reports.razor)
        var totalProfitPotential = groceries.Sum(g => (g.Price - g.CostToProduce) * g.Stock);
        var averageProfitPerItem = groceries.Average(g => g.Price - g.CostToProduce);
        var totalItemsInStock = groceries.Sum(g => g.Stock);
        var totalCategories = groceries.Select(g => g.Category).Distinct().Count();

        // Key metrics with formatting
        worksheet.Cell(3, 1).Value = "Total Profit Potential:";
        worksheet.Cell(3, 1).Style.Font.Bold = true;
        worksheet.Cell(3, 2).Value = totalProfitPotential;
        worksheet.Cell(3, 2).Style.NumberFormat.Format = "$#,##0.00";
        worksheet.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.LightGreen;

        worksheet.Cell(4, 1).Value = "Average Profit per Item:";
        worksheet.Cell(4, 1).Style.Font.Bold = true;
        worksheet.Cell(4, 2).Value = averageProfitPerItem;
        worksheet.Cell(4, 2).Style.NumberFormat.Format = "$#,##0.00";
        worksheet.Cell(4, 2).Style.Fill.BackgroundColor = XLColor.LightBlue;

        worksheet.Cell(5, 1).Value = "Total Items in Stock:";
        worksheet.Cell(5, 1).Style.Font.Bold = true;
        worksheet.Cell(5, 2).Value = totalItemsInStock;
        worksheet.Cell(5, 2).Style.Fill.BackgroundColor = XLColor.LightYellow;

        worksheet.Cell(6, 1).Value = "Product Categories:";
        worksheet.Cell(6, 1).Style.Font.Bold = true;
        worksheet.Cell(6, 2).Value = totalCategories;
        worksheet.Cell(6, 2).Style.Fill.BackgroundColor = XLColor.LightCyan;

        // Note about data source
        worksheet.Cell(8, 1).Value = "Note: These values are calculated from the detailed data in the other sheets.";
        worksheet.Cell(8, 1).Style.Font.Italic = true;
        worksheet.Range(8, 1, 8, 3).Merge();

        worksheet.Columns().AdjustToContents();
    }

    private void CreateProfitByCategorySheet(XLWorkbook workbook, List<GroceryItemDto> groceries)
    {
        var worksheet = workbook.Worksheets.Add("Profit by Category");
        
        // Calculate category stats (same logic as in Reports.razor)
        var categoryStats = groceries
            .GroupBy(g => g.Category)
            .Select(g => new
            {
                Category = g.Key,
                ItemCount = g.Count(),
                TotalStock = g.Sum(i => i.Stock),
                AverageProfit = g.Average(i => i.Price - i.CostToProduce),
                TotalPotentialProfit = g.Sum(i => (i.Price - i.CostToProduce) * i.Stock)
            })
            .OrderByDescending(c => c.TotalPotentialProfit)
            .ToList();

        // Headers
        worksheet.Cell(1, 1).Value = "Category";
        worksheet.Cell(1, 2).Value = "Items";
        worksheet.Cell(1, 3).Value = "Total Stock";
        worksheet.Cell(1, 4).Value = "Avg Profit";
        worksheet.Cell(1, 5).Value = "Total Potential";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

        // Data
        for (int i = 0; i < categoryStats.Count; i++)
        {
            var category = categoryStats[i];
            var row = i + 2;

            worksheet.Cell(row, 1).Value = category.Category.ToString();
            worksheet.Cell(row, 2).Value = category.ItemCount;
            worksheet.Cell(row, 3).Value = category.TotalStock;
            worksheet.Cell(row, 4).Value = category.AverageProfit;
            worksheet.Cell(row, 5).Value = category.TotalPotentialProfit;
        }

        // Format currency columns
        worksheet.Range(2, 4, categoryStats.Count + 1, 5).Style.NumberFormat.Format = "$#,##0.00";

        worksheet.Columns().AdjustToContents();
    }

    private void CreateTopProfitableItemsSheet(XLWorkbook workbook, List<GroceryItemDto> groceries)
    {
        var worksheet = workbook.Worksheets.Add("Top Profitable Items");
        
        // Get top profitable items (same logic as in Reports.razor)
        var topProfitableItems = groceries
            .OrderByDescending(g => g.Price - g.CostToProduce)
            .Take(10)
            .ToList();

        // Headers
        worksheet.Cell(1, 1).Value = "Item";
        worksheet.Cell(1, 2).Value = "Category";
        worksheet.Cell(1, 3).Value = "Profit";
        worksheet.Cell(1, 4).Value = "Margin %";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

        // Data
        for (int i = 0; i < topProfitableItems.Count; i++)
        {
            var item = topProfitableItems[i];
            var row = i + 2;
            var profit = item.Price - item.CostToProduce;
            var marginPercent = (profit / item.Price);

            worksheet.Cell(row, 1).Value = item.Name;
            worksheet.Cell(row, 2).Value = item.Category.ToString();
            worksheet.Cell(row, 3).Value = profit;
            worksheet.Cell(row, 4).Value = marginPercent;
        }

        // Format currency and percentage columns
        worksheet.Range(2, 3, topProfitableItems.Count + 1, 3).Style.NumberFormat.Format = "$#,##0.00";
        worksheet.Range(2, 4, topProfitableItems.Count + 1, 4).Style.NumberFormat.Format = "0.0%";

        worksheet.Columns().AdjustToContents();
    }

    private void CreateStockAnalysisSheet(XLWorkbook workbook, List<GroceryItemDto> groceries)
    {
        var worksheet = workbook.Worksheets.Add("Stock Analysis");
        
        // Stock analysis (same logic as in Reports.razor)
        var highStockItems = groceries.Where(g => g.Stock > 5).ToList();
        var lowStockItems = groceries.Where(g => g.Stock >= 1 && g.Stock <= 5).ToList();
        var outOfStockItems = groceries.Where(g => g.Stock == 0).ToList();

        // Headers
        worksheet.Cell(1, 1).Value = "Status";
        worksheet.Cell(1, 2).Value = "Items";
        worksheet.Cell(1, 3).Value = "Total Stock";
        worksheet.Cell(1, 4).Value = "Value";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightCoral;

        // Data
        var stockData = new[]
        {
            new { Status = "High Stock (>5)", Items = highStockItems, Color = XLColor.LightGreen },
            new { Status = "Low Stock (1-5)", Items = lowStockItems, Color = XLColor.LightYellow },
            new { Status = "Out of Stock (0)", Items = outOfStockItems, Color = XLColor.LightPink }
        };

        for (int i = 0; i < stockData.Length; i++)
        {
            var level = stockData[i];
            var row = i + 2;

            worksheet.Cell(row, 1).Value = level.Status;
            worksheet.Cell(row, 2).Value = level.Items.Count;
            worksheet.Cell(row, 3).Value = level.Items.Sum(g => g.Stock);
            worksheet.Cell(row, 4).Value = level.Items.Sum(g => g.Stock * g.Price);

            // Apply row color
            var rowRange = worksheet.Range(row, 1, row, 4);
            rowRange.Style.Fill.BackgroundColor = level.Color;
        }

        // Format currency columns
        worksheet.Range(2, 4, stockData.Length + 1, 4).Style.NumberFormat.Format = "$#,##0.00";

        worksheet.Columns().AdjustToContents();
    }

    private void CreatePriceRangeAnalysisSheet(XLWorkbook workbook, List<GroceryItemDto> groceries)
    {
        var worksheet = workbook.Worksheets.Add("Price Range Analysis");
        
        // Price range analysis (same logic as in Reports.razor)
        var priceRanges = new[]
        {
            new { Range = "$0-$1", Items = groceries.Where(g => g.Price < 1m) },
            new { Range = "$1-$3", Items = groceries.Where(g => g.Price >= 1m && g.Price < 3m) },
            new { Range = "$3-$5", Items = groceries.Where(g => g.Price >= 3m && g.Price < 5m) },
            new { Range = "$5-$10", Items = groceries.Where(g => g.Price >= 5m && g.Price < 10m) },
            new { Range = "$10+", Items = groceries.Where(g => g.Price >= 10m) }
        }.Where(r => r.Items.Any()).ToList();

        // Headers
        worksheet.Cell(1, 1).Value = "Price Range";
        worksheet.Cell(1, 2).Value = "Items";
        worksheet.Cell(1, 3).Value = "Avg Cost";
        worksheet.Cell(1, 4).Value = "Avg Profit";
        worksheet.Cell(1, 5).Value = "Profit Margin";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightYellow;

        // Data
        for (int i = 0; i < priceRanges.Count; i++)
        {
            var range = priceRanges[i];
            var row = i + 2;
            var items = range.Items.ToList();

            worksheet.Cell(row, 1).Value = range.Range;
            worksheet.Cell(row, 2).Value = items.Count;
            worksheet.Cell(row, 3).Value = items.Average(g => g.CostToProduce);
            worksheet.Cell(row, 4).Value = items.Average(g => g.Price - g.CostToProduce);
            worksheet.Cell(row, 5).Value = items.Average(g => (g.Price - g.CostToProduce) / g.Price);
        }

        // Format currency and percentage columns
        worksheet.Range(2, 3, priceRanges.Count + 1, 4).Style.NumberFormat.Format = "$#,##0.00";
        worksheet.Range(2, 5, priceRanges.Count + 1, 5).Style.NumberFormat.Format = "0.0%";

        worksheet.Columns().AdjustToContents();
    }
}