using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Features.Roles.Commands.RemoveRole;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CleanArchitecture.UnitTests.Features.Roles.Commands.RemoveRole;

public sealed class RemoveRoleCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IRoleRepository> _roleRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly RemoveRoleCommandHandler _sut;

    public RemoveRoleCommandHandlerTests()
    {
        _sut = new RemoveRoleCommandHandler(
            _userRepoMock.Object,
            _roleRepoMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRoleAssigned_ReturnsSuccess()
    {
        var user = User.Create("Alice", Email.Create("alice@example.com"));
        var roleId = Guid.NewGuid();

        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(r => r.IsRoleAssignedAsync(user.Id, roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RemoveRoleCommand(user.Id, roleId);
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _roleRepoMock.Verify(r => r.RemoveRoleAsync(user.Id, roleId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        var unknownUserId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdAsync(unknownUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new RemoveRoleCommand(unknownUserId, Guid.NewGuid());
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Users.UserNotFound");

        _roleRepoMock.Verify(r => r.RemoveRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRoleNotAssigned_ReturnsNotFoundError()
    {
        var user = User.Create("Bob", Email.Create("bob@example.com"));
        var roleId = Guid.NewGuid();

        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(r => r.IsRoleAssignedAsync(user.Id, roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new RemoveRoleCommand(user.Id, roleId);
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Roles.NotAssigned");

        _roleRepoMock.Verify(r => r.RemoveRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
