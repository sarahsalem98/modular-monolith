using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Api.Endpoints;

public static class CategoryEndpoints
{
    public record CreateCategoryRequest(string Name);

    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog/categories").WithTags("Catalog");

        group.MapGet("/", async (ICategoryRepository repo) =>
        {
            var categories = await repo.GetAllAsync();
            return Results.Ok(categories);
        });

        group.MapPost("/", async (CreateCategoryRequest request, ICategoryRepository repo) =>
        {
            var category = Category.Create(request.Name);
            await repo.AddAsync(category);
            return Results.Created($"/api/catalog/categories/{category.Id}", category);
        });
    }
}
