-- Schema initialization for RestaurantOps legacy app
-- Run against SQL Server (2019+). Adjust types if using another provider.

CREATE DATABASE RestaurantOps;
GO

USE RestaurantOps;
GO

-- Category table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL
);
GO

-- Menu Item table
CREATE TABLE MenuItems (
    MenuItemId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryId),
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000) NULL,
    Price DECIMAL(10,2) NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1
);
GO

-- Seed sample data
INSERT INTO Categories (Name, Description)
VALUES ('Starters', 'Appetizers to begin the meal'),
       ('Mains', 'Main course offerings'),
       ('Desserts', 'Sweet endings'),
       ('Drinks', 'Beverages');

INSERT INTO MenuItems (CategoryId, Name, Description, Price)
VALUES 
    (1, 'Bruschetta', 'Toasted bread with tomato and basil', 6.50),
    (1, 'Stuffed Mushrooms', 'Mushrooms filled with cheese and herbs', 7.00),
    (2, 'Grilled Salmon', 'Served with seasonal vegetables', 18.00),
    (2, 'Chicken Parmesan', 'Breaded chicken breast with marinara and cheese', 16.50),
    (2, 'Vegetarian Lasagna', 'Layers of pasta, vegetables, and cheese', 14.00),
    (3, 'Cheesecake', 'Classic New York style', 7.00),
    (3, 'Chocolate Lava Cake', 'Warm chocolate cake with molten center', 8.00),
    (3, 'Tiramisu', 'Coffee-flavored Italian dessert', 7.50),
    (4, 'Cappuccino', 'Espresso with steamed milk foam', 4.00),
    (4, 'Herbal Tea', 'Selection of herbal infusions', 3.50);

GO 

-- =============================================
-- Tables & Order Management
-- =============================================

-- Dining tables
CREATE TABLE RestaurantTables (
    TableId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Seats INT NOT NULL,
    IsOccupied BIT NOT NULL DEFAULT 0
);
GO

-- Orders (one active per table)
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    TableId INT NOT NULL FOREIGN KEY REFERENCES RestaurantTables(TableId),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ClosedAt DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Open'
);
GO

-- Order lines
CREATE TABLE OrderLines (
    OrderLineId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderId) ON DELETE CASCADE,
    MenuItemId INT NOT NULL FOREIGN KEY REFERENCES MenuItems(MenuItemId),
    Quantity INT NOT NULL,
    PriceEach DECIMAL(10,2) NOT NULL
);
GO

-- Seed tables
INSERT INTO RestaurantTables (Name, Seats)
VALUES ('T1', 4), ('T2', 4), ('T3', 2), ('T4', 2), ('T5', 6);
GO 

-- =============================================
-- Inventory
-- =============================================

-- Ingredients master list
CREATE TABLE Ingredients (
    IngredientId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Unit NVARCHAR(20) NOT NULL, -- e.g. kg, pcs, l
    QuantityOnHand DECIMAL(10,2) NOT NULL DEFAULT 0,
    ReorderThreshold DECIMAL(10,2) NOT NULL DEFAULT 0
);
GO

-- Inventory transactions (adjustments, usage, deliveries)
CREATE TABLE InventoryTx (
    TxId INT IDENTITY(1,1) PRIMARY KEY,
    IngredientId INT NOT NULL FOREIGN KEY REFERENCES Ingredients(IngredientId),
    TxDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    QuantityChange DECIMAL(10,2) NOT NULL,
    Notes NVARCHAR(255) NULL
);
GO

-- Seed sample ingredients for menu dishes (pizza, pasta, salmon, desserts)
INSERT INTO Ingredients (Name, Unit, QuantityOnHand, ReorderThreshold)
VALUES 
    ('Tomatoes', 'kg', 20.0, 5.0),
    ('Mozzarella', 'kg', 10.0, 3.0),
    ('Salmon Fillet', 'kg', 8.0, 2.0),
    ('Pasta Sheets', 'kg', 15.0, 4.0),
    ('Vanilla Extract', 'l', 1.5, 0.5),
    ('Basil', 'kg', 2.0, 0.5),
    ('Olive Oil', 'l', 5.0, 1.0),
    ('Parmesan', 'kg', 4.0, 1.0),
    ('Flour', 'kg', 25.0, 5.0),
    ('Eggs', 'pcs', 60, 12),
    ('Butter', 'kg', 6.0, 1.0),
    ('Sugar', 'kg', 8.0, 2.0),
    ('Cream', 'l', 4.0, 1.0),
    ('Lemon', 'kg', 3.0, 1.0),
    ('Black Pepper', 'kg', 1.0, 0.2),
    ('Salt', 'kg', 3.0, 1.0),
    ('Garlic', 'kg', 2.0, 0.5),
    ('Spinach', 'kg', 5.0, 1.0),
    ('Ricotta', 'kg', 3.0, 1.0),
    ('Chocolate', 'kg', 2.0, 0.5);
GO 

-- =============================================
-- Employee Scheduling
-- =============================================

-- Employees master
CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Role NVARCHAR(30) NOT NULL, -- e.g. Server, Cook, Host
    HireDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- Shifts (weekly schedule)
CREATE TABLE Shifts (
    ShiftId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employees(EmployeeId) ON DELETE CASCADE,
    ShiftDate DATE NOT NULL,
    StartTime TIME(0) NOT NULL,
    EndTime TIME(0) NOT NULL,
    Role NVARCHAR(30) NOT NULL
);
GO

-- Time-off requests
CREATE TABLE TimeOff (
    TimeOffId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employees(EmployeeId) ON DELETE CASCADE,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending' -- Pending, Approved, Denied
);
GO

-- Seed sample employees
INSERT INTO Employees (FirstName, LastName, Role, HireDate)
VALUES
 ('Alice', 'Smith', 'Server',  DATEADD(year,-2, GETDATE())),
 ('Bob',   'Johnson', 'Cook',   DATEADD(year,-3, GETDATE())),
 ('Carol', 'Davis', 'Host',     DATEADD(year,-1, GETDATE())),
 ('Dave',  'Miller', 'Server',  DATEADD(year,-4, GETDATE())),
 ('Eve',   'Wilson', 'Cook',    DATEADD(year,-2, GETDATE()));
GO 