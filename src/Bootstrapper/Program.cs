using Catalog.Api;
using Catalog.Api.Endpoints;
using Catalog.Infrastructure.Data;
using Customers.Api;
using Customers.Api.Endpoints;
using Customers.Infrastructure.Data;
using Ordering.Api;
using Ordering.Api.Endpoints;
using Ordering.Infrastructure.Data;
using Serilog;
using Shared.Exceptions;
using Shared.Modules;

// ──────────────────────────────────────────────
// SERILOG — Structured logging (replaces Console.WriteLine)
// WHY: Structured logs let you query by properties (e.g. "show me all requests > 500ms")
//       Console.WriteLine is unstructured text — useless at scale
// ──────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Replace default logging with Serilog
    builder.Host.UseSerilog();

    // --- Module Registration ---
    IModule[] modules =
    [
        new CatalogModule(),
        new CustomerModule(),
        new OrderingModule()
    ];

    foreach (var module in modules)
    {
        Log.Information("Registering module: {ModuleName}", module.Name);
        module.RegisterServices(builder.Services);
    }

    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(Catalog.Infrastructure.EventHandlers.OrderPlacedHandler).Assembly);
        cfg.RegisterServicesFromAssembly(typeof(OrderingModule).Assembly);
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ──────────────────────────────────────────────
    // GLOBAL EXCEPTION HANDLER — catches ALL unhandled exceptions
    // WHY: One place for all error handling instead of try/catch in every endpoint
    //       Returns consistent RFC 7807 Problem Details format
    //       Logs every error with full context (method, path, stack trace)
    // ──────────────────────────────────────────────
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // ──────────────────────────────────────────────
    // HEALTH CHECKS — "Is the app alive? Is the DB reachable?"
    // WHY: IIS/load balancers ping /health to decide if your app should receive traffic
    //       Without this, a broken app keeps getting requests and failing silently
    //
    // AddDbContextCheck<T> runs a SELECT 1 against each database
    // If any DB is down, health check returns "Unhealthy"
    // ──────────────────────────────────────────────
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<CatalogDbContext>("catalog-db")
        .AddDbContextCheck<CustomerDbContext>("customers-db")
        .AddDbContextCheck<OrderingDbContext>("ordering-db");

    var app = builder.Build();

    // ──────────────────────────────────────────────
    // REQUEST TIMING MIDDLEWARE — Serilog logs every HTTP request with duration
    // WHY: This is how you measure p99 latency (Chapter 1 key concept)
    //       "Amazon found even 100ms latency increases cost them 1% of sales"
    //       You can't optimize what you don't measure
    //
    // Each log entry includes: HTTP method, path, status code, elapsed time
    // Example: HTTP GET /api/products responded 200 in 45.2ms
    // ──────────────────────────────────────────────
    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    app.UseSwagger();
    app.UseSwaggerUI();

    // Map health check endpoint
    // Hit GET /health to see: Healthy / Degraded / Unhealthy
    app.MapHealthChecks("/health");

    // --- Endpoint Registration ---
    app.MapProductEndpoints();
    app.MapCategoryEndpoints();
    app.MapUserEndpoints();
    app.MapOrderEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
