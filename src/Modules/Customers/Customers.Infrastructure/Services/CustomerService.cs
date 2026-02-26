using Customers.Infrastructure.Data;
using Shared.Contracts.Customers;

namespace Customers.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly CustomerDbContext _db;

    public CustomerService(CustomerDbContext db)
    {
        _db = db;
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return null;

        return new CustomerDto(user.Id, user.Name, user.Email);
    }
}
