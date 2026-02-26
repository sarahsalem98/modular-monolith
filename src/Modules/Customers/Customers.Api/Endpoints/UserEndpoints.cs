using Customers.Core.Entities;
using Customers.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Customers.Api.Endpoints;

public static class UserEndpoints
{
    public record CreateUserRequest(string Name, string Email, string? Address);

    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers/users").WithTags("Customers");

        group.MapGet("/", async (IUserRepository repo) =>
        {
            var users = await repo.GetAllAsync();
            return Results.Ok(users);
        });

        group.MapGet("/{id:guid}", async (Guid id, IUserRepository repo) =>
        {
            var user = await repo.GetByIdAsync(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        group.MapPost("/", async (CreateUserRequest request, IUserRepository repo) =>
        {
            var user = User.Create(request.Name, request.Email, request.Address);
            await repo.AddAsync(user);
            return Results.Created($"/api/customers/users/{user.Id}", user);
        });
    }
}
