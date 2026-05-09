using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Features.Users.Commands.UpdateUser;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CleanArchitecture.UnitTests.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UpdateUserCommandHandler _sut;

    public UpdateUserCommandHandlerTests()
    {
        _sut = new UpdateUserCommandHandler(_userRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndEmailNotTaken_ReturnsSuccess()
    {
        var user = User.Create("Old Name", Email.Create("old@example.com"));
        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userRepoMock.Setup(r => r.ExistsByEmailAsync("new@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UpdateUserCommand(user.Id, "New Name", "new@example.com");
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New Name");
        result.Value.Email.Should().Be("new@example.com");

        _userRepoMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailNotChanged_SkipsEmailConflictCheck()
    {
        var user = User.Create("Name", Email.Create("same@example.com"));
        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new UpdateUserCommand(user.Id, "Updated Name", "same@example.com");
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _userRepoMock.Verify(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        var unknownId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdAsync(unknownId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new UpdateUserCommand(unknownId, "Name", "email@example.com");
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Users.UserNotFound");
    }

    [Fact]
    public async Task Handle_WhenNewEmailAlreadyTaken_ReturnsConflictError()
    {
        var user = User.Create("Name", Email.Create("original@example.com"));
        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userRepoMock.Setup(r => r.ExistsByEmailAsync("taken@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new UpdateUserCommand(user.Id, "Name", "taken@example.com");
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Users.EmailAlreadyExists");

        _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
