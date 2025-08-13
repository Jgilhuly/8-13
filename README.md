# Restaurant Operations System

A legacy-style .NET Core web application for restaurant back-office operations, designed to demonstrate modernization techniques. The app handles menu management, inventory, scheduling, and reporting while intentionally using outdated patterns that can be incrementally modernized.

## Overview

This project showcases a complete restaurant management system built with deliberate "legacy" patterns using modern .NET Core runtime. It serves as a baseline for demonstrating various modernization techniques, from infrastructure upgrades to microservices architecture.

## Features

- **Menu Management** - CRUD operations for menu items, categories, pricing, and availability
- **Table & Order Management** - Floor plan view, table seating, order creation and modification
- **Inventory Tracking** - Ingredient management, stock adjustments, and reorder thresholds
- **Employee Scheduling** - Role definitions, shift planning, and time-off requests
- **Daily Reports** - Sales summaries, labor analysis, and inventory variance
- **Integration Stubs** - Receipt printer, payment gateway, and kitchen display system

## Architecture

Built as a monolithic ASP.NET Core MVC application with:
- **Frontend**: Server-rendered Razor views with Bootstrap 4 and jQuery
- **Data Layer**: ADO.NET with `SqlConnection`, `DataTable`, and stored procedures
- **Services**: Fat service classes with no dependency injection
- **Authentication**: Cookie-based auth with hard-coded roles
- **Logging**: Basic `System.Diagnostics.Trace`

## Setup

1. **Prerequisites**:
   - .NET 6+ SDK
   - SQL Server or SQL Server Express
   - Docker (optional)

2. **Database Setup**:
   ```bash
   # Initialize the database
   dotnet run --project RestaurantOps.Legacy -- --setup-db
   ```

3. **Run the application**:
   ```bash
   cd RestaurantOps.Legacy
   dotnet run
   ```

4. **Access the application**:
   - Open `http://localhost:5000`
   - Default credentials: admin/password

## Development

For development with auto-reload:
```bash
dotnet watch run --project RestaurantOps.Legacy
```

## Modernization Roadmap

This codebase is intentionally designed with legacy patterns to demonstrate modernization paths:

1. **Infrastructure** - Add DI container, structured logging (Serilog)
2. **Data Layer** - Migrate to Entity Framework Core with async patterns
3. **Application** - Introduce MediatR, CQRS, FluentValidation
4. **Frontend** - Replace Razor views with React/Blazor SPA
5. **Architecture** - Extract microservices, add message queuing
6. **Cloud** - Deploy to Azure with App Service and managed databases

## Database Schema

Core entities include:
- **Menu**: Categories, MenuItems, Pricing
- **Orders**: Tables, Orders, OrderLines
- **Inventory**: Ingredients, InventoryTransactions
- **Staff**: Employees, Shifts, TimeOff
- **Reports**: Sales, Labor, Inventory summaries 