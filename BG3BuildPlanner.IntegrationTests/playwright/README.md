# Playwright API Tests for BG3BuildPlanner

## Quick Start

### Step 1: Navigate to the Playwright directory
```bash
cd BG3BuildPlanner.IntegrationTests/playwright
```

### Step 2: Install dependencies
```bash
npm install
```

### Step 3: Start the API server
Open a new terminal and run:
```bash
cd BG3BuildPlanner
dotnet run
```

The API server should be running at `http://localhost:5000`

### Step 4: Run Playwright tests
```bash
# Run all tests
npm test

# Run specific controller tests
npm run test:characters
npm run test:builds
npm run test:items
npm run test:skills
npm run test:ratings
npm run test:users

# Debug mode (interactive)
npm run test:debug

# With browser visible
npm run test:headed

# View HTML report
npm run report
```

---

## Project Structure

```
playwright/
├── package.json                    # NPM scripts and dependencies
├── playwright.config.ts           # Playwright configuration
├── tsconfig.json                  # TypeScript configuration
├── PLAYWRIGHT_API_TESTS_GUIDE.md  # Complete 10-step guide
├── README.md                      # This file
└── tests/
    ├── api.base.ts               # Base test class with fixtures
    ├── fixtures/
    │   ├── auth.fixture.ts       # Authentication header fixture
    │   └── data.fixture.ts       # Data seeding utilities
    └── api/
        ├── characters.spec.ts    # Characters API tests (6 tests)
        ├── builds.spec.ts        # Builds API tests (5 tests)
        ├── items.spec.ts         # Items API tests (7 tests)
        ├── skills.spec.ts        # Skills API tests (6 tests)
        ├── ratings.spec.ts       # Ratings API tests (7 tests)
        ├── users.spec.ts         # Users API tests (7 tests)
        └── profile-files.spec.ts # Profile Files API tests (4 tests)
```

---

## Test Coverage

### 7 API Controllers Tested

1. **Characters API** (`/api/characters`)
   - List, Create, Read, Update, Delete, Search
   - Soft delete validation
   - Character CRUD with validation

2. **Builds API** (`/api/builds`)
   - List, Create, Read, Update, Delete
   - Build-Character relationships
   - Soft delete tracking

3. **Items API** (`/api/items`)
   - List, Create, Read, Update, Delete, Search
   - Item type validation (Weapon, Armor, Accessory)
   - Rarity and power attributes

4. **Skills API** (`/api/skills`)
   - List, Create, Read, Update, Delete, Search
   - Skill level management
   - Soft delete with `.Active()` filter

5. **Ratings API** (`/api/ratings`) - **Authorization Required**
   - List (anonymous), Create (auth), Read (anonymous), Update (auth), Delete (auth)
   - Owner validation: Build owner cannot rate own build
   - Search ratings by comment

6. **Users API** (`/api/users`) - **ASP.NET Identity Integration**
   - List, Create, Read, Update, Delete, Search
   - Password validation
   - Soft delete for user accounts

7. **Profile Files API** (`/api/profile/files`) - **Authorization Required**
   - Upload file (jpg/png/gif/webp, max 5MB)
   - Set current profile image
   - Delete uploaded file
   - List user's uploaded files

---

## Authentication

Tests use the `X-Test-UserId` header to simulate authenticated requests:

```typescript
const response = await apiContext.get('/api/users', {
  headers: { 'X-Test-UserId': 'test-user-001' }
});
```

This header is automatically added to requests via the `authenticatedRequest` fixture.

---

## Test Patterns

### Arrange-Act-Assert
All tests follow the AAA pattern:

```typescript
apiTest('Brief test description', async ({ apiContext, testUserId }) => {
  // Arrange: Setup test data
  const createResponse = await apiContext.post('/api/characters', {
    headers: { 'X-Test-UserId': testUserId },
    data: { name: 'Test', race: 'Human', class: 'Fighter', level: 5 },
  });
  const character = await createResponse.json();

  // Act: Execute the API call
  const response = await apiContext.get(`/api/characters/${character.id}`);

  // Assert: Verify results
  expect(response.status()).toBe(200);
  const data = await response.json();
  expect(data.id).toBe(character.id);
});
```

### Using Seeding Helpers
```typescript
import { seedCharacter } from '../fixtures/data.fixture';

apiTest('Add character to build', async ({ apiContext, baseURL, testUserId }) => {
  const character = await seedCharacter(apiContext, baseURL, testUserId, 'Hero');
  expect(character.id).toBeDefined();
});
```

---

## Running Tests in CI/CD

### GitHub Actions Example
```yaml
name: Run Playwright Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: 18
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      
      - name: Install NPM dependencies
        run: npm ci --cwd BG3BuildPlanner.IntegrationTests/playwright
      
      - name: Start API server
        run: dotnet run --project BG3BuildPlanner/BG3BuildPlanner.csproj &
        env:
          ASPNETCORE_ENVIRONMENT: Testing
      
      - name: Run Playwright tests
        run: npm test --cwd BG3BuildPlanner.IntegrationTests/playwright
      
      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: playwright-report
          path: BG3BuildPlanner.IntegrationTests/playwright/playwright-report/
```

---

## Debugging Tests

### Debug Mode
Run tests interactively with step-by-step execution:
```bash
npm run test:debug
```

In the Playwright Inspector:
- Step through each action
- View network requests/responses
- Edit and re-run tests
- Inspect DOM elements

### Headed Mode
View browser activity while tests run:
```bash
npm run test:headed
```

### Generate Trace Files
Enable detailed trace collection:
```bash
npx playwright test --trace on
```

Open traces with:
```bash
npx playwright show-trace trace.zip
```

---

## Viewing Test Reports

After tests complete:
```bash
npm run report
```

Opens `playwright-report/index.html` with:
- Test execution timeline
- Failure details and screenshots
- Request/response logs
- Test duration and status
- Video recordings (if enabled)

---

## Common Issues & Solutions

### "Connection refused" error
**Problem:** Tests can't connect to API
**Solution:** Ensure API is running: `dotnet run` in the BG3BuildPlanner directory

### "404 Not Found" on endpoints
**Problem:** API endpoint doesn't exist
**Solution:** Verify `baseURL` in `playwright.config.ts` is correct (default: `http://localhost:5000`)

### "401 Unauthorized" on protected endpoints
**Problem:** Missing or invalid authentication header
**Solution:** Ensure `X-Test-UserId` header is passed for protected endpoints (Ratings, Users, Profile Files)

### Tests timeout
**Problem:** Tests take too long to complete
**Solution:** Increase timeout in individual test:
```typescript
apiTest('slow test', async ({ apiContext }) => {
  // test code
}, { timeout: 30000 }); // 30 seconds
```

---

## Next Steps

1. Copy template files from `tests/api/characters.spec.ts` to other controller directories
2. Adapt test data and endpoints for each controller
3. Run full test suite: `npm test`
4. Review HTML report: `npm run report`
5. Integrate into CI/CD pipeline
6. Consider adding performance/load tests for critical endpoints

---

## References

- [Playwright Documentation](https://playwright.dev)
- [Playwright Test API](https://playwright.dev/docs/api/class-playwrighttest)
- [BG3BuildPlanner API Controllers](../../../BG3BuildPlanner/Controllers/api/)
- [PLAYWRIGHT_API_TESTS_GUIDE.md](./PLAYWRIGHT_API_TESTS_GUIDE.md) - Complete 10-step implementation guide

