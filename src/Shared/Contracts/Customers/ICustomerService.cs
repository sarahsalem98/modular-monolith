namespace Shared.Contracts.Customers;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(Guid id);
}
