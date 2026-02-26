using Ordering.Core.Entities;

namespace Ordering.Core.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId);
    Task<IReadOnlyList<Order>> GetAllAsync();
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}
