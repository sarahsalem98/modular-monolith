namespace Shared.Contracts.Catalog;

public record ProductDto(Guid Id, string Name, decimal Price, int Quantity);
