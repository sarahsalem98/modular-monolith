namespace Shared.Contracts.Catalog;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<bool> HasSufficientStockAsync(Guid productId, int quantity);
}
