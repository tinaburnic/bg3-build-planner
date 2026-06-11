// Integration tests for Builds API.
// Use WebApplicationFactory and HttpClient.
// Use the real API endpoint.
// Use EF Core InMemory.
// First implement GET /api/builds/{id} when build exists.

using BG3BuildPlanner.Data;
using BG3BuildPlanner.IntegrationTests.infrastructure;
using BG3BuildPlanner.Models.Dto;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Threading;

namespace BG3BuildPlanner.IntegrationTests.api;

public class BuildApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private static int _idSequence = 100000;
    private readonly CustomWebApplicationFactory _factory;

    public BuildApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetBuilds_WhenBuildsExist_ReturnsOkAndBuilds()
    {
        // Arrange
        var seeded = await SeedBuildAsync("List Build");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/builds");
        var builds = await response.Content.ReadFromJsonAsync<List<BuildDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        builds.Should().NotBeNull();
        builds!.Should().Contain(b => b.Id == seeded.Id && b.Title == seeded.Title);
    }

    [Fact]
    public async Task GetBuildById_WhenBuildExists_ReturnsOkAndBuildDto()
    {
        // Arrange
        var seeded = await SeedBuildAsync("Sorcerer Burst Build");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/builds/{seeded.Id}");
        var buildDto = await response.Content.ReadFromJsonAsync<BuildDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        buildDto.Should().NotBeNull();
        buildDto!.Id.Should().Be(seeded.Id);
        buildDto.Title.Should().Be(seeded.Title);
    }

    [Fact]
    public async Task GetBuildById_WhenBuildDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var missingId = NextId();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/builds/{missingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateBuild_WhenRequestIsValid_ReturnsCreatedAndBuildDto()
    {
        // Arrange
        var seededRefs = await SeedUserAndCharacterAsync();
        var client = _factory.CreateClient();
        var request = new BuildCreateDto
        {
            Title = "Posted Build",
            Description = "Created via integration test",
            Difficulty = Difficulty.Balanced,
            CharacterId = seededRefs.CharacterId,
            UserId = seededRefs.UserId
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/builds", request);
        var buildDto = await response.Content.ReadFromJsonAsync<BuildDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        buildDto.Should().NotBeNull();
        buildDto!.Id.Should().BeGreaterThan(0);
        buildDto.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task CreateBuild_WhenRequestIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new BuildCreateDto
        {
            Title = "Invalid Build",
            Description = "Character and user do not exist",
            Difficulty = Difficulty.Explorer,
            CharacterId = NextId(),
            UserId = NextId()
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/builds", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateBuild_WhenBuildExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedBuildAsync("Build Before Update");
        var client = _factory.CreateClient();
        var request = new BuildUpdateDto
        {
            Title = "Build After Update",
            Description = "Updated via integration test",
            Difficulty = Difficulty.Tactician,
            CharacterId = seeded.CharacterId,
            UserId = seeded.UserId
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/builds/{seeded.Id}", request);
        var getUpdatedResponse = await client.GetAsync($"/api/builds/{seeded.Id}");
        var updatedDto = await getUpdatedResponse.Content.ReadFromJsonAsync<BuildDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getUpdatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedDto.Should().NotBeNull();
        updatedDto!.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task UpdateBuild_WhenBuildDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var missingId = NextId();
        var client = _factory.CreateClient();
        var request = new BuildUpdateDto
        {
            Title = "No Build",
            Description = "Missing build",
            Difficulty = Difficulty.Balanced,
            CharacterId = NextId(),
            UserId = NextId()
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/builds/{missingId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBuild_WhenBuildExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedBuildAsync("Build To Delete");
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/builds/{seeded.Id}");
        var getDeletedResponse = await client.GetAsync($"/api/builds/{seeded.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBuild_WhenBuildDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var missingId = NextId();
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/builds/{missingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<(int UserId, int CharacterId)> SeedUserAndCharacterAsync()
    {
        var userId = NextId();
        var characterId = NextId();

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Users.Add(new AppUser
            {
                Id = userId,
                UserName = "integration-user",
                NormalizedUserName = "INTEGRATION-USER",
                Email = "integration@example.test",
                NormalizedEmail = "INTEGRATION@EXAMPLE.TEST",
                CreatedAt = DateTime.UtcNow
            });

            dbContext.Characters.Add(new Character
            {
                Id = characterId,
                Name = "Gale",
                Race = "Human",
                Background = "Sage",
                Level = 12,
                CreatedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();
        }

        return (userId, characterId);
    }

    private async Task<(int Id, string Title, int UserId, int CharacterId)> SeedBuildAsync(string title)
    {
        var buildId = NextId();
        var refs = await SeedUserAndCharacterAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Builds.Add(new Build
            {
                Id = buildId,
                Title = title,
                Description = "High damage single-target setup",
                Difficulty = Difficulty.Tactician,
                CreatedAt = DateTime.UtcNow,
                UserId = refs.UserId,
                CharacterId = refs.CharacterId,
                User = null!,
                Character = null!
            });

            await dbContext.SaveChangesAsync();
        }

        return (buildId, title, refs.UserId, refs.CharacterId);
    }

    private static int NextId()
    {
        return Interlocked.Increment(ref _idSequence);
    }
}