using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.Catalog;
using Shared.Modules;

namespace Catalog.Api;

public class CatalogModule : IModule
{
    public string Name => "Catalog";

    public void RegisterServices(IServiceCollection services)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer("Name=ConnectionStrings:CatalogDb",
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "catalog")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // This is exposed to OTHER modules via the Shared contract
        services.AddScoped<IProductService, ProductService>();
    }
}
