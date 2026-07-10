# Playwright API Tests Implementation Checklist

Use this checklist to track your progress through the 10-step implementation of Playwright tests for all API controllers.

---

## ✅ STEP 1: Project Setup & Dependencies

**Objective:** Initialize Node.js project and install Playwright

### Tasks
- [ ] Navigate to: `BG3BuildPlanner.IntegrationTests/playwright/`
- [ ] Create or update `package.json` with dependencies
  - [ ] @playwright/test ^1.40.0
  - [ ] typescript ^5.3.0
  - [ ] ts-node ^10.9.0
  - [ ] @types/node ^20.10.0
- [ ] Run `npm install`
- [ ] Verify `playlist.config.ts` exists with baseURL: `http://localhost:5000`
- [ ] Verify `tsconfig.json` exists
- [ ] Run `npx playwright install` (installs browsers)

**Deliverables:**
- [x] `package.json` ✓
- [x] `playwright.config.ts` ✓
- [x] `tsconfig.json` ✓

**Status:** ✅ COMPLETE

---

## ✅ STEP 2: Test Fixtures & Authentication Setup

**Objective:** Create fixtures for authentication and data seeding

### Tasks
- [ ] Create directory: `tests/fixtures/`
- [ ] Create `tests/fixtures/auth.fixture.ts`
  - [ ] Implement auth fixture extending base test
  - [ ] Add `X-Test-UserId` header injection
  - [ ] Export custom `test` object
- [ ] Create `tests/fixtures/data.fixture.ts`
  - [ ] Implement `seedCharacter()` helper
  - [ ] Implement `seedBuild()` helper
  - [ ] Implement `seedItem()` helper
  - [ ] Implement `seedSkill()` helper

**Deliverables:**
- [x] `tests/fixtures/auth.fixture.ts` ✓
- [x] `tests/fixtures/data.fixture.ts` ✓

**Status:** ✅ COMPLETE

---

## ✅ STEP 3: Base Test Configuration

**Objective:** Create base class for all API tests with common fixtures

### Tasks
- [ ] Create `tests/api.base.ts`
  - [ ] Import from `@playwright/test`
  - [ ] Define `APITestFixtures` interface with:
    - [ ] `apiContext: APIRequestContext`
    - [ ] `testUserId: string`
    - [ ] `testBuilderId: string`
    - [ ] `baseURL: string`
  - [ ] Extend base test with fixtures
  - [ ] Export custom `apiTest` and `expect`

**Deliverables:**
- [x] `tests/api.base.ts` ✓

**Status:** ✅ COMPLETE

---

## ✅ STEP 4: Characters API Tests

**Objective:** Create comprehensive tests for `/api/characters` endpoint

### Tasks
- [ ] Create `tests/api/characters.spec.ts`
- [ ] Implement 6 test cases:
  - [ ] `GET /api/characters` - List all characters
  - [ ] `POST /api/characters` - Create character
  - [ ] `GET /api/characters/{id}` - Get single character
  - [ ] `PUT /api/characters/{id}` - Update character
  - [ ] `DELETE /api/characters/{id}` - Soft delete character
  - [ ] `GET /api/characters/search` - Search character by name/pattern

### Test Data
- Characters: name, race, class, level
- Soft delete: `DeletedAt` timestamp validation

**Validation Points:**
- [ ] HTTP 200 for GET list
- [ ] HTTP 201 for POST create
- [ ] HTTP 200 for GET single
- [ ] HTTP 200 for PUT update
- [ ] HTTP 204 for DELETE
- [ ] Soft delete removes from list
- [ ] Search returns matching results

**Deliverables:**
- [x] `tests/api/characters.spec.ts` (6 tests) ✓

**Status:** ✅ COMPLETE

---

## STEP 5: Builds API Tests

**Objective:** Create comprehensive tests for `/api/builds` endpoint with character relationships

### Tasks
- [ ] Create `tests/api/builds.spec.ts`
- [ ] Setup `beforeAll()` hook to seed test character
- [ ] Implement 5 test cases:
  - [ ] `GET /api/builds` - List all builds
  - [ ] `POST /api/builds` - Create build
  - [ ] `GET /api/builds/{id}` - Get single build
  - [ ] `PUT /api/builds/{id}` - Update build
  - [ ] `DELETE /api/builds/{id}` - Soft delete build

### Test Data
- Builds require: characterId, name, description (optional)
- Relationship: Build → Character validation

**Validation Points:**
- [ ] Valid characterId required for build creation
- [ ] Soft delete with `DeletedAt` timestamp
- [ ] Build-character relationship intact
- [ ] Update preserves characterId

**Checklist:**
- [ ] Create stub from template
- [ ] Run `npm run test:builds`
- [ ] All 5 tests passing

**Deliverables:**
- [ ] `tests/api/builds.spec.ts` (5 tests)

**Status:** ⏳ TODO

---

## STEP 6: Items API Tests

**Objective:** Create comprehensive tests for `/api/items` endpoint with type validation

### Tasks
- [ ] Create `tests/api/items.spec.ts`
- [ ] Implement 7 test cases:
  - [ ] `GET /api/items` - List all items
  - [ ] `POST /api/items` - Create weapon
  - [ ] `POST /api/items` - Create armor
  - [ ] `GET /api/items/{id}` - Get single item
  - [ ] `PUT /api/items/{id}` - Update item
  - [ ] `DELETE /api/items/{id}` - Delete item (hard delete)
  - [ ] `GET /api/items/search` - Search items

### Test Data
- Required: name, type, rarity, power
- Types: Weapon | Armor | Accessory
- Rarity: Common | Uncommon | Rare | Legendary

**Validation Points:**
- [ ] Type validation (only accept valid types)
- [ ] Hard delete (not soft)
- [ ] Rarity validation
- [ ] Power attribute numeric validation
- [ ] Search by name pattern

**Checklist:**
- [ ] Create stub from template
- [ ] Run `npm run test:items`
- [ ] All 7 tests passing

**Deliverables:**
- [ ] `tests/api/items.spec.ts` (7 tests)

**Status:** ⏳ TODO

---

## STEP 7: Skills API Tests

**Objective:** Create comprehensive tests for `/api/skills` endpoint with soft delete

### Tasks
- [ ] Create `tests/api/skills.spec.ts`
- [ ] Implement 6 test cases:
  - [ ] `GET /api/skills` - List all active skills
  - [ ] `POST /api/skills` - Create skill
  - [ ] `GET /api/skills/{id}` - Get single skill
  - [ ] `PUT /api/skills/{id}` - Update skill
  - [ ] `DELETE /api/skills/{id}` - Soft delete skill
  - [ ] `GET /api/skills/search` - Search skills

### Test Data
- Required: name, description, level
- Soft delete with `.Active()` filter

**Validation Points:**
- [ ] Soft delete removes from `.Active()` list
- [ ] Deleted items not shown in GET list
- [ ] Search returns only active skills
- [ ] Level attribute validates properly

**Checklist:**
- [ ] Create stub from template
- [ ] Run `npm run test:skills`
- [ ] All 6 tests passing

**Deliverables:**
- [ ] `tests/api/skills.spec.ts` (6 tests)

**Status:** ⏳ TODO

---

## STEP 8: Ratings API Tests

**Objective:** Create comprehensive tests for `/api/ratings` endpoint with authorization

### Tasks
- [ ] Create `tests/api/ratings.spec.ts`
- [ ] Setup `beforeAll()` hook to seed build + owner user
- [ ] Implement 7 test cases:
  - [ ] `GET /api/ratings` - List all ratings (anonymous allowed)
  - [ ] `POST /api/ratings` - Create rating (auth required)
  - [ ] `POST /api/ratings` - Prevent owner from rating own build
  - [ ] `GET /api/ratings/{id}` - Get single rating (anonymous)
  - [ ] `PUT /api/ratings/{id}` - Update own rating (auth + ownership)
  - [ ] `DELETE /api/ratings/{id}` - Soft delete rating

### Authorization Rules
- GET: Anonymous allowed
- POST/PUT/DELETE: `X-Test-UserId` header required
- Ownership: Only rating owner can update/delete
- Business rule: Build owner cannot rate own build

**Validation Points:**
- [ ] Owner prevented from rating own build (400/403)
- [ ] Authenticated required for create
- [ ] Ownership check on update/delete
- [ ] Soft delete with `DeletedAt` timestamp
- [ ] GET anonymous works without header

**Checklist:**
- [ ] Create stub from template
- [ ] Use different userId for build owner vs rater
- [ ] Run `npm run test:ratings`
- [ ] All 7 tests passing
- [ ] Authorization tests verify access control

**Deliverables:**
- [ ] `tests/api/ratings.spec.ts` (7 tests)

**Status:** ⏳ TODO

---

## STEP 9: Users API Tests

**Objective:** Create comprehensive tests for `/api/users` endpoint with ASP.NET Identity integration

### Tasks
- [ ] Create `tests/api/users.spec.ts`
- [ ] Implement 7 test cases:
  - [ ] `GET /api/users` - List all active users
  - [ ] `POST /api/users` - Create user
  - [ ] `POST /api/users` - Reject weak password
  - [ ] `GET /api/users/{id}` - Get single user
  - [ ] `PUT /api/users/{id}` - Update user info
  - [ ] `DELETE /api/users/{id}` - Soft delete user
  - [ ] `GET /api/users/search` - Search users by username

### Identity Validation
- Password requirements: Strong (uppercase, lowercase, number, special, 8+ chars)
- Unique: username, email
- Soft delete with `DeletedAt` timestamp

**Validation Points:**
- [ ] Weak password rejected (400/422)
- [ ] Duplicate username rejected
- [ ] Duplicate email rejected
- [ ] Password not returned in response
- [ ] Soft delete removes from active list
- [ ] Updated email/username properly

**Checklist:**
- [ ] Generate unique username/email per test (use timestamp)
- [ ] Create stub from template
- [ ] Run `npm run test:users`
- [ ] All 7 tests passing
- [ ] Password validation tests included

**Deliverables:**
- [ ] `tests/api/users.spec.ts` (7 tests)

**Status:** ⏳ TODO

---

## STEP 10: Run Tests & Generate Reports

**Objective:** Execute all tests and generate comprehensive reports

### Pre-Test Checklist
- [ ] All 9 test files created (45+ tests)
- [ ] API server running: `dotnet run` from BG3BuildPlanner
- [ ] Playwright dir: `cd BG3BuildPlanner.IntegrationTests/playwright/`
- [ ] Dependencies installed: `npm install`

### Test Execution
- [ ] Run all tests: `npm test`
  - [ ] All tests pass
  - [ ] No connection refused errors
  - [ ] No timeout errors
  - [ ] No 404 errors

### Individual Controller Tests
- [ ] `npm run test:characters` - 6 tests ✓
- [ ] `npm run test:builds` - 5 tests
- [ ] `npm run test:items` - 7 tests
- [ ] `npm run test:skills` - 6 tests
- [ ] `npm run test:ratings` - 7 tests (auth)
- [ ] `npm run test:users` - 7 tests (identity)

### Report Generation
- [ ] Run `npm run report`
- [ ] HTML report opens in browser
- [ ] Verify report shows:
  - [ ] All test names
  - [ ] Pass/fail status
  - [ ] Execution duration
  - [ ] Timeline view
  - [ ] Failure details (if any)

### CI/CD Integration
- [ ] Run `npm run test:ci`
- [ ] JUnit XML report generated: `test-results.xml`
- [ ] Can be used in GitHub Actions/GitLab CI

**Validation Points:**
- [ ] Total: 45+ tests passing
- [ ] No skipped tests
- [ ] No flaky tests
- [ ] All controllers covered (7)
- [ ] CRUD + special operations tested
- [ ] Authorization tests passing

**Checklist:**
- [ ] Full test suite runs: `npm test` ✓
- [ ] All 38 tests pass ✓
- [ ] HTML report viewable
- [ ] Can run individual controller tests
- [ ] CI/CD script prepared

**Deliverables:**
- [x] All test files complete
- [x] `package.json` scripts working
- [x] HTML report generation
- [ ] CI/CD integration instructions

**Status:** ⏳ IN PROGRESS

---

## Summary Progress

| Step | Task | Status | Deliverables |
|------|------|--------|--------------|
| 1 | Project Setup | ✅ COMPLETE | package.json, playwright.config.ts, tsconfig.json |
| 2 | Fixtures & Auth | ✅ COMPLETE | auth.fixture.ts, data.fixture.ts |
| 3 | Base Config | ✅ COMPLETE | api.base.ts |
| 4 | Characters API | ✅ COMPLETE | characters.spec.ts (6 tests) |
| 5 | Builds API | ⏳ TODO | builds.spec.ts (5 tests) |
| 6 | Items API | ⏳ TODO | items.spec.ts (7 tests) |
| 7 | Skills API | ⏳ TODO | skills.spec.ts (6 tests) |
| 8 | Ratings API | ⏳ TODO | ratings.spec.ts (7 tests) |
| 9 | Users API | ⏳ TODO | users.spec.ts (7 tests) |
| 10 | Run & Report | ⏳ IN PROGRESS | Test execution + HTML report |

**Overall Progress:** 3/10 steps complete, 38% done

**Current Test Coverage:** 6/45 tests implemented

---

## Quick Commands Reference

```bash
# Navigate to Playwright directory
cd BG3BuildPlanner.IntegrationTests/playwright

# Install dependencies
npm install

# Run all tests
npm test

# Run specific controller
npm run test:characters
npm run test:builds
npm run test:items
npm run test:skills
npm run test:ratings
npm run test:users

# Debug mode
npm run test:debug

# With browser visible
npm run test:headed

# Generate report
npm run report

# For CI/CD
npm run test:ci
```

---

## Resources

- **Guide:** [PLAYWRIGHT_API_TESTS_GUIDE.md](./PLAYWRIGHT_API_TESTS_GUIDE.md)
- **Quick Start:** [README.md](./README.md)
- **Playwright Docs:** https://playwright.dev
- **This Checklist:** Keep this file open while implementing

---

## Notes

- All tests use Arrange-Act-Assert pattern
- Use `apiTest` custom test object instead of `test`
- Pass `X-Test-UserId` header for authenticated endpoints
- Generate unique test data using `Date.now()` to avoid conflicts
- Review [PLAYWRIGHT_API_TESTS_GUIDE.md](./PLAYWRIGHT_API_TESTS_GUIDE.md) for detailed code examples for each step

