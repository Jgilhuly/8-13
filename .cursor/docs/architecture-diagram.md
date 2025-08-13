# RestaurantOps System Architecture

## High-Level System Overview

```mermaid
graph TB
    subgraph "Client Layer"
        Browser[Web Browser]
        Mobile[Mobile App - Future]
    end
    
    subgraph "Presentation Layer"
        Controllers[ASP.NET Core Controllers]
        Views[Razor Views]
        CSS[CSS/Styling]
        JS[JavaScript]
    end
    
    subgraph "Business Logic Layer"
        Services[Business Services]
        Validators[Validation Logic]
        Workflows[Business Workflows]
    end
    
    subgraph "Data Access Layer"
        EF[Entity Framework Core]
        ADO[ADO.NET SqlHelper]
        Repositories[Repository Classes]
    end
    
    subgraph "Data Layer"
        SQL[SQL Server Database]
        Migrations[EF Core Migrations]
    end
    
    subgraph "Infrastructure"
        DI[Dependency Injection]
        Logging[Structured Logging]
        Config[Configuration]
    end
    
    Browser --> Controllers
    Mobile --> Controllers
    Controllers --> Views
    Views --> CSS
    Views --> JS
    
    Controllers --> Services
    Services --> Validators
    Services --> Workflows
    
    Services --> Repositories
    Repositories --> EF
    Repositories --> ADO
    EF --> SQL
    ADO --> SQL
    EF --> Migrations
    
    Controllers --> DI
    Services --> DI
    Repositories --> DI
    DI --> Config
    DI --> Logging
```

## Detailed Component Architecture

```mermaid
graph LR
    subgraph "Controllers"
        HC[HomeController]
        IC[InventoryController]
        KDC[KitchenDisplayController]
        MC[MenuController]
        OC[OrderController]
        PGC[PaymentGatewayController]
        SC[ScheduleController]
        TC[TablesController]
    end
    
    subgraph "Models"
        Cat[Category]
        Emp[Employee]
        Ing[Ingredient]
        InvTx[InventoryTx]
        MI[MenuItem]
        Ord[Order]
        OL[OrderLine]
        RT[RestaurantTable]
        Shift[Shift]
        TO[TimeOff]
    end
    
    subgraph "Repositories"
        EmpRepo[EmployeeRepository]
        IngRepo[IngredientRepository]
        InvRepo[InventoryRepository]
        MenuRepo[MenuRepository]
        OrdRepo[OrderRepository]
        ShiftRepo[ShiftRepository]
        TableRepo[TableRepository]
    end
    
    subgraph "Data Context"
        DbContext[RestaurantOpsDbContext]
        SqlHelper[SqlHelper]
    end
    
    subgraph "Database Tables"
        CatTable[Categories]
        EmpTable[Employees]
        IngTable[Ingredients]
        InvTxTable[InventoryTx]
        MITable[MenuItems]
        OrdTable[Orders]
        OLTable[OrderLines]
        RTTable[RestaurantTables]
        ShiftTable[Shifts]
        TOTable[TimeOff]
    end
    
    HC --> Cat
    IC --> Ing
    IC --> InvTx
    KDC --> Ord
    KDC --> OL
    MC --> Cat
    MC --> MI
    OC --> Ord
    OC --> OL
    OC --> RT
    PGC --> Ord
    SC --> Emp
    SC --> Shift
    SC --> TO
    TC --> RT
    
    Cat --> MenuRepo
    Emp --> EmpRepo
    Ing --> IngRepo
    InvTx --> InvRepo
    MI --> MenuRepo
    Ord --> OrdRepo
    OL --> OrdRepo
    RT --> TableRepo
    Shift --> ShiftRepo
    TO --> EmpRepo
    
    MenuRepo --> DbContext
    EmpRepo --> DbContext
    IngRepo --> SqlHelper
    InvRepo --> SqlHelper
    OrdRepo --> DbContext
    TableRepo --> DbContext
    ShiftRepo --> DbContext
    
    DbContext --> CatTable
    DbContext --> EmpTable
    DbContext --> MITable
    DbContext --> OrdTable
    DbContext --> OLTable
    DbContext --> RTTable
    DbContext --> ShiftTable
    DbContext --> TOTable
    
    SqlHelper --> IngTable
    SqlHelper --> InvTxTable
```

## Data Flow Architecture

```mermaid
sequenceDiagram
    participant User
    participant Controller
    participant Service
    participant Repository
    participant Database
    
    Note over User,Database: Order Processing Flow
    
    User->>Controller: Place Order
    Controller->>Service: Validate Order
    Service->>Repository: Check Inventory
    Repository->>Database: Query Stock Levels
    Database-->>Repository: Stock Data
    Repository-->>Service: Inventory Status
    Service->>Repository: Create Order
    Repository->>Database: Insert Order
    Database-->>Repository: Order ID
    Repository-->>Service: Order Created
    Service-->>Controller: Order Result
    Controller-->>User: Order Confirmation
    
    Note over User,Database: Inventory Management Flow
    
    User->>Controller: Adjust Stock
    Controller->>Service: Process Adjustment
    Service->>Repository: Update Inventory
    Repository->>Database: Insert Transaction
    Repository->>Database: Update Stock Level
    Database-->>Repository: Success
    Repository-->>Service: Stock Updated
    Service-->>Controller: Adjustment Complete
    Controller-->>User: Stock Updated
```

## Database Schema Relationships

```mermaid
erDiagram
    Categories ||--o{ MenuItems : "has many"
    MenuItems ||--o{ OrderLines : "ordered in"
    RestaurantTables ||--o{ Orders : "seats customers"
    Orders ||--o{ OrderLines : "contains"
    Ingredients ||--o{ InventoryTx : "tracked in"
    Employees ||--o{ Shifts : "work during"
    Employees ||--o{ TimeOff : "request"
    
    Categories {
        int CategoryId PK
        string Name
        string Description
    }
    
    MenuItems {
        int MenuItemId PK
        int CategoryId FK
        string Name
        string Description
        decimal Price
        boolean IsAvailable
    }
    
    Orders {
        int OrderId PK
        int TableId FK
        datetime CreatedAt
        datetime ClosedAt
        string Status
    }
    
    OrderLines {
        int OrderLineId PK
        int OrderId FK
        int MenuItemId FK
        int Quantity
        string SpecialInstructions
    }
    
    RestaurantTables {
        int TableId PK
        string TableNumber
        int Capacity
        string Status
    }
    
    Ingredients {
        int IngredientId PK
        string Name
        string Description
        decimal QuantityOnHand
        string Unit
    }
    
    InventoryTx {
        int TxId PK
        int IngredientId FK
        datetime TxDate
        decimal QuantityChange
        string Notes
    }
    
    Employees {
        int EmployeeId PK
        string FirstName
        string LastName
        string Role
        string Email
    }
    
    Shifts {
        int ShiftId PK
        int EmployeeId FK
        datetime StartTime
        datetime EndTime
        string Status
    }
    
    TimeOff {
        int TimeOffId PK
        int EmployeeId FK
        datetime StartDate
        datetime EndDate
        string Reason
        string Status
    }
```

## Technology Stack

```mermaid
graph TB
    subgraph "Frontend"
        Razor[Razor Views]
        Bootstrap[Bootstrap CSS]
        jQuery[jQuery]
        CustomJS[Custom JavaScript]
    end
    
    subgraph "Backend Framework"
        ASPNET[ASP.NET Core 8.0]
        MVC[MVC Pattern]
        DI[Dependency Injection]
        Logging[Structured Logging]
    end
    
    subgraph "Data Access"
        EFCore[Entity Framework Core]
        ADONET[ADO.NET]
        SQLServer[SQL Server]
        Migrations[EF Core Migrations]
    end
    
    subgraph "Development Tools"
        VS[Visual Studio]
        VSCode[VS Code]
        Git[Git Version Control]
        Testing[xUnit Testing]
    end
    
    Razor --> ASPNET
    Bootstrap --> Razor
    jQuery --> Razor
    CustomJS --> Razor
    
    ASPNET --> MVC
    ASPNET --> DI
    ASPNET --> Logging
    
    MVC --> EFCore
    MVC --> ADONET
    EFCore --> SQLServer
    ADONET --> SQLServer
    EFCore --> Migrations
    
    VS --> ASPNET
    VSCode --> ASPNET
    Git --> ASPNET
    Testing --> ASPNET
```

## Deployment Architecture

```mermaid
graph TB
    subgraph "Development Environment"
        DevApp[Local ASP.NET Core App]
        DevDB[Local SQL Server]
        DevConfig[appsettings.Development.json]
    end
    
    subgraph "Production Environment"
        WebServer[Web Server]
        AppPool[Application Pool]
        ProdDB[Production SQL Server]
        ProdConfig[appsettings.Production.json]
    end
    
    subgraph "Infrastructure"
        LoadBalancer[Load Balancer]
        Monitoring[Application Monitoring]
        Logging[Centralized Logging]
        Backup[Database Backup]
    end
    
    DevApp --> DevDB
    DevApp --> DevConfig
    
    WebServer --> AppPool
    AppPool --> ProdDB
    AppPool --> ProdConfig
    
    LoadBalancer --> WebServer
    WebServer --> Monitoring
    WebServer --> Logging
    ProdDB --> Backup
```

## Key Architectural Decisions

### 1. Hybrid Data Access
- **Entity Framework Core**: For new development and complex queries
- **ADO.NET**: For legacy operations and performance-critical queries
- **Repository Pattern**: Abstracts data access implementation details

### 2. Layered Architecture
- **Presentation Layer**: Controllers and Views
- **Business Logic Layer**: Services and validators
- **Data Access Layer**: Repositories and data context
- **Data Layer**: SQL Server database

### 3. Dependency Injection
- **Service Registration**: Configured in Program.cs
- **Constructor Injection**: Used throughout the application
- **Interface-based Design**: Enables testing and flexibility

### 4. Configuration Management
- **Environment-specific**: Separate configs for dev/prod
- **User Secrets**: Secure configuration during development
- **Environment Variables**: Production configuration override

### 5. Testing Strategy
- **Unit Tests**: Isolated component testing
- **Integration Tests**: Database and service integration
- **Test Builders**: Consistent test data construction
