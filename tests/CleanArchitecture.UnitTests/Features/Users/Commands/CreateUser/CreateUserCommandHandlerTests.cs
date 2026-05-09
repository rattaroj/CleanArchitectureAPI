using CleanArchitecture.Application.Abstractions.Persistence;
using CleanArchitecture.Application.Features.Users.Commands.CreateUser;
using FluentAssertions;
using Moq;

namespace CleanArchitecture.UnitTests.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
    {
        _sut = new CreateUserCommandHandler(_userRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEmailNotTaken_ReturnsSuccess()
    {
        _userRepoMock.Setup(r => r.ExistsByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateUserCommand("John Doe", "john@example.com");
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("John Doe");
        result.Value.Email.Should().Be("john@example.com");

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<global::CleanArchitecture.Domain.Entities.User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsConflictError()
    {
        _userRepoMock.Setup(r => r.ExistsByEmailAsync("taken@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateUserCommand("Jane Doe", "taken@example.com");
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Users.EmailAlreadyExists");

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<global::CleanArchitecture.Domain.Entities.User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
