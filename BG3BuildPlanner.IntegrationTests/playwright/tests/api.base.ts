import { test as base, APIRequestContext, expect as baseExpect } from '@playwright/test';

/**
 * Base test fixture with common API testing utilities
 */

interface APITestFixtures {
  apiContext: APIRequestContext;
  testUserId: string;
  testBuilderId: string;
  baseURL: string;
}

export const apiTest = base.extend<APITestFixtures>({
  apiContext: async ({ request }, use) => {
    await use(request);
  },

  testUserId: async ({}, use) => {
    await use('test-user-001');
  },

  testBuilderId: async ({}, use) => {
    await use('test-builder-002');
  },

  baseURL: async ({ baseURL }, use) => {
    await use(baseURL || 'http://localhost:5000');
  },
});

export const expect = baseExpect;
