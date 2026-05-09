using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }
    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Email cannot be empty.");
        }

        if (!value.Contains('@', StringComparison.Ordinal))
        {
            throw new DomainException("Invalid email format.");
        }

        return new Email(value.Trim().ToLowerInvariant());
    }

    public override string ToString() => Value;
}