using BG3BuildPlanner.Data;
using BG3BuildPlanner.IntegrationTests.infrastructure;
using BG3BuildPlanner.Models.Dto;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Threading;

namespace BG3BuildPlanner.IntegrationTests.api;

public class RatingsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private static int _idSequence = 300000;
    private readonly CustomWebApplicationFactory _factory;

    public RatingsApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetRatings_WhenRatingsExist_ReturnsOkAndRatings()
    {
        // Arrange
        var seeded = await SeedRatingAsync("Seeded rating", 4);
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/ratings");
        var ratings = await response.Content.ReadFromJsonAsync<List<RatingDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        ratings.Should().NotBeNull();
        ratings!.Should().Contain(r => r.Id == seeded.RatingId && r.Comment == seeded.Comment);
    }

    [Fact]
    public async Task GetRatingById_WhenRatingExists_ReturnsOkAndRatingDto()
    {
        // Arrange
        var seeded = await SeedRatingAsync("Single rating", 5);
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/ratings/{seeded.RatingId}");
        var rating = await response.Content.ReadFromJsonAsync<RatingDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        rating.Should().NotBeNull();
        rating!.Id.Should().Be(seeded.RatingId);
        rating.Comment.Should().Be(seeded.Comment);
    }

    [Fact]
    public async Task GetRatingById_WhenRatingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var missingId = NextId();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/ratings/{missingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateRating_WhenRequestIsValid_ReturnsCreatedAndRatingDto()
    {
        // Arrange
        var seededBuild = await SeedBuildScenarioAsync();
        var raterUserId = await SeedUserAsync("rater-user");
        var client = CreateAuthenticatedClient(raterUserId);
        var request = new RatingCreateDto
        {
            Score = 5,
            Comment = "Great build for burst damage",
            BuildId = seededBuild.BuildId
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ratings", request);
        var rating = await response.Content.ReadFromJsonAsync<RatingDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        rating.Should().NotBeNull();
        rating!.Id.Should().BeGreaterThan(0);
        rating.Comment.Should().Be(request.Comment);
        rating.UserId.Should().Be(raterUserId);
    }

    [Fact]
    public async Task CreateRating_WhenRequestIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var raterUserId = await SeedUserAsync("invalid-rater");
        var client = CreateAuthenticatedClient(raterUserId);
        var request = new RatingCreateDto
        {
            Score = 4,
            Comment = "Build id does not exist",
            BuildId = NextId()
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ratings", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRating_WhenRatingExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedRatingAsync("Before update", 3);
        var client = CreateAuthenticatedClient(seeded.RatingUserId);
        var request = new RatingUpdateDto
        {
            Score = 5,
            Comment = "After update",
            BuildId = seeded.BuildId
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/ratings/{seeded.RatingId}", request);
        var getUpdatedResponse = await client.GetAsync($"/api/ratings/{seeded.RatingId}");
        var updatedRating = await getUpdatedResponse.Content.ReadFromJsonAsync<RatingDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getUpdatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedRating.Should().NotBeNull();
        updatedRating!.Comment.Should().Be(request.Comment);
        updatedRating.Score.Should().Be(request.Score);
    }

    [Fact]
    public async Task UpdateRating_WhenRatingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = await SeedUserAsync("missing-rating-user");
        var seededBuild = await SeedBuildScenarioAsync();
        var missingId = NextId();
        var client = CreateAuthenticatedClient(userId);
        var request = new RatingUpdateDto
        {
            Score = 4,
            Comment = "Missing rating",
            BuildId = seededBuild.BuildId
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/ratings/{missingId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRating_WhenRatingExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedRatingAsync("To delete", 2);
        var client = CreateAuthenticatedClient(seeded.RatingUserId);

        // Act
        var response = await client.DeleteAsync($"/api/ratings/{seeded.RatingId}");
        var getDeletedResponse = await client.GetAsync($"/api/ratings/{seeded.RatingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRating_WhenRatingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = await SeedUserAsync("missing-delete-user");
        var missingId = NextId();
        var client = CreateAuthenticatedClient(userId);

        // Act
        var response = await client.DeleteAsync($"/api/ratings/{missingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private HttpClient CreateAuthenticatedClient(int userId)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeaderName, userId.ToString());
        return client;
    }

    private async Task<(int BuildId, int OwnerUserId)> SeedBuildScenarioAsync()
    {
        var ownerUserId = await SeedUserAsync("build-owner");
        var characterId = NextId();
        var buildId = NextId();

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Characters.Add(new Character
            {
                Id = characterId,
                Name = "Test Character",
                Race = "Human",
                Background = "Sage",
                Level = 10,
                CreatedAt = DateTime.UtcNow
            });

            dbContext.Builds.Add(new Build
            {
                Id = buildId,
                Title = "Rated Build",
                Description = "Build used for ratings tests",
                Difficulty = Difficulty.Balanced,
                CreatedAt = DateTime.UtcNow,
                UserId = ownerUserId,
                CharacterId = characterId,
                User = null!,
                Character = null!
            });

            await dbContext.SaveChangesAsync();
        }

        return (buildId, ownerUserId);
    }

    private async Task<int> SeedUserAsync(string usernamePrefix)
    {
        var userId = NextId();
        var username = $"{usernamePrefix}-{userId}";
        var email = $"{username}@example.test";

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Users.Add(new AppUser
            {
                Id = userId,
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                CreatedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();
        }

        return userId;
    }

    private async Task<(int RatingId, int BuildId, int RatingUserId, string Comment)> SeedRatingAsync(string comment, int score)
    {
        var buildScenario = await SeedBuildScenarioAsync();
        var ratingUserId = await SeedUserAsync("rating-user");
        var ratingId = NextId();

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Ratings.Add(new Rating
            {
                Id = ratingId,
                Score = score,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                BuildId = buildScenario.BuildId,
                UserId = ratingUserId,
                Build = null!,
                User = null!
            });

            await dbContext.SaveChangesAsync();
        }

        return (ratingId, buildScenario.BuildId, ratingUserId, comment);
    }

    private static int NextId()
    {
        return Interlocked.Increment(ref _idSequence);
    }
}