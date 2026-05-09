using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.ValueObjects;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateUserCommand, Result<UpdateUserResponse>>
{
    public async Task<Result<UpdateUserResponse>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UpdateUserResponse>(
                Error.NotFound("Users.UserNotFound", "The user with the specified ID was not found."));
        }

        var newEmail = Email.Create(request.Email);

        if (!user.Email.Equals(newEmail))
        {
            var emailTaken = await userRepository.ExistsByEmailAsync(newEmail.Value, cancellationToken);
            if (emailTaken)
            {
                return Result.Failure<UpdateUserResponse>(
                    Error.Conflict("Users.EmailAlreadyExists", "A user with the same email already exists."));
            }
        }

        user.ChangeName(request.Name);
        user.ChangeEmail(newEmail);

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateUserResponse(user.Id, user.Name, user.Email.Value));
    }
}
