using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Events;

public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
