using Customers.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customers.Api
{
    public class CustomerModule :IModule
    {
        public string Name => "Customers";
        public void RegisterServices(IServiceCollection services)
        {
            services.AddDbContext<CustomerDbContext>(options =>
                options.UseSqlServer("Name=ConnectionStrings:CustomersDb",
                    sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "customer")));

            services.AddScoped<Core.Repositories.IUserRepository, Infrastructure.Repositories.UserRepository>();

            // Exposed to other modules via Shared contract
            services.AddScoped<Shared.Contracts.Customers.ICustomerService, Infrastructure.Services.CustomerService>();
        }
    }
}
