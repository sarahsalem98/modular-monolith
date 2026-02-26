using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Api.Endpoints;

public static class ProductEndpoints
{
    public record CreateProductRequest(string Name, int Quantity, decimal Price, Guid CategoryId);

    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog/products").WithTags("Catalog");

        group.MapGet("/", async (IProductRepository repo) =>
        {
            var products = await repo.GetAllAsync();
            return Results.Ok(products);
        });

        group.MapGet("/{id:guid}", async (Guid id, IProductRepository repo) =>
        {
            var product = await repo.GetByIdAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        group.MapPost("/", async (CreateProductRequest request, IProductRepository repo) =>
        {
            var product = Product.Create(request.Name, request.Quantity, request.Price, request.CategoryId);
            await repo.AddAsync(product);
            return Results.Created($"/api/catalog/products/{product.Id}", product);
        });

        group.MapDelete("/{id:guid}", async (Guid id, IProductRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
