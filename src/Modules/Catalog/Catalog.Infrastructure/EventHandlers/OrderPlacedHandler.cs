using Catalog.Infrastructure.Data;
using MediatR;
using Shared.Contracts.Catalog;

namespace Catalog.Infrastructure.EventHandlers;

// This handler runs whenever someone publishes an OrderPlacedEvent
// Catalog doesn't know WHO published it — it just reacts
public class OrderPlacedHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly CatalogDbContext _db;

    public OrderPlacedHandler(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FindAsync([notification.ProductId], cancellationToken);
        if (product is null) return;

        product.AdjustQuantity(product.Quantity - notification.Quantity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
