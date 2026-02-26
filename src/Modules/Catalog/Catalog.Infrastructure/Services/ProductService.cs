using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Catalog;

namespace Catalog.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly CatalogDbContext _db;

    public ProductService(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return null;

        return new ProductDto(product.Id, product.Name, product.Price, product.Quantity);
    }

    public async Task<bool> HasSufficientStockAsync(Guid productId, int quantity)
    {
        var product = await _db.Products.FindAsync(productId);
        return product is not null && product.Quantity >= quantity;
    }
}
