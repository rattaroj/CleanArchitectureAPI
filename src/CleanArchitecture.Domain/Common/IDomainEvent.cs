namespace CleanArchitecture.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
