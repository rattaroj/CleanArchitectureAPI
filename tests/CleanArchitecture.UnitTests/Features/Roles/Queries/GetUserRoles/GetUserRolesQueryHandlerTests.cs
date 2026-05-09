using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Features.Roles.Queries.GetUserRoles;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CleanArchitecture.UnitTests.Features.Roles.Queries.GetUserRoles;

public sealed class GetUserRolesQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IRoleRepository> _roleRepoMock = new();
    private readonly GetUserRolesQueryHandler _sut;

    public GetUserRolesQueryHandlerTests()
    {
        _sut = new GetUserRolesQueryHandler(_userRepoMock.Object, _roleRepoMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsUserRoleDtos()
    {
        var user = User.Create("Alice", Email.Create("alice@example.com"));
        var role = Role.Create("Admin", "Full access");
        var userRole = UserRole.Create(user.Id, role.Id);

        // Set the navigation property so the handler can access Role.Name / Role.Description
        typeof(UserRole).GetProperty(nameof(UserRole.Role))!.SetValue(userRole, role);

        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(r => r.GetUserRolesAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserRole> { userRole });

        var query = new GetUserRolesQuery(user.Id);
        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
        result.Value[0].RoleId.Should().Be(role.Id);
        result.Value[0].Name.Should().Be("Admin");
        result.Value[0].Description.Should().Be("Full access");
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        var unknownId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdAsync(unknownId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var query = new GetUserRolesQuery(unknownId);
        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Users.UserNotFound");

        _roleRepoMock.Verify(r => r.GetUserRolesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserHasNoRoles_ReturnsEmptyList()
    {
        var user = User.Create("Bob", Email.Create("bob@example.com"));
        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(r => r.GetUserRolesAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserRole>());

        var query = new GetUserRolesQuery(user.Id);
        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().BeEmpty();
    }
}
