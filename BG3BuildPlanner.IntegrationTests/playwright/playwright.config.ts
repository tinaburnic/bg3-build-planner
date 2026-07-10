import { defineConfig } from '@playwright/test';

/**
 * Playwright Configuration for BG3BuildPlanner API Tests
 * Runs against local dev server at http://localhost:5000
 */
export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env['CI'],
  retries: process.env['CI'] ? 2 : 0,
  workers: process.env['CI'] ? 1 : undefined,
  reporter: process.env['CI']
    ? [['html'], ['junit', { outputFile: 'test-results.xml' }]]
    : [['html']],
  
  use: {
    baseURL: process.env['API_URL'] || 'http://localhost:5000',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },

  webServer: {
    command: 'dotnet run --project ../../../BG3BuildPlanner/BG3BuildPlanner.csproj',
    url: 'http://localhost:5000',
    reuseExistingServer: !process.env['CI'],
  },
});
