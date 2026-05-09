using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Features.Users.Queries.GetUsers;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CleanArchitecture.UnitTests.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly GetUsersQueryHandler _sut;

    public GetUsersQueryHandlerTests()
    {
        _sut = new GetUsersQueryHandler(_userRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResultMappedToDto()
    {
        var user1 = User.Create("Alice", Email.Create("alice@example.com"));
        var user2 = User.Create("Bob", Email.Create("bob@example.com"));
        var pagedResult = PagedResult<User>.Create(
            new[] { user1, user2 }, pageNumber: 1, pageSize: 10, totalCount: 2);

        _userRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<PaginationRequest>(),
                It.IsAny<GetUsersFilter>(),
                It.IsAny<SortRequest<UserSortColumn>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var query = new GetUsersQuery(PageNumber: 1, PageSize: 10);
        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.Items[0].Name.Should().Be("Alice");
        result.Value.Items[1].Name.Should().Be("Bob");
        result.Value.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithFilter_PassesFilterToRepository()
    {
        var pagedResult = PagedResult<User>.Create(
            Array.Empty<User>(), pageNumber: 1, pageSize: 10, totalCount: 0);

        GetUsersFilter? capturedFilter = null;
        _userRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<PaginationRequest>(),
                It.IsAny<GetUsersFilter>(),
                It.IsAny<SortRequest<UserSortColumn>>(),
                It.IsAny<CancellationToken>()))
            .Callback<PaginationRequest, GetUsersFilter, SortRequest<UserSortColumn>, CancellationToken>(
                (_, filter, _, _) => capturedFilter = filter)
            .ReturnsAsync(pagedResult);

        var query = new GetUsersQuery(NameContains: "alice", IsActive: true);
        await _sut.Handle(query, CancellationToken.None);

        capturedFilter!.NameContains.Should().Be("alice");
        capturedFilter.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithSort_PassesSortToRepository()
    {
        var pagedResult = PagedResult<User>.Create(
            Array.Empty<User>(), pageNumber: 1, pageSize: 10, totalCount: 0);

        SortRequest<UserSortColumn>? capturedSort = null;
        _userRepoMock.Setup(r => r.GetPagedAsync(
                It.IsAny<PaginationRequest>(),
                It.IsAny<GetUsersFilter>(),
                It.IsAny<SortRequest<UserSortColumn>>(),
                It.IsAny<CancellationToken>()))
            .Callback<PaginationRequest, GetUsersFilter, SortRequest<UserSortColumn>, CancellationToken>(
                (_, _, sort, _) => capturedSort = sort)
            .ReturnsAsync(pagedResult);

        var query = new GetUsersQuery(SortBy: UserSortColumn.Email, Direction: SortDirection.Desc);
        await _sut.Handle(query, CancellationToken.None);

        capturedSort!.SortBy.Should().Be(UserSortColumn.Email);
        capturedSort.Direction.Should().Be(SortDirection.Desc);
    }
}
