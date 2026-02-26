using MediatR;

namespace Shared.Contracts.Catalog;

// INotification = MediatR's way of saying "this is a pub/sub event"
// Multiple handlers can subscribe to this — the publisher doesn't know who listens
public record OrderPlacedEvent(Guid ProductId, int Quantity) : INotification;
