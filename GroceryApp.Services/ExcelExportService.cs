using OfficeOpenXml;
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
        using var package = new ExcelPackage();
        var groceryList = groceries.ToList();

        // Sheet 1: Key Metrics Summary
        var summarySheet = package.Workbook.Worksheets.Add("Key Metrics");
        await CreateKeyMetricsSheet(summarySheet, groceryList);

        // Sheet 2: Profit by Category
        var categorySheet = package.Workbook.Worksheets.Add("Profit by Category");
        await CreateProfitByCategorySheet(categorySheet, groceryList);

        // Sheet 3: Top Profitable Items
        var profitableSheet = package.Workbook.Worksheets.Add("Top Profitable Items");
        await CreateTopProfitableItemsSheet(profitableSheet, groceryList);

        // Sheet 4: Stock Analysis
        var stockSheet = package.Workbook.Worksheets.Add("Stock Analysis");
        await CreateStockAnalysisSheet(stockSheet, groceryList);

        // Sheet 5: Price Range Analysis
        var priceSheet = package.Workbook.Worksheets.Add("Price Range Analysis");
        await CreatePriceRangeAnalysisSheet(priceSheet, groceryList);

        return package.GetAsByteArray();
    }

    private async Task CreateKeyMetricsSheet(ExcelWorksheet worksheet, List<GroceryItemDto> groceries)
    {
        // Title
        worksheet.Cells[1, 1].Value = "Grocery Reports - Key Metrics Summary";
        worksheet.Cells[1, 1].Style.Font.Size = 16;
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        
        // Calculate metrics (same as in Reports.razor)
        var totalProfitPotential = groceries.Sum(g => (g.Price - g.CostToProduce) * g.Stock);
        var averageProfitPerItem = groceries.Average(g => g.Price - g.CostToProduce);
        var totalItemsInStock = groceries.Sum(g => g.Stock);
        var totalCategories = groceries.Select(g => g.Category).Distinct().Count();

        // Key metrics
        worksheet.Cells[3, 1].Value = "Total Profit Potential:";
        worksheet.Cells[3, 1].Style.Font.Bold = true;
        worksheet.Cells[3, 2].Value = totalProfitPotential;
        worksheet.Cells[3, 2].Style.Numberformat.Format = "$#,##0.00";
        worksheet.Cells[3, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[3, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

        worksheet.Cells[4, 1].Value = "Average Profit per Item:";
        worksheet.Cells[4, 1].Style.Font.Bold = true;
        worksheet.Cells[4, 2].Value = averageProfitPerItem;
        worksheet.Cells[4, 2].Style.Numberformat.Format = "$#,##0.00";
        worksheet.Cells[4, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[4, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

        worksheet.Cells[5, 1].Value = "Total Items in Stock:";
        worksheet.Cells[5, 1].Style.Font.Bold = true;
        worksheet.Cells[5, 2].Value = totalItemsInStock;
        worksheet.Cells[5, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[5, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

        worksheet.Cells[6, 1].Value = "Product Categories:";
        worksheet.Cells[6, 1].Style.Font.Bold = true;
        worksheet.Cells[6, 2].Value = totalCategories;
        worksheet.Cells[6, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        worksheet.Cells[6, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

        // Note about data source
        worksheet.Cells[8, 1].Value = "Note: These values are calculated from the detailed data in the other sheets.";
        worksheet.Cells[8, 1].Style.Font.Italic = true;
        worksheet.Cells[8, 1, 8, 2].Merge = true;

        worksheet.Cells.AutoFitColumns();
        await Task.CompletedTask;
    }

    private async Task CreateProfitByCategorySheet(ExcelWorksheet worksheet, List<GroceryItemDto> groceries)
    {
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
        worksheet.Cells[1, 1].Value = "Category";
        worksheet.Cells[1, 2].Value = "Items";
        worksheet.Cells[1, 3].Value = "Total Stock";
        worksheet.Cells[1, 4].Value = "Avg Profit";
        worksheet.Cells[1, 5].Value = "Total Potential";

        // Style headers
        using var headerRange = worksheet.Cells[1, 1, 1, 5];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

        // Data
        for (int i = 0; i < categoryStats.Count; i++)
        {
            var category = categoryStats[i];
            var row = i + 2;

            worksheet.Cells[row, 1].Value = category.Category.ToString();
            worksheet.Cells[row, 2].Value = category.ItemCount;
            worksheet.Cells[row, 3].Value = category.TotalStock;
            worksheet.Cells[row, 4].Value = category.AverageProfit;
            worksheet.Cells[row, 5].Value = category.TotalPotentialProfit;
        }

        // Format currency columns
        worksheet.Cells[2, 4, categoryStats.Count + 1, 5].Style.Numberformat.Format = "$#,##0.00";

        worksheet.Cells.AutoFitColumns();
        await Task.CompletedTask;
    }

    private async Task CreateTopProfitableItemsSheet(ExcelWorksheet worksheet, List<GroceryItemDto> groceries)
    {
        // Get top profitable items (same logic as in Reports.razor)
        var topProfitableItems = groceries
            .OrderByDescending(g => g.Price - g.CostToProduce)
            .Take(10)
            .ToList();

        // Headers
        worksheet.Cells[1, 1].Value = "Item";
        worksheet.Cells[1, 2].Value = "Category";
        worksheet.Cells[1, 3].Value = "Profit";
        worksheet.Cells[1, 4].Value = "Margin %";

        // Style headers
        using var headerRange = worksheet.Cells[1, 1, 1, 4];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

        // Data
        for (int i = 0; i < topProfitableItems.Count; i++)
        {
            var item = topProfitableItems[i];
            var row = i + 2;
            var profit = item.Price - item.CostToProduce;
            var marginPercent = (profit / item.Price * 100);

            worksheet.Cells[row, 1].Value = item.Name;
            worksheet.Cells[row, 2].Value = item.Category.ToString();
            worksheet.Cells[row, 3].Value = profit;
            worksheet.Cells[row, 4].Value = marginPercent;
        }

        // Format currency and percentage columns
        worksheet.Cells[2, 3, topProfitableItems.Count + 1, 3].Style.Numberformat.Format = "$#,##0.00";
        worksheet.Cells[2, 4, topProfitableItems.Count + 1, 4].Style.Numberformat.Format = "0.0%";

        worksheet.Cells.AutoFitColumns();
        await Task.CompletedTask;
    }

    private async Task CreateStockAnalysisSheet(ExcelWorksheet worksheet, List<GroceryItemDto> groceries)
    {
        // Stock analysis (same logic as in Reports.razor)
        var highStockItems = groceries.Where(g => g.Stock > 5).ToList();
        var lowStockItems = groceries.Where(g => g.Stock >= 1 && g.Stock <= 5).ToList();
        var outOfStockItems = groceries.Where(g => g.Stock == 0).ToList();

        // Headers
        worksheet.Cells[1, 1].Value = "Status";
        worksheet.Cells[1, 2].Value = "Items";
        worksheet.Cells[1, 3].Value = "Total Stock";
        worksheet.Cells[1, 4].Value = "Value";

        // Style headers
        using var headerRange = worksheet.Cells[1, 1, 1, 4];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);

        // Data
        var stockData = new[]
        {
            new { Status = "High Stock (>5)", Items = highStockItems, Color = System.Drawing.Color.LightGreen },
            new { Status = "Low Stock (1-5)", Items = lowStockItems, Color = System.Drawing.Color.LightYellow },
            new { Status = "Out of Stock (0)", Items = outOfStockItems, Color = System.Drawing.Color.LightPink }
        };

        for (int i = 0; i < stockData.Length; i++)
        {
            var level = stockData[i];
            var row = i + 2;

            worksheet.Cells[row, 1].Value = level.Status;
            worksheet.Cells[row, 2].Value = level.Items.Count;
            worksheet.Cells[row, 3].Value = level.Items.Sum(g => g.Stock);
            worksheet.Cells[row, 4].Value = level.Items.Sum(g => g.Stock * g.Price);

            // Apply row color
            using var rowRange = worksheet.Cells[row, 1, row, 4];
            rowRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            rowRange.Style.Fill.BackgroundColor.SetColor(level.Color);
        }

        // Format currency columns
        worksheet.Cells[2, 4, stockData.Length + 1, 4].Style.Numberformat.Format = "$#,##0.00";

        worksheet.Cells.AutoFitColumns();
        await Task.CompletedTask;
    }

    private async Task CreatePriceRangeAnalysisSheet(ExcelWorksheet worksheet, List<GroceryItemDto> groceries)
    {
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
        worksheet.Cells[1, 1].Value = "Price Range";
        worksheet.Cells[1, 2].Value = "Items";
        worksheet.Cells[1, 3].Value = "Avg Cost";
        worksheet.Cells[1, 4].Value = "Avg Profit";
        worksheet.Cells[1, 5].Value = "Profit Margin";

        // Style headers
        using var headerRange = worksheet.Cells[1, 1, 1, 5];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

        // Data
        for (int i = 0; i < priceRanges.Count; i++)
        {
            var range = priceRanges[i];
            var row = i + 2;
            var items = range.Items.ToList();

            worksheet.Cells[row, 1].Value = range.Range;
            worksheet.Cells[row, 2].Value = items.Count;
            worksheet.Cells[row, 3].Value = items.Average(g => g.CostToProduce);
            worksheet.Cells[row, 4].Value = items.Average(g => g.Price - g.CostToProduce);
            worksheet.Cells[row, 5].Value = items.Average(g => (g.Price - g.CostToProduce) / g.Price);
        }

        // Format currency and percentage columns
        worksheet.Cells[2, 3, priceRanges.Count + 1, 4].Style.Numberformat.Format = "$#,##0.00";
        worksheet.Cells[2, 5, priceRanges.Count + 1, 5].Style.Numberformat.Format = "0.0%";

        worksheet.Cells.AutoFitColumns();
        await Task.CompletedTask;
    }
}