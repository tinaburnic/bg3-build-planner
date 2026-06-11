using BG3BuildPlanner.Data;
using BG3BuildPlanner.IntegrationTests.infrastructure;
using BG3BuildPlanner.Models.Dto;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace BG3BuildPlanner.IntegrationTests.api;

public class SkillsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private const int MissingSkillId = 2147483000;
    private readonly CustomWebApplicationFactory _factory;

    public SkillsApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetSkills_WhenSkillsExist_ReturnsOkAndSkills()
    {
        // Arrange
        var seeded = await SeedSkillAsync("Arcane Surge");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/skills");
        var skills = await response.Content.ReadFromJsonAsync<List<SkillDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        skills.Should().NotBeNull();
        skills!.Should().Contain(s => s.Id == seeded.Id && s.Name == seeded.Name);
    }

    [Fact]
    public async Task GetSkillById_WhenSkillExists_ReturnsOkAndSkillDto()
    {
        // Arrange
        var seeded = await SeedSkillAsync("Meteor Mastery");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/skills/{seeded.Id}");
        var skill = await response.Content.ReadFromJsonAsync<SkillDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        skill.Should().NotBeNull();
        skill!.Id.Should().Be(seeded.Id);
        skill.Name.Should().Be(seeded.Name);
    }

    [Fact]
    public async Task GetSkillById_WhenSkillDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/skills/{MissingSkillId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSkill_WhenRequestIsValid_ReturnsCreatedAndSkillDto()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new SkillCreateDto
        {
            Name = "Blade Dance",
            Description = "Fast melee burst combo",
            RequiredLevel = 6,
            ImageUrl = "https://example.test/skills/blade-dance.png"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/skills", request);
        var skill = await response.Content.ReadFromJsonAsync<SkillDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        skill.Should().NotBeNull();
        skill!.Id.Should().BeGreaterThan(0);
        skill.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateSkill_WhenRequestIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new SkillCreateDto
        {
            Name = string.Empty,
            Description = string.Empty,
            RequiredLevel = 0,
            ImageUrl = "invalid-url"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/skills", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSkill_WhenSkillExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedSkillAsync("Before Update");
        var client = _factory.CreateClient();
        var request = new SkillUpdateDto
        {
            Name = "After Update",
            Description = "Updated skill description",
            RequiredLevel = 9,
            ImageUrl = "https://example.test/skills/after-update.png"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/skills/{seeded.Id}", request);
        var getUpdatedResponse = await client.GetAsync($"/api/skills/{seeded.Id}");
        var updatedSkill = await getUpdatedResponse.Content.ReadFromJsonAsync<SkillDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getUpdatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedSkill.Should().NotBeNull();
        updatedSkill!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateSkill_WhenSkillDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new SkillUpdateDto
        {
            Name = "Missing Skill",
            Description = "No skill should match",
            RequiredLevel = 4,
            ImageUrl = "https://example.test/skills/missing.png"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/skills/{MissingSkillId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSkill_WhenSkillExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedSkillAsync("To Delete");
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/skills/{seeded.Id}");
        var getDeletedResponse = await client.GetAsync($"/api/skills/{seeded.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSkill_WhenSkillDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/skills/{MissingSkillId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<(int Id, string Name)> SeedSkillAsync(string name)
    {
        Skill createdSkill;

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            createdSkill = new Skill
            {
                Name = name,
                Description = "Seeded for integration testing",
                RequiredLevel = 5,
                ImageUrl = "https://example.test/skills/seeded.png",
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Skills.Add(createdSkill);
            await dbContext.SaveChangesAsync();
        }

        return (createdSkill.Id, name);
    }
}