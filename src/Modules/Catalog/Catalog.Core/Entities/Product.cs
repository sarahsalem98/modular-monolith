using Shared.Domain;

namespace Catalog.Core.Entities;

public enum ProductStatus
{
    Draft,
    Active,
    Discontinued
}

public class Product : IEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public ProductStatus Status { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private Product() { }

    public static Product Create(string name, int quantity, decimal price, Guid categoryId)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Quantity = quantity,
            Price = price,
            Status = ProductStatus.Draft,
            CategoryId = categoryId
        };
    }

    public void Activate() => Status = ProductStatus.Active;

    public void Discontinue() => Status = ProductStatus.Discontinued;

    public void UpdatePrice(decimal newPrice) => Price = newPrice;

    public void AdjustQuantity(int quantity) => Quantity = quantity;
}
