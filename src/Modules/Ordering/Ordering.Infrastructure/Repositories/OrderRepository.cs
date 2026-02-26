using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _db;

    public OrderRepository(OrderingDbContext db)
    {
        _db = db;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
        => await _db.Orders.FindAsync(id);

    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId)
        => await _db.Orders.Where(o => o.CustomerId == customerId).ToListAsync();

    public async Task<IReadOnlyList<Order>> GetAllAsync()
        => await _db.Orders.ToListAsync();

    public async Task AddAsync(Order order)
    {
        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
    }
}
