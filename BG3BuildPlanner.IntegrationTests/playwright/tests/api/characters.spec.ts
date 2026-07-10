import { apiTest, expect } from '../api.base';

/**
 * API Tests for Characters Controller
 * Endpoint: /api/characters
 * 
 * Tests CRUD operations and search for character management
 */
apiTest.describe('Characters API', () => {
  
  apiTest('GET /api/characters - List all characters', async ({ apiContext, baseURL }) => {
    // Act
    const response = await apiContext.get(`${baseURL}/api/characters`);

    // Assert
    expect(response.status()).toBe(200);
    const characters = await response.json();
    expect(Array.isArray(characters.data || characters)).toBeTruthy();
  });

  apiTest('POST /api/characters - Create character', async ({ apiContext, baseURL, testUserId }) => {
    // Arrange
    const characterData = {
      name: `TestChar_${Date.now()}`,
      race: 'Human',
      class: 'Fighter',
      level: 10,
    };

    // Act
    const response = await apiContext.post(`${baseURL}/api/characters`, {
      headers: { 'X-Test-UserId': testUserId },
      data: characterData,
    });

    // Assert
    expect(response.status()).toBe(201);
    const created = await response.json();
    expect(created.name).toBe(characterData.name);
    expect(created.id).toBeDefined();
  });

  apiTest('GET /api/characters/{id} - Get single character', async ({ apiContext, baseURL, testUserId }) => {
    // Arrange: Create a character
    const createResponse = await apiContext.post(`${baseURL}/api/characters`, {
      headers: { 'X-Test-UserId': testUserId },
      data: {
        name: `GetTest_${Date.now()}`,
        race: 'Elf',
        class: 'Wizard',
        level: 8,
      },
    });
    const character = await createResponse.json();

    // Act
    const getResponse = await apiContext.get(`${baseURL}/api/characters/${character.id}`);

    // Assert
    expect(getResponse.status()).toBe(200);
    const retrieved = await getResponse.json();
    expect(retrieved.id).toBe(character.id);
    expect(retrieved.name).toBe(character.name);
  });

  apiTest('PUT /api/characters/{id} - Update character', async ({ apiContext, baseURL, testUserId }) => {
    // Arrange: Create and then update
    const createResponse = await apiContext.post(`${baseURL}/api/characters`, {
      headers: { 'X-Test-UserId': testUserId },
      data: {
        name: `UpdateTest_${Date.now()}`,
        race: 'Dwarf',
        class: 'Cleric',
        level: 5,
      },
    });
    const character = await createResponse.json();

    // Act
    const updateResponse = await apiContext.put(`${baseURL}/api/characters/${character.id}`, {
      headers: { 'X-Test-UserId': testUserId },
      data: {
        level: 12,
        name: `UpdateTest_${Date.now()}_Updated`,
      },
    });

    // Assert
    expect(updateResponse.status()).toBe(200);
    const updated = await updateResponse.json();
    expect(updated.level).toBe(12);
    expect(updated.name).toContain('Updated');
  });

  apiTest('DELETE /api/characters/{id} - Soft delete character', async ({ apiContext, baseURL, testUserId }) => {
    // Arrange: Create character to delete
    const createResponse = await apiContext.post(`${baseURL}/api/characters`, {
      headers: { 'X-Test-UserId': testUserId },
      data: {
        name: `DeleteTest_${Date.now()}`,
        race: 'Human',
        class: 'Rogue',
        level: 7,
      },
    });
    const character = await createResponse.json();

    // Act
    const deleteResponse = await apiContext.delete(`${baseURL}/api/characters/${character.id}`, {
      headers: { 'X-Test-UserId': testUserId },
    });

    // Assert
    expect(deleteResponse.status()).toBe(204);

    // Verify soft delete: item should not appear in list
    const listResponse = await apiContext.get(`${baseURL}/api/characters`);
    const characters = await listResponse.json();
    const found = (characters.data || characters).find((c: any) => c.id === character.id);
    expect(found).toBeUndefined();
  });

  apiTest('GET /api/characters/search - Search character by name', async ({ apiContext, baseURL, testUserId }) => {
    // Arrange: Create a character with unique name
    const uniqueName = `SearchTest_${Date.now()}`;
    await apiContext.post(`${baseURL}/api/characters`, {
      headers: { 'X-Test-UserId': testUserId },
      data: {
        name: uniqueName,
        race: 'Tiefling',
        class: 'Sorcerer',
        level: 9,
      },
    });

    // Act
    const searchResponse = await apiContext.get(`${baseURL}/api/characters/search?q=${uniqueName.substring(0, 6)}`);

    // Assert
    expect(searchResponse.status()).toBe(200);
    const results = await searchResponse.json();
    expect(Array.isArray(results.data || results)).toBeTruthy();
    const found = (results.data || results).some((c: any) => c.name.includes(uniqueName.substring(0, 6)));
    expect(found).toBeTruthy();
  });
});
