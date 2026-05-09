using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Events;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities;

public sealed class User : Entity<Guid>
{
    public string Name { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private User() {}

    private User(Guid id, string name, Email email)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Name cannot be empty.");
        }

        Id = id;
        Name = name.Trim();
        Email = email;
        IsActive = true;
    }

    public static User Create(string name, Email email)
    {
        var user = new User(Guid.NewGuid(), name, email);
        user.AddDomainEvent(new UserCreatedDomainEvent(user.Id));
        return user;
    }

    public void ChangeEmail(Email email)
    {
        Email = email ?? throw new DomainException("Email cannot be null.");
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
    }
}