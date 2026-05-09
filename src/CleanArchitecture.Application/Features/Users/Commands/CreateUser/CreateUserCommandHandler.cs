using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    public async Task<Result<CreateUserResponse>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var email = Email.Create(request.Email);

        var exists = await userRepository.ExistsByEmailAsync(email.Value, cancellationToken);
        if(exists)
        {
            return Result.Failure<CreateUserResponse>(
                Error.Conflict("Users.EmailAlreadyExists", "A user with the same email already exists."));
        }

        var user = User.Create(request.Name, email);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateUserResponse(user.Id, user.Name, user.Email.Value));
    }
}