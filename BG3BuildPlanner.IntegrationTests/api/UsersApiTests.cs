using BG3BuildPlanner.Data;
using BG3BuildPlanner.IntegrationTests.infrastructure;
using BG3BuildPlanner.Models.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Threading;

namespace BG3BuildPlanner.IntegrationTests.api;

public class UsersApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private static int _idSequence = 400000;
    private readonly CustomWebApplicationFactory _factory;

    public UsersApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUsers_WhenUsersExist_ReturnsOkAndUsers()
    {
        // Arrange
        var seeded = await SeedUserAsync("list-user");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users");
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        users.Should().NotBeNull();
        users!.Should().Contain(u => u.Id == seeded.Id && u.Username == seeded.Username);
    }

    [Fact]
    public async Task GetUserById_WhenUserExists_ReturnsOkAndUserDto()
    {
        // Arrange
        var seeded = await SeedUserAsync("single-user");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/users/{seeded.Id}");
        var user = await response.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        user.Should().NotBeNull();
        user!.Id.Should().Be(seeded.Id);
        user.Username.Should().Be(seeded.Username);
    }

    [Fact]
    public async Task GetUserById_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var missingId = NextId();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/users/{missingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateUser_WhenRequestIsValid_ReturnsCreatedAndUserDto()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new UserCreateDto
        {
            Username = $"created-user-{NextId()}",
            Email = $"created-user-{NextId()}@example.test",
            Password = "Valid1!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/users", request);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        user.Should().NotBeNull();
        user!.Id.Should().BeGreaterThan(0);
        user.Username.Should().Be(request.Username);
        user.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task CreateUser_WhenRequestIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new UserCreateDto
        {
            Username = string.Empty,
            Email = "not-an-email",
            Password = string.Empty
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_WhenUserExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedUserAsync("before-update-user");
        var client = _factory.CreateClient();
        var request = new UserUpdateDto
        {
            Username = $"updated-user-{NextId()}",
            Email = $"updated-user-{NextId()}@example.test",
            Password = "Updated1!"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/users/{seeded.Id}", request);
        var getUpdatedResponse = await client.GetAsync($"/api/users/{seeded.Id}");
        var updatedUser = await getUpdatedResponse.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getUpdatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedUser.Should().NotBeNull();
        updatedUser!.Username.Should().Be(request.Username);
        updatedUser.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task UpdateUser_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var missingId = NextId();
        var client = _factory.CreateClient();
        var request = new UserUpdateDto
        {
            Username = "missing-user",
            Email = "missing-user@example.test",
            Password = "Updated1!"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/users/{missingId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedUserAsync("to-delete-user");
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/users/{seeded.Id}");
        var getDeletedResponse = await client.GetAsync($"/api/users/{seeded.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var missingId = NextId();
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/users/{missingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<(int Id, string Username, string Email)> SeedUserAsync(string usernamePrefix)
    {
        var username = $"{usernamePrefix}-{NextId()}";
        var email = $"{username}@example.test";

        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            var user = new AppUser
            {
                UserName = username,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(user, "Seeded1!");
            createResult.Succeeded.Should().BeTrue();

            return (user.Id, username, email);
        }
    }

    private static int NextId()
    {
        return Interlocked.Increment(ref _idSequence);
    }
}