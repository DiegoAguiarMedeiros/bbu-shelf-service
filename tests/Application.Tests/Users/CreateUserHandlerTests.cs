using Application.Users.Commands.CreateUser;
using Application.Interfaces.IUserRepository;
using Domain.Users;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Users;

public class CreateUserHandlerTests
{
    private readonly Mock<IUserRepository> _shelfRepositoryMock;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _shelfRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateUserHandler(_shelfRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateUserAndReturnDto()
    {
        // Arrange
        var command = new CreateUserCommand(Guid.NewGuid(), "jane_doe", "jane@example.com");

        _shelfRepositoryMock
            .Setup(r => r.GetByPersonaNameAsync(command.PersonaName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PersonaName.Should().Be("jane_doe");
        result.CustomURL.Should().Be("jane@example.com");

        _shelfRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPersonaNameAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateUserCommand(Guid.NewGuid(), "existing_user", "test@example.com");

        var existingUser = User.Create(
            Id.New(),
            Domain.Users.ValueObjects.PersonaName.Create("existing_user"),
            Domain.Users.ValueObjects.CustomURL.Create("test@example.com"));

        _shelfRepositoryMock
            .Setup(r => r.GetByPersonaNameAsync(command.PersonaName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }
}
