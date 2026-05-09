using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.Entities;

public sealed class Role : Entity<Guid>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    private Role() { }

    private Role(Guid id, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Role name cannot be empty.");

        Id = id;
        Name = name.Trim();
        Description = description.Trim();
    }

    public static Role Create(string name, string description = "") =>
        new(Guid.NewGuid(), name, description);
}
