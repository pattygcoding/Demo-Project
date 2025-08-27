# Grocery Shopping App

A complete full-stack .NET 9 Blazor Server application for managing grocery items. The application follows the controller/service/repository pattern with Entity Framework Core for persistence and includes a comprehensive test suite.

## Features

- **Add Grocery Items**: Create new grocery items with name, category, price, cost to produce, and stock information
- **List Grocery Items**: View all grocery items organized by category
- **View Item Details**: Display detailed information about specific grocery items  
- **Edit Grocery Items**: Update existing grocery items (including cost to produce)
- **Delete Grocery Items**: Remove grocery items with confirmation
- **Reports Dashboard**: Comprehensive reporting with profit analysis, stock levels, and category breakdowns
- **Excel Export**: Generate detailed Excel reports with 5 sheets including key metrics, profit analysis, stock analysis, and price range breakdowns
- **Data Persistence**: All data is persisted to a SQLite database using EF Core
- **Seeded Data**: Application starts with 50 pre-loaded grocery items across 5 categories

## Domain Model

The application manages grocery items with the following properties:
- **Id**: Unique identifier (auto-generated)
- **Name**: Item name (required, 1-100 characters)
- **Category**: One of Fruit, Vegetable, Meat, Cheese, or Bread
- **Price**: Item price (must be > 0)
- **Cost to Produce**: Production cost (must be > 0)
- **Stock**: Available quantity (default: 10, cannot be negative)
- **CreatedUtc**: Creation timestamp (auto-generated)

## Architecture

The application follows a layered architecture pattern:

- **GroceryApp**: Blazor Server frontend with API controllers
- **GroceryApp.Models**: Domain models, enums, and DTOs
- **GroceryApp.Data**: Entity Framework DbContext, repositories, and migrations
- **GroceryApp.Services**: Business logic layer
- **GroceryApp.Tests**: Unit tests for the service layer

## Tech Stack

- .NET 9
- Blazor Server
- ASP.NET Core Web API
- Entity Framework Core 9.0
- SQLite Database
- EPPlus for Excel generation
- xUnit for Testing
- Bootstrap 5 for Styling

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Any IDE that supports .NET development (Visual Studio, VS Code, JetBrains Rider)

## Setup Instructions

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd Test-Repo
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the solution**:
   ```bash
   dotnet build
   ```

4. **Run the application**:
   ```bash
   cd GroceryApp
   dotnet run
   ```

5. **Access the application**:
   - Open your browser and navigate to `https://localhost:7033` or `http://localhost:5073`
   - The database will be automatically created with seeded data on first run

## Running Tests

To run the unit tests:

```bash
dotnet test
```

The test suite includes comprehensive tests for the service layer using EF Core's InMemory database provider.

## Application Structure

```
GroceryApp/
├── GroceryApp/                 # Blazor Server app & API controllers
│   ├── Components/             # Blazor components and pages
│   │   ├── Layout/            # Layout components
│   │   └── Pages/             # Page components
│   ├── Controllers/           # API controllers
│   └── Program.cs             # Application configuration
├── GroceryApp.Models/         # Domain models and DTOs
│   ├── DTOs/                 # Data Transfer Objects
│   ├── GroceryItem.cs        # Main domain model
│   └── Category.cs           # Category enum
├── GroceryApp.Data/           # Data access layer
│   ├── Repositories/         # Repository pattern implementation
│   └── GroceryDbContext.cs   # EF Core DbContext
├── GroceryApp.Services/       # Business logic layer
│   ├── IGroceryService.cs    # Service interface
│   ├── GroceryService.cs     # Service implementation
│   ├── IExcelExportService.cs # Excel export service interface
│   └── ExcelExportService.cs # Excel export service implementation
└── GroceryApp.Tests/         # Unit tests
    └── GroceryServiceTests.cs # Service layer tests
```

## API Endpoints

The application exposes REST API endpoints:

- `GET /api/groceries` - Get all grocery items
- `GET /api/groceries/{id}` - Get grocery item by ID
- `POST /api/groceries` - Create new grocery item
- `PUT /api/groceries/{id}` - Update grocery item
- `DELETE /api/groceries/{id}` - Delete grocery item
- `GET /api/reports/export-excel` - Download Excel report with comprehensive analytics

## Pages

- `/` - Home page with detailed app information and quick actions
- `/groceries` - List all grocery items organized by category
- `/groceries/create` - Create new grocery item form
- `/groceries/{id}` - View grocery item details
- `/groceries/edit/{id}` - Edit grocery item form
- `/groceries/delete/{id}` - Delete confirmation page
- `/reports` - Comprehensive reports dashboard with profit analysis and Excel export

## Seeded Data

The application comes with 50 pre-loaded grocery items:
- 10 Fruits (Apple, Banana, Orange, Strawberry, Grapes, Pineapple, Mango, Kiwi, Blueberry, Peach)
- 10 Vegetables (Carrot, Broccoli, Spinach, Tomato, Cucumber, Bell Pepper, Onion, Garlic, Potato, Lettuce)
- 10 Meats (Chicken Breast, Ground Beef, Salmon Fillet, Pork Chops, Turkey Breast, Beef Steak, Ground Turkey, Bacon, Ham, Lamb Chops)
- 10 Cheeses (Cheddar, Mozzarella, Swiss, Brie, Gouda, Parmesan, Blue, Feta, Cream, Cottage)
- 10 Breads (White Bread, Whole Wheat Bread, Sourdough, Rye Bread, Baguette, Ciabatta, Pita Bread, Bagels, Croissant, Dinner Rolls)

## Development

The application uses dependency injection for all services and follows SOLID principles. The smart/dumb component pattern is implemented with pages handling data operations and reusable components for UI elements.

Form validation is implemented using data annotations, and the application includes proper error handling and user feedback.