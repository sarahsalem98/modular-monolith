using Customers.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customers.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<User>> GetAllAsync();
        Task AddAsync(User product);
        Task UpdateAsync(User product);
        Task DeleteAsync(Guid id);
    }
}
