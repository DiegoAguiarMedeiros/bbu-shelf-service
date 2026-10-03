using Domain.Users;
using Domain.Users.Enums;
using Domain.Users.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldInitializeUserCorrectly()
    {
        // Arrange
        var personaName = PersonaName.Create("john_doe");
        var customURL = CustomURL.Create("john@example.com");
        var id = Id.New();

        // Act
        var user = User.Create(id, personaName, customURL);

        // Assert
        user.Should().NotBeNull();
        user.Id.Value.Should().NotBeEmpty();
        user.PersonaName.Value.Should().Be("john_doe");
        user.CustomURL.Value.Should().Be("john@example.com");
        user.Status.Should().Be(UserStatus.Active);
        user.DomainEvents.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void PersonaName_WithInvalidLength_ShouldThrowArgumentException(string invalidPersonaName)
    {
        // Act
        Action action = () => PersonaName.Create(invalidPersonaName);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("invalid-customURL")]
    [InlineData("test@")]
    public void CustomURL_WithInvalidFormat_ShouldThrowArgumentException(string invalidCustomURL)
    {
        // Act
        Action action = () => CustomURL.Create(invalidCustomURL);

        // Assert
        action.Should().Throw<ArgumentException>();
    }
}
