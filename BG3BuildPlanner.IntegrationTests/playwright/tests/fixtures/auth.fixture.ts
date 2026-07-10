import { test as base } from '@playwright/test';

/**
 * Fixture: Adds custom authentication header support
 * Uses X-Test-UserId header to simulate authenticated requests
 */
export const test = base.extend<{
  authenticatedRequest: (url: string, userId: string, options?: any) => Promise<any>,
}>({
  authenticatedRequest: async ({ request }, use) => {
    const authenticatedRequest = async (url: string, userId: string, options: any = {}) => {
      const headers = {
        'X-Test-UserId': userId,
        'Content-Type': 'application/json',
        ...options.headers,
      };
      
      return request.fetch(url, {
        ...options,
        headers,
      });
    };

    await use(authenticatedRequest);
  },
});

export { expect } from '@playwright/test';
