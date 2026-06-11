using BG3BuildPlanner.Data;
using BG3BuildPlanner.IntegrationTests.infrastructure;
using BG3BuildPlanner.Models.Dto;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace BG3BuildPlanner.IntegrationTests.api;

public class ItemsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private const int MissingItemId = 2147483000;
    private readonly CustomWebApplicationFactory _factory;

    public ItemsApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetItems_WhenItemsExist_ReturnsOkAndItems()
    {
        // Arrange
        var seeded = await SeedItemAsync("Arcane Staff");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/items");
        var items = await response.Content.ReadFromJsonAsync<List<ItemDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        items.Should().NotBeNull();
        items!.Should().Contain(i => i.Id == seeded.Id && i.Name == seeded.Name);
    }

    [Fact]
    public async Task GetItemById_WhenItemExists_ReturnsOkAndItemDto()
    {
        // Arrange
        var seeded = await SeedItemAsync("Infernal Blade");
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/items/{seeded.Id}");
        var item = await response.Content.ReadFromJsonAsync<ItemDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        item.Should().NotBeNull();
        item!.Id.Should().Be(seeded.Id);
        item.Name.Should().Be(seeded.Name);
    }

    [Fact]
    public async Task GetItemById_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/items/{MissingItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateItem_WhenRequestIsValid_ReturnsCreatedAndItemDto()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new ItemCreateDto
        {
            Name = "Dragon Scale Armor",
            Type = ItemType.Armor,
            Rarity = "Legendary",
            Power = 50
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/items", request);
        var item = await response.Content.ReadFromJsonAsync<ItemDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        item.Should().NotBeNull();
        item!.Id.Should().BeGreaterThan(0);
        item.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateItem_WhenRequestIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new ItemCreateDto
        {
            Name = string.Empty,
            Type = ItemType.Weapon,
            Rarity = string.Empty,
            Power = -1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/items", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateItem_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedItemAsync("Old Relic");
        var client = _factory.CreateClient();
        var request = new ItemUpdateDto
        {
            Name = "Updated Relic",
            Type = ItemType.Accessory,
            Rarity = "Epic",
            Power = 21
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/items/{seeded.Id}", request);
        var getUpdatedResponse = await client.GetAsync($"/api/items/{seeded.Id}");
        var updatedItem = await getUpdatedResponse.Content.ReadFromJsonAsync<ItemDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getUpdatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedItem.Should().NotBeNull();
        updatedItem!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new ItemUpdateDto
        {
            Name = "Missing Item",
            Type = ItemType.Weapon,
            Rarity = "Common",
            Power = 3
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/items/{MissingItemId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteItem_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var seeded = await SeedItemAsync("Disposable Dagger");
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/items/{seeded.Id}");
        var getDeletedResponse = await client.GetAsync($"/api/items/{seeded.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/items/{MissingItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<(int Id, string Name)> SeedItemAsync(string name)
    {
        Item createdItem;

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            createdItem = new Item
            {
                Name = name,
                Type = ItemType.Weapon,
                Rarity = "Rare",
                Power = 17
            };

            dbContext.Items.Add(createdItem);
            await dbContext.SaveChangesAsync();
        }

        return (createdItem.Id, name);
    }
}