namespace CleanArchitecture.Domain.Entities;

public sealed class UserRole
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public Role Role { get; private set; } = default!;

    private UserRole() { }

    public static UserRole Create(Guid userId, Guid roleId) => new()
    {
        UserId = userId,
        RoleId = roleId,
        AssignedAt = DateTime.UtcNow
    };
}
