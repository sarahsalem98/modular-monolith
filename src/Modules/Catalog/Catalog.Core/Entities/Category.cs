using Shared.Domain;

namespace Catalog.Core.Entities;

public class Category : IEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    // so nobody outside can create an invalid Category
    private Category() { }

    public static Category Create(string name)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name
        };
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}
