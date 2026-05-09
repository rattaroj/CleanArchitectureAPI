using CleanArchitecture.Application.Common.Models;
using FluentValidation;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PaginationRequest.MaxPageSize)
            .WithMessage($"PageSize must be between 1 and {PaginationRequest.MaxPageSize}.");

        RuleFor(x => x.SortBy)
            .IsInEnum()
            .WithMessage("SortBy contains an invalid column name.");

        RuleFor(x => x.Direction)
            .IsInEnum()
            .WithMessage("Direction must be Asc or Desc.");

        RuleFor(x => x.NameContains)
            .MaximumLength(100)
            .WithMessage("NameContains filter cannot exceed 100 characters.")
            .When(x => x.NameContains is not null);

        RuleFor(x => x.EmailEquals)
            .MaximumLength(255)
            .WithMessage("EmailEquals filter cannot exceed 255 characters.")
            .EmailAddress()
            .WithMessage("EmailEquals must be a valid email address.")
            .When(x => x.EmailEquals is not null);
    }
}
