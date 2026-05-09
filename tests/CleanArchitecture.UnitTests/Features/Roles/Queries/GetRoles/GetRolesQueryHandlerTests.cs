using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Features.Roles.Queries.GetRoles;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Moq;

namespace CleanArchitecture.UnitTests.Features.Roles.Queries.GetRoles;

public sealed class GetRolesQueryHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepoMock = new();
    private readonly GetRolesQueryHandler _sut;

    public GetRolesQueryHandlerTests()
    {
        _sut = new GetRolesQueryHandler(_roleRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllRolesMappedToDto()
    {
        var roles = new List<Role>
        {
            Role.Create("Admin", "Full access"),
            Role.Create("User", "Basic access"),
            Role.Create("Moderator", "Content moderation")
        };

        _roleRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        var result = await _sut.Handle(new GetRolesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(3);
        result.Value.Select(r => r.Name).Should().BeEquivalentTo("Admin", "User", "Moderator");
        result.Value.Select(r => r.Description).Should().Contain("Full access");
    }

    [Fact]
    public async Task Handle_WhenNoRoles_ReturnsEmptyList()
    {
        _roleRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Role>());

        var result = await _sut.Handle(new GetRolesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().BeEmpty();
    }
}
