using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Shared.Contracts.Catalog;
using Shared.Contracts.Customers;

namespace Ordering.Api.Endpoints;

public static class OrderEndpoints
{
    public record PlaceOrderRequest(Guid CustomerId, Guid ProductId, int Quantity);

    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ordering/orders").WithTags("Ordering");

        group.MapGet("/", async (IOrderRepository repo) =>
        {
            var orders = await repo.GetAllAsync();
            return Results.Ok(orders);
        });

        group.MapGet("/{id:guid}", async (Guid id, IOrderRepository repo) =>
        {
            var order = await repo.GetByIdAsync(id);
            return order is null ? Results.NotFound() : Results.Ok(order);
        });

        // THIS is the key endpoint — notice how it uses ICustomerService
        // and IProductService from SHARED, not from Catalog/Customers directly
        group.MapPost("/", async (
            PlaceOrderRequest request,
            IOrderRepository orderRepo,
            ICustomerService customerService,
            IProductService productService,
            IPublisher publisher) =>
        {
            // 1. Validate customer exists (via Shared contract)
            var customer = await customerService.GetByIdAsync(request.CustomerId);
            if (customer is null)
                return Results.BadRequest("Customer not found");

            // 2. Validate product exists and has stock (via Shared contract)
            var product = await productService.GetByIdAsync(request.ProductId);
            if (product is null)
                return Results.BadRequest("Product not found");

            var hasStock = await productService.HasSufficientStockAsync(request.ProductId, request.Quantity);
            if (!hasStock)
                return Results.BadRequest("Insufficient stock");

            // 3. Create the order
            var order = Order.Create(
                request.CustomerId,
                request.ProductId,
                product.Name,
                request.Quantity,
                product.Price);

            await orderRepo.AddAsync(order);

            // 4. Publish event — Ordering doesn't know who handles this!
            //    Catalog will pick it up and reduce stock automatically
            await publisher.Publish(new OrderPlacedEvent(request.ProductId, request.Quantity));

            return Results.Created($"/api/ordering/orders/{order.Id}", order);
        });
    }
}
