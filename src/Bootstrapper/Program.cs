using Catalog.Api;
using Catalog.Api.Endpoints;
using Customers.Api;
using Customers.Api.Endpoints;
using Ordering.Api;
using Ordering.Api.Endpoints;
using Shared.Modules;

var builder = WebApplication.CreateBuilder(args);

// --- Module Registration ---
// Each module registers its OWN services (DbContext, repos, contracts)
// The Bootstrapper doesn't know or care about module internals
IModule[] modules =
[
    new CatalogModule(),
    new CustomerModule(),
    new OrderingModule()
];

foreach (var module in modules)
{
    Console.WriteLine($"Registering module: {module.Name}");
    module.RegisterServices(builder.Services);
}

// MediatR scans these assemblies for event handlers
// Each module's handlers are discovered automatically
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Catalog.Infrastructure.EventHandlers.OrderPlacedHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(OrderingModule).Assembly);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// --- Endpoint Registration ---
// Each module maps its own routes
app.MapProductEndpoints();
app.MapCategoryEndpoints();
app.MapUserEndpoints();
app.MapOrderEndpoints();

app.Run();
