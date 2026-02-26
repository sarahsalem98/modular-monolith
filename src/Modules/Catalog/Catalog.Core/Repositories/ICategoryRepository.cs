using Catalog.Core.Entities;

namespace Catalog.Core.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Category>> GetAllAsync();
    Task AddAsync(Category category);
}
