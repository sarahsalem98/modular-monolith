# Modular Monolith - Learning Progress

## What is a Modular Monolith?

| | Traditional Monolith | **Modular Monolith** | Microservices |
|---|---|---|---|
| Deployment | Single unit | **Single unit** | Many units |
| Code organization | Tangled layers | **Independent modules** | Independent services |
| Communication | Direct calls everywhere | **Defined contracts** | Network calls (HTTP/messaging) |
| Database | Shared everything | **Schema-per-module** | DB-per-service |
| Complexity | Low initially, grows fast | **Moderate, controlled** | High from day one |

---

## Completed Steps

### Step 1: Solution Structure (DONE)

```
modular/
├── src/
│   ├── Bootstrapper/          → ASP.NET Web API (startup project)
│   ├── Shared/                → Shared kernel (interfaces, contracts)
│   └── Modules/
│       ├── Catalog/           → Products & Categories
│       ├── Customers/         → Users
│       └── Ordering/          → Orders (consumes Catalog + Customers via contracts)
```

**Reference Rules:**
- Core → Shared only
- Infrastructure → Core + Shared
- Api → Core + Infrastructure + Shared
- Bootstrapper → all Api projects
- NO module-to-module references

### Step 2: Shared Kernel (DONE)

```
Shared/
├── Domain/
│   └── IEntity.cs                    → Base interface for all entities
├── Contracts/
│   ├── IIntegrationEvent.cs          → Contract for cross-module events
│   ├── Catalog/
│   │   ├── ProductDto.cs             → What Catalog exposes to other modules
│   │   └── IProductService.cs        → Contract other modules use to query products
│   └── Customers/
│       ├── CustomerDto.cs            → What Customers exposes to other modules
│       └── ICustomerService.cs       → Contract other modules use to query customers
└── Modules/
    └── IModule.cs                    → Each module registers itself via this
```

**Key lesson:** Shared contains ONLY interfaces and DTOs — never concrete implementations.
If Shared had EF Core dependencies, every Core project would be forced to depend on EF Core too.

### Step 3: Catalog Module (DONE)

```
Catalog.Core/          (pure C# — zero external packages)
├── Entities/
│   ├── Product.cs          → private set, factory method, encapsulated logic
│   └── Category.cs
└── Repositories/
    ├── IProductRepository.cs
    └── ICategoryRepository.cs

Catalog.Infrastructure/ (EF Core lives here)
├── Data/
│   └── CatalogDbContext.cs     → Schema: "catalog"
├── Repositories/
│   ├── ProductRepository.cs
│   └── CategoryRepository.cs
└── Services/
    └── ProductService.cs       → Implements IProductService from Shared

Catalog.Api/
├── CatalogModule.cs            → Registers DbContext, repos, and IProductService
└── Endpoints/
    ├── ProductEndpoints.cs
    └── CategoryEndpoints.cs
```

### Step 4: Customers Module (DONE — built by YOU!)

```
Customers.Core/
├── Entities/
│   └── User.cs
└── Repositories/
    └── IUserRepository.cs

Customers.Infrastructure/
├── Data/
│   └── CustomerDbContext.cs    → Schema: "customers"
├── Repositories/
│   └── UserRepository.cs
└── Services/
    └── CustomerService.cs      → Implements ICustomerService from Shared

Customers.Api/
├── CustomerModule.cs
└── Endpoints/
    └── UserEndpoints.cs
```

### Step 5: Ordering Module (DONE)

**Key lesson:** Ordering needs Products and Customers but has ZERO references to their projects.
It uses `IProductService` and `ICustomerService` from Shared — injected via DI at runtime.

```
Ordering.Core/
├── Entities/
│   └── Order.cs               → Stores CustomerId/ProductId as Guids (no FK to other modules!)
└── Repositories/
    └── IOrderRepository.cs

Ordering.Infrastructure/
├── Data/
│   └── OrderingDbContext.cs    → Schema: "ordering"
└── Repositories/
    └── OrderRepository.cs

Ordering.Api/
├── OrderingModule.cs
└── Endpoints/
    └── OrderEndpoints.cs       → Injects IProductService + ICustomerService from Shared
```

Cross-module communication flow:
```
Catalog registers:    IProductService → ProductService    (in CatalogModule)
Customers registers:  ICustomerService → CustomerService  (in CustomerModule)
Ordering consumes:    IProductService + ICustomerService   (via DI, no direct reference)
```

### Step 6: Bootstrapper Wiring (DONE)

```
Program.cs:
1. Creates module instances (CatalogModule, CustomerModule, OrderingModule)
2. Calls RegisterServices() on each → each module registers its own DI
3. Maps endpoints from each module
4. Swagger UI available at /swagger

appsettings.json:
- All 3 modules point to SAME database (ModularShopDb)
- Each module uses its OWN schema (catalog, customers, ordering)
- To split a module into a microservice later: just change connection string
```

---

## Remaining Steps

### Step 7: Add Integration Events (NEXT)
- Add MediatR for in-process event publishing
- Example: when an Order is placed → publish "OrderPlaced" event
- Catalog subscribes and reduces product stock automatically
- Events travel in-memory (same process), not over network

### Future Enhancements
- Add EF Core migrations per module
- Add unit tests per module
- Add validation (FluentValidation)
- Add CQRS pattern (separate read/write models)
