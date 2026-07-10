import type { APIRequestContext } from '@playwright/test';

/**
 * Helper functions for seeding test data
 */

export async function seedCharacter(
  request: APIRequestContext,
  baseURL: string,
  userId: string,
  name: string,
) {
  const response = await request.post(`${baseURL}/api/characters`, {
    headers: { 'X-Test-UserId': userId },
    data: {
      name: name,
      race: 'Human',
      class: 'Fighter',
      level: 5,
    },
  });

  if (!response.ok()) {
    throw new Error(`Failed to seed character: ${response.status()}`);
  }

  return await response.json();
}

export async function seedBuild(
  request: APIRequestContext,
  baseURL: string,
  userId: string,
  characterId: string,
  name: string,
) {
  const response = await request.post(`${baseURL}/api/builds`, {
    headers: { 'X-Test-UserId': userId },
    data: {
      name: name,
      characterId: characterId,
      description: 'Test build',
    },
  });

  if (!response.ok()) {
    throw new Error(`Failed to seed build: ${response.status()}`);
  }

  return await response.json();
}

export async function seedItem(
  request: APIRequestContext,
  baseURL: string,
  userId: string,
  name: string,
  type: 'Weapon' | 'Armor' | 'Accessory',
) {
  const response = await request.post(`${baseURL}/api/items`, {
    headers: { 'X-Test-UserId': userId },
    data: {
      name: name,
      type: type,
      rarity: 'Common',
      power: 10,
    },
  });

  if (!response.ok()) {
    throw new Error(`Failed to seed item: ${response.status()}`);
  }

  return await response.json();
}

export async function seedSkill(
  request: APIRequestContext,
  baseURL: string,
  userId: string,
  name: string,
) {
  const response = await request.post(`${baseURL}/api/skills`, {
    headers: { 'X-Test-UserId': userId },
    data: {
      name: name,
      description: `Test skill for ${name}`,
      level: 1,
    },
  });

  if (!response.ok()) {
    throw new Error(`Failed to seed skill: ${response.status()}`);
  }

  return await response.json();
}
