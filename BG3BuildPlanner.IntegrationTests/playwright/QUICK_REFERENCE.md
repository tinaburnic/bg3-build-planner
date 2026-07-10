# 🚀 Playwright API Tests - Quick Reference Card

## One-Page Guide

---

## 📍 LOCATION
```
cd BG3BuildPlanner.IntegrationTests/playwright
```

---

## 🔧 COMMANDS

```bash
# Install
npm install

# Run tests
npm test                    # All tests
npm run test:characters     # Specific controller
npm run test:builds

# Debug
npm run test:debug          # Interactive (+ step through)
npm run test:headed         # Watch tests run in browser

# Reports
npm run report              # Open HTML report
npm run test:ci             # Generate JUnit XML

# CI/CD
npm run test:ci
```

---

## 📝 TEST STRUCTURE

```typescript
apiTest('Test name', async ({ apiContext, testUserId, baseURL }) => {
  // ARRANGE: Setup data
  const data = await seedCharacter(apiContext, baseURL, testUserId, 'Hero');
  
  // ACT: Call API
  const response = await apiContext.get(`${baseURL}/api/path`);
  
  // ASSERT: Verify
  expect(response.status()).toBe(200);
  const result = await response.json();
  expect(result.id).toBe(data.id);
});
```

---

## 🔐 AUTHENTICATION

```typescript
// Add X-Test-UserId header for protected endpoints
const response = await apiContext.post('/api/endpoint', {
  headers: { 'X-Test-UserId': testUserId },
  data: { /* body */ }
});
```

---

## 🎯 TEST CHECKLIST

| Step | File | Tests | Status |
|------|------|-------|--------|
| 1 | Setup | - | ✅ |
| 2 | Fixtures | - | ✅ |
| 3 | Base Config | - | ✅ |
| 4 | Characters | 6 | ✅ |
| 5 | Builds | 5 | 📋 |
| 6 | Items | 7 | 📋 |
| 7 | Skills | 6 | 📋 |
| 8 | Ratings | 7 | 📋 (auth) |
| 9 | Users | 7 | 📋 (identity) |
| 10 | Reports | - | 📋 |

---

## 📂 FILE LOCATIONS

```
DOCUMENTATION:
  SUMMARY.md ........................... Big picture overview
  PLAYGROUND_API_TESTS_GUIDE.md ........ Full 10-step guide
  IMPLEMENTATION_CHECKLIST.md .......... Track progress
  ARCHITECTURE.md ..................... Visual diagrams
  README.md ........................... Quick start

TEST FILES:
  tests/api.base.ts ................... Base class
  tests/fixtures/auth.fixture.ts ...... Auth header
  tests/fixtures/data.fixture.ts ...... Seeding helpers
  tests/api/characters.spec.ts ........ ✅ Example (6 tests)
  tests/api/builds.spec.ts ............ 📋 Implement
  tests/api/items.spec.ts ............ 📋 Implement
  tests/api/skills.spec.ts ........... 📋 Implement
  tests/api/ratings.spec.ts .......... 📋 Implement (auth)
  tests/api/users.spec.ts ............ 📋 Implement (identity)

CONFIG:
  playwright.config.ts ................ Main config
  tsconfig.json ....................... TypeScript
  package.json ........................ Scripts & deps
```

---

## ⚡ COPY-PASTE TEMPLATES

### New Test File Template
```typescript
import { apiTest, expect } from '../api.base';

apiTest.describe('ControllerName API', () => {
  
  apiTest('GET /api/path - List all', async ({ apiContext, baseURL }) => {
    const response = await apiContext.get(`${baseURL}/api/path`);
    expect(response.status()).toBe(200);
    const data = await response.json();
    expect(Array.isArray(data.data || data)).toBeTruthy();
  });

});
```

### Create Test Template
```typescript
apiTest('POST /api/path - Create item', async ({ apiContext, baseURL, testUserId }) => {
  const itemData = { name: 'Test', value: 100 };
  const response = await apiContext.post(`${baseURL}/api/path`, {
    headers: { 'X-Test-UserId': testUserId },
    data: itemData,
  });
  expect(response.status()).toBe(201);
  const created = await response.json();
  expect(created.name).toBe('Test');
});
```

### Seeding Template
```typescript
import { seedCharacter } from '../fixtures/data.fixture';

apiTest('My test', async ({ apiContext, baseURL, testUserId }) => {
  const char = await seedCharacter(apiContext, baseURL, testUserId, 'Hero');
  expect(char.id).toBeDefined();
});
```

---

## 🐛 COMMON ISSUES

| Problem | Solution |
|---------|----------|
| "Connection refused" | Run `dotnet run` in BG3BuildPlanner |
| "404 Not Found" | Check baseURL in playwright.config.ts |
| "401 Unauthorized" | Add `X-Test-UserId` header for protected endpoints |
| "Tests timeout" | Increase timeout: `{ timeout: 30000 }` |
| "Can't find module" | Run `npm install` |

---

## 📊 SUCCESS INDICATORS

**When it works:**
```
npm test
  ✓ Test 1
  ✓ Test 2
  ✓ Test 3
  
  45 passed
  Finished in 45s
```

**View report:**
```bash
npm run report
# Opens http://localhost ... in browser
```

---

## 🎓 IMPLEMENTATION SPEED

| Task | Time |
|------|------|
| Setup (Step 1-3) | 5 min |
| Characters (Step 4) | 15 min |
| Builds, Items, Skills (Step 5-7) | 45 min |
| Ratings, Users (Step 8-9) | 30 min |
| Test & Report (Step 10) | 5 min |
| **TOTAL** | **~100 min (1.5-2 hrs)** |

---

## 📚 DOCUMENTATION MAP

```
Start Here ─────────────────┐
                            │
                            ▼
                     SUMMARY.md (2 min)
                            │
                    Need quick start?
                            │
        ┌───────────────────┴───────────────────┐
        ▼                                       ▼
    README.md                    IMPLEMENTATION_CHECKLIST.md
    (5 min)                      (for tracking)
        │
    Run: npm test
        │
    Implement tests?
        │
        ▼
PLAYGROUND_API_TESTS_GUIDE.md
(detailed code examples)
        │
    Need visuals?
        │
        ▼
ARCHITECTURE.md
(diagrams & flow charts)
```

---

## 🔗 IMPORTANT LINKS

- **This file:** Quick reference card (you are here)
- **Full guide:** PLAYGROUND_API_TESTS_GUIDE.md
- **Checklist:** IMPLEMENTATION_CHECKLIST.md
- **Example:** tests/api/characters.spec.ts
- **Playwright:** https://playwright.dev

---

## ✨ KEY POINTS

1. **Use `apiTest` not `test`** - It has the special fixtures
2. **Use `${baseURL}` in paths** - Configured in playwright.config.ts
3. **Add `X-Test-UserId` header** - For authenticated endpoints
4. **Generate unique data** - Use `Date.now()` to avoid conflicts
5. **Follow AAA pattern** - Arrange, Act, Assert

---

## 🏁 QUICK START (5 minutes)

```bash
# 1. Navigate
cd BG3BuildPlanner.IntegrationTests/playwright

# 2. Install
npm install

# 3. Start API (different terminal)
cd ../../../BG3BuildPlanner
dotnet run

# 4. Run tests (first terminal)
npm test

# 5. View results
npm run report
```

---

## 📞 NEED HELP?

1. **"How do I implement X?"** → Read PLAYGROUND_API_TESTS_GUIDE.md
2. **"Am I on track?"** → Check IMPLEMENTATION_CHECKLIST.md
3. **"Why did test Y fail?"** → Run `npm run test:debug`
4. **"I'm stuck"** → Look at tests/api/characters.spec.ts (working example)
5. **"What's the big picture?"** → Read SUMMARY.md or ARCHITECTURE.md

---

## 💡 PRO TIPS

- Use `Date.now()` for unique test data: `name: \`Test_\${Date.now()}\``
- Use seeding helpers for relationships: `const char = await seedCharacter(...)`
- For flexible assertions: `expect([400, 403]).toContain(status)`
- Watch tests run: `npm run test:headed`
- Debug interactively: `npm run test:debug`

---

## 🎊 NEXT STEPS

✅ I've prepared everything for you:
- Playwright configured and ready
- 6 working tests (Characters API)
- Templates for all other controllers
- Complete documentation

👉 **Your turn:**
1. Run `npm test` to see 6 tests pass
2. Copy characters.spec.ts to builds.spec.ts
3. Implement 5 build tests (~15 min)
4. Repeat for other controllers
5. Run full suite: `npm test`

**Happy testing!** 🚀

---

## Print This Card! 📌

Keep this on your desk while implementing. It has:
- All commands you need
- Test file structure template
- Common solutions
- Progress checklist
- Documentation roadmap

```
Save as: ~/Desktop/playwright-quick-ref.md
or Print this page
```

---

*Last Updated: 2026-07-10*
*Playwright Version: ^1.40.0*
*BG3BuildPlanner API Controllers: 7*
*Total Tests: 45+*
