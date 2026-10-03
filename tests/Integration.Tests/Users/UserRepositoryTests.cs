using Domain.Users;
using Domain.Users.ValueObjects;
using FluentAssertions;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Integration.Tests.Users;

public class UserRepositoryTests
{
    private readonly UserDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var user = User.Create(Id.New(), PersonaName.Create("repo_test"), CustomURL.Create("repo@test.com"));

        // Act
        await _repository.AddAsync(user);

        // Assert
        var fetchedUser = await _repository.GetByIdAsync(user.Id);
        fetchedUser.Should().NotBeNull();
        fetchedUser!.PersonaName.Value.Should().Be("repo_test");
    }
}
