using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Features.Roles.Commands.AssignRole;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CleanArchitecture.UnitTests.Features.Roles.Commands.AssignRole;

public sealed class AssignRoleCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IRoleRepository> _roleRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly AssignRoleCommandHandler _sut;

    public AssignRoleCommandHandlerTests()
    {
        _sut = new AssignRoleCommandHandler(
            _userRepoMock.Object,
            _roleRepoMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidAssignment_ReturnsSuccess()
    {
        var user = User.Create("Alice", Email.Create("alice@example.com"));
        var role = Role.Create("Admin", "Full access");

        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(r => r.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _roleRepoMock.Setup(r => r.IsRoleAssignedAsync(user.Id, role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new AssignRoleCommand(user.Id, role.Id);
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _roleRepoMock.Verify(r => r.AssignRoleAsync(
            It.Is<UserRole>(ur => ur.UserId == user.Id && ur.RoleId == role.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        var unknownUserId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdAsync(unknownUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new AssignRoleCommand(unknownUserId, Guid.NewGuid());
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Users.UserNotFound");

        _roleRepoMock.Verify(r => r.AssignRoleAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ReturnsNotFoundError()
    {
        var user = User.Create("Bob", Email.Create("bob@example.com"));
        var unknownRoleId = Guid.NewGuid();

        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(r => r.GetByIdAsync(unknownRoleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var command = new AssignRoleCommand(user.Id, unknownRoleId);
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Roles.RoleNotFound");
    }

    [Fact]
    public async Task Handle_WhenRoleAlreadyAssigned_ReturnsConflictError()
    {
        var user = User.Create("Carol", Email.Create("carol@example.com"));
        var role = Role.Create("User", "Standard access");

        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(r => r.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _roleRepoMock.Setup(r => r.IsRoleAssignedAsync(user.Id, role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new AssignRoleCommand(user.Id, role.Id);
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Roles.AlreadyAssigned");
    }
}
