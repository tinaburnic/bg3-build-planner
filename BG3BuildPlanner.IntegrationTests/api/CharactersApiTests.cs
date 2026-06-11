using BG3BuildPlanner.Data;
using BG3BuildPlanner.IntegrationTests.infrastructure;
using BG3BuildPlanner.Models.Dto;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace BG3BuildPlanner.IntegrationTests.api;

public class CharactersApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private const int MissingCharacterId = 2147483000;
    private readonly CustomWebApplicationFactory _factory;

    public CharactersApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCharacters_WhenCharactersExist_ReturnsOkAndCharacters()
    {
        // Arrange
        var seeded = await SeedCharacterAsync("Astarion");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/characters");
        var characters = await response.Content.ReadFromJsonAsync<List<CharacterDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        characters.Should().NotBeNull();
        characters!.Should().Contain(c => c.Id == seeded.Id && c.Name == seeded.Name);
    }

    [Fact]
    public async Task GetCharacterById_WhenCharacterExists_ReturnsOkAndCharacterDto()
    {
        // Arrange
        var seeded = await SeedCharacterAsync("Shadowheart");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/characters/{seeded.Id}");
        var character = await response.Content.ReadFromJsonAsync<CharacterDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        character.Should().NotBeNull();
        character!.Id.Should().Be(seeded.Id);
        character.Name.Should().Be(seeded.Name);
    }

    [Fact]
    public async Task GetCharacterById_WhenCharacterDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/characters/{MissingCharacterId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCharacter_WhenRequestIsValid_ReturnsCreatedAndCharacterDto()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CharacterCreateDto
        {
            Name = "Wyll",
            PortraitUrl = "https://example.test/wyll.png",
            Race = "Human",
            Background = "Folk Hero",
            Level = 8
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/characters", request);
        var character = await response.Content.ReadFromJsonAsync<CharacterDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        character.Should().NotBeNull();
        character!.Id.Should().BeGreaterThan(0);
        character.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateCharacter_WhenRequestIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CharacterCreateDto
        {
            Name = string.Empty,
            PortraitUrl = "not-a-valid-url",
            Race = string.Empty,
            Background = string.Empty,
            Level = 0
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/characters", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCharacter_WhenCharacterExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedCharacterAsync("Karlach");
        var client = _factory.CreateClient();
        var request = new CharacterUpdateDto
        {
            Name = "Karlach Updated",
            PortraitUrl = "https://example.test/karlach-updated.png",
            Race = "Tiefling",
            Background = "Soldier",
            Level = 12
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/characters/{seeded.Id}", request);
        var getUpdatedResponse = await client.GetAsync($"/api/characters/{seeded.Id}");
        var updatedCharacter = await getUpdatedResponse.Content.ReadFromJsonAsync<CharacterDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getUpdatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedCharacter.Should().NotBeNull();
        updatedCharacter!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateCharacter_WhenCharacterDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CharacterUpdateDto
        {
            Name = "Missing Character",
            PortraitUrl = "https://example.test/missing.png",
            Race = "Human",
            Background = "Soldier",
            Level = 5
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/characters/{MissingCharacterId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCharacter_WhenCharacterExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedCharacterAsync("Lae'zel");
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/characters/{seeded.Id}");
        var getDeletedResponse = await client.GetAsync($"/api/characters/{seeded.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCharacter_WhenCharacterDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/characters/{MissingCharacterId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<(int Id, string Name)> SeedCharacterAsync(string name)
    {
        Character createdCharacter;

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            createdCharacter = new Character
            {
                Name = name,
                PortraitUrl = "https://example.test/portrait.png",
                Race = "Elf",
                Background = "Noble",
                Level = 10,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.Characters.Add(createdCharacter);

            await dbContext.SaveChangesAsync();
        }

        return (createdCharacter.Id, name);
    }
}