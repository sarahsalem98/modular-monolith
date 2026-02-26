using Customers.Core.Entities;
using Customers.Core.Repositories;
using Customers.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customers.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CustomerDbContext _db;
        public UserRepository(CustomerDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(User user)
        {
              await _db.Users.AddAsync(user);    
              await _db.SaveChangesAsync();

        }

        public async Task DeleteAsync(Guid id)
        {
            var user =await  _db.Users.FindAsync(id);
            if (user != null) {
                _db.Users.Remove(user);
 
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            return await _db.Users.ToListAsync();
        }

        public async  Task<User?> GetByIdAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            return user;

        }

        public Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            return _db.SaveChangesAsync();
        }
    }
}
