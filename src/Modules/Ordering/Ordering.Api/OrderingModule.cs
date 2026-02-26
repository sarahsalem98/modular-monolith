using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories;
using Shared.Modules;

namespace Ordering.Api;

public class OrderingModule : IModule
{
    public string Name => "Ordering";

    public void RegisterServices(IServiceCollection services)
    {
        services.AddDbContext<OrderingDbContext>(options =>
            options.UseSqlServer("Name=ConnectionStrings:OrderingDb",
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "ordering")));

        services.AddScoped<IOrderRepository, OrderRepository>();

        // Notice: we do NOT register IProductService or ICustomerService here
        // Those are registered by THEIR OWN modules (Catalog and Customers)
        // Ordering just consumes them via dependency injection
    }
}
