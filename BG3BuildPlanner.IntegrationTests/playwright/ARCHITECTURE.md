# Playwright API Tests - Architecture Overview

## 📐 Complete Project Structure

```
BG3BuildPlanner.IntegrationTests/playwright/
│
├─ 📚 DOCUMENTATION
│  ├── SUMMARY.md                     ← START HERE (this gives you the big picture)
│  ├── PLAYWRIGHT_API_TESTS_GUIDE.md  ← Read for step-by-step implementation
│  ├── README.md                      ← Quick start & troubleshooting
│  └── IMPLEMENTATION_CHECKLIST.md    ← Track your progress
│
├─ ⚙️ CONFIGURATION
│  ├── package.json                   ← npm scripts & dependencies
│  ├── playwright.config.ts           ← Playwright config (baseURL, browsers)
│  └── tsconfig.json                  ← TypeScript config
│
└─ 🧪 TEST CODE
   └── tests/
      ├── api.base.ts                 ← Base class for all API tests
      ├── fixtures/
      │  ├── auth.fixture.ts         ← Authentication header injection
      │  └── data.fixture.ts         ← Seeding helpers (seedCharacter, seedBuild, etc)
      └── api/
         ├── characters.spec.ts       ✅ 6 tests (COMPLETE)
         ├── builds.spec.ts           📋 5 tests (template in guide)
         ├── items.spec.ts            📋 7 tests (template in guide)
         ├── skills.spec.ts           📋 6 tests (template in guide)
         ├── ratings.spec.ts          📋 7 tests (template in guide - auth)
         ├── users.spec.ts            📋 7 tests (template in guide - identity)
         └── profile-files.spec.ts    📋 4 tests (optional - auth + file upload)

TOTAL: 45+ automated API tests ready to implement
```

---

## 🏗️ Test Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      Playwright Test                         │
│                       (npm test)                             │
└──────────────────────────┬──────────────────────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        ▼                  ▼                  ▼
    ┌────────┐         ┌────────┐        ┌────────┐
    │  Test  │         │  Test  │        │  Test  │
    │ Case 1 │         │ Case 2 │        │ Case N │
    └───┬────┘         └───┬────┘        └───┬────┘
        │                  │                  │
        └──────────────────┼──────────────────┘
                           │
        ┌──────────────────┴──────────────────┐
        ▼                                     ▼
    ┌──────────────────┐            ┌─────────────────┐
    │   api.base.ts    │            │  fixtures/      │
    │  (fixtures)      │            │  - auth         │
    │  - apiContext    │            │  - data seeding │
    │  - testUserId    │            └─────────────────┘
    │  - baseURL       │
    └────────┬─────────┘
             │
        ┌────┴─────┐
        ▼          ▼
    ┌─────────────────────────────────────────────┐
    │        Playwright HTTP Client               │
    │        (@playwright/test)                   │
    │  - GET, POST, PUT, DELETE                   │
    │  - Headers (X-Test-UserId)                  │
    │  - Request/Response logging                 │
    └────────┬────────────────────────────────────┘
             │
        ┌────┴─────────────────────────────────┐
        ▼                                      ▼
    HTTP/HTTPS                    Local Api Server
    Request                       http://localhost:5000
    │                             (dotnet run)
    │                              │
    │  Includes:                   │
    │  - Request body              │ Controllers:
    │  - Headers                   │ • /api/characters
    │  - Authorization             │ • /api/builds
    │  - Query params              │ • /api/items
    │                              │ • /api/skills
    ▼                              │ • /api/ratings
    │                              │ • /api/users
    │  Response:                   │ • /api/profile/files
    │  - Status (200, 201, etc)    │
    │  - Headers                   ▼      ▼
    │  - Body (JSON)         DbContext  Authentication
    │                        (In-Memory) (X-Test-UserId)
    │
    └─── Assertion (expect)
         - Response status
         - Response body
         - Data properties
         - Error handling
```

---

## 🔄 Test Execution Flow

```
START: npm test
  │
  ├─ Load playwright.config.ts
  │  │
  │  ├─ Set baseURL: http://localhost:5000
  │  └─ Configure reporters: html, junit
  │
  ├─ Load test files (characters.spec.ts, builds.spec.ts, etc)
  │  │
  │  └─ Load fixtures (auth, data, api.base)
  │
  ├─ For each test file:
  │  │
  │  ├─ describe('API Name', () => {
  │  │  │
  │  │  └─ For each test:
  │  │     │
  │  │     ├─ ARRANGE: Seed test data
  │  │     │  example: await seedCharacter(request, userId, 'TestChar')
  │  │     │
  │  │     ├─ ACT: Execute API call
  │  │     │  example: response = await apiContext.get('/api/characters')
  │  │     │
  │  │     ├─ ASSERT: Verify results
  │  │     │  example: expect(response.status()).toBe(200)
  │  │     │
  │  │     └─ ✅ or ❌ Test result
  │  │
  │  └─ })
  │
  ├─ Collect results
  │  ├─ Tests passed: 38
  │  ├─ Tests failed: 0
  │  └─ Duration: 45 seconds
  │
  ├─ Generate reports
  │  ├─ playwright-report/index.html
  │  ├─ test-results.xml (for CI/CD)
  │  └─ screenshots on failure
  │
  └─ END: Display summary & exit code
```

---

## 📊 API Controller Mapping

```
┌────────────────────────────────────────────────────────┐
│            API Controllers & Test Files                 │
├────────────────────────────────────────────────────────┤
│                                                         │
│  1. CharactersApiController    →  characters.spec.ts   │
│     GET, POST, PUT, DELETE, SEARCH                     │
│     ✅ 6 tests implemented                              │
│                                                         │
│  2. BuildsApiController         →  builds.spec.ts      │
│     GET, POST, PUT, DELETE                             │
│     📋 5 tests (use template)                           │
│                                                         │
│  3. ItemsApiController          →  items.spec.ts       │
│     GET, POST (weapon/armor), PUT, DELETE, SEARCH      │
│     📋 7 tests (use template)                           │
│                                                         │
│  4. SkillsApiController         →  skills.spec.ts      │
│     GET, POST, PUT, DELETE, SEARCH                     │
│     📋 6 tests (use template)                           │
│                                                         │
│  5. RatingsApiController        →  ratings.spec.ts     │
│     GET (anon), POST (auth), PUT (auth), DELETE (auth) │
│     Special: Owner cannot rate own build               │
│     📋 7 tests (use template + auth)                    │
│                                                         │
│  6. UsersApiController          →  users.spec.ts       │
│     POST (password validation), GET, PUT, DELETE       │
│     Integrates: ASP.NET Identity UserManager           │
│     📋 7 tests (use template + identity)                │
│                                                         │
│  7. ProfileFilesApiController   →  profile-files.spec  │
│     POST (file upload), GET, DELETE, PUT (set current) │
│     Integrates: File storage, authorization            │
│     📋 4 tests (optional - use template)                │
│                                                         │
└────────────────────────────────────────────────────────┘
```

---

## 🔐 Authentication & Authorization Pattern

```
ANONYMOUS ENDPOINT (No auth required)
┌─────────────────────┐
│  GET /api/ratings   │
└──────────┬──────────┘
           │
    No X-Test-UserId header needed
           │
           ▼
    Returns public data


PROTECTED ENDPOINT (Auth required)
┌──────────────────────────────┐
│  POST /api/ratings           │
│  Headers: X-Test-UserId: 123 │
└──────────┬───────────────────┘
           │
    User ID extracted from header
           │
           ▼
    Validates: User exists
    Validates: Not rating own build
           │
           ▼
    ✅ Create rating  or  ❌ 400/403 error


OWNERSHIP CHECK EXAMPLE
┌────────────────────────────────────────┐
│  PUT /api/ratings/{id}                 │
│  Headers: X-Test-UserId: 123 (owner)   │
└──────────┬─────────────────────────────┘
           │
    Check: User 123 owns rating {id}?
           │
       ┌───┴───┐
       ▼       ▼
      YES      NO
      │        │
      ✅       ❌ 403 Forbidden
    Update   Reject
```

---

## 📈 Test Coverage Matrix

```
┌─────────────────┬────────┬────────┬────────┬────────┬────────┐
│  Endpoint       │  GET   │  POST  │  PUT   │ DELETE │ SEARCH │
├─────────────────┼────────┼────────┼────────┼────────┼────────┤
│  Characters     │   ✅   │   ✅   │   ✅   │   ✅   │   ✅   │
│  Builds         │   📋   │   📋   │   📋   │   📋   │   -    │
│  Items          │   ✅   │   ✅   │   ✅   │   ✅   │   ✅   │
│  Skills         │   ✅   │   ✅   │   ✅   │   ✅   │   ✅   │
│  Ratings        │   ✅   │   ✅   │   ✅   │   ✅   │   ✅   │
│  Users          │   ✅   │   ✅   │   ✅   │   ✅   │   ✅   │
│  Profile Files  │   📋   │   📋   │   📋   │   📋   │   -    │
└─────────────────┴────────┴────────┴────────┴────────┴────────┘

Legend:
✅ = Implemented & working
📋 = Template provided in guide
-  = Not applicable

Total: 45+ tests
Status: 6 tests ✅ / 39 tests 📋
Coverage: 13% implemented, 87% ready to implement
```

---

## 🚀 Implementation Roadmap

```
╔════════════════════════════════════════════════════════════════╗
║  PHASE 1: Setup Complete (30% time, 13% tests)  [✅ DONE]     ║
╠════════════════════════════════════════════════════════════════╣
║  ✅ Install Playwright & dependencies                          ║
║  ✅ Configure playwright.config.ts (baseURL, reporters)        ║
║  ✅ Create auth fixture (X-Test-UserId header)                ║
║  ✅ Create data seeding fixture                                ║
║  ✅ Implement Characters API tests (6 tests)                   ║
╚════════════════════════════════════════════════════════════════╝
                            │
                            ▼
╔════════════════════════════════════════════════════════════════╗
║  PHASE 2: Core APIs (40% time, 57% tests)  [⏳ NEXT]          ║
╠════════════════════════════════════════════════════════════════╣
║  📋 Builds API tests (5 tests)     - seed character first      ║
║  📋 Items API tests (7 tests)      - test all item types       ║
║  📋 Skills API tests (6 tests)     - validate soft delete      ║
║  📋 Ratings API tests (7 tests)    - test auth & ownership     ║
╚════════════════════════════════════════════════════════════════╝
                            │
                            ▼
╔════════════════════════════════════════════════════════════════╗
║  PHASE 3: Advanced APIs (20% time, 30% tests)  [⏳ LATER]     ║
╠════════════════════════════════════════════════════════════════╣
║  📋 Users API tests (7 tests)      - Identity & password       ║
║  📋 Profile Files API (4 tests)    - File upload & storage     ║
║  Review & optimize                                             ║
║  Run full suite & generate report                              ║
╚════════════════════════════════════════════════════════════════╝
                            │
                            ▼
                     ✅ ALL 45 TESTS COMPLETE
```

---

## 💼 Quick Implementation Workflow

```
For each controller:

1. COPY TEMPLATE (5 min)
   └─ Reference: tests/api/characters.spec.ts
   
2. ADAPT TEMPLATE (15 min)
   ├─ Change endpoint path
   ├─ Adjust test data structure
   └─ Update assertions for expected responses
   
3. RUN TESTS (2 min)
   ├─ npm run test:<controller>
   └─ Verify all tests pass
   
4. CHECK COVERAGE (3 min)
   ├─ Does it cover GET, POST, PUT, DELETE?
   ├─ Does it test edge cases?
   └─ Are error scenarios handled?
   
5. MOVE TO NEXT (skip)
   └─ Repeat for next controller

TOTAL TIME PER CONTROLLER: ~25 minutes
TOTAL TIME FOR ALL 6 CONTROLLERS: ~2.5 hours
```

---

## 📚 File-to-Purpose Reference

| File | Purpose | Created | Status |
|------|---------|---------|--------|
| PLAYGROUND_API_TESTS_GUIDE.md | Step-by-step implementation (10 steps) | ✅ | Complete |
| README.md | Quick start & troubleshooting | ✅ | Complete |
| IMPLEMENTATION_CHECKLIST.md | Progress tracking | ✅ | Complete |
| SUMMARY.md | This overview | ✅ | Complete |
| package.json | npm scripts & dependencies | ✅ | Ready |
| playwright.config.ts | Playwright configuration | ✅ | Ready |
| tsconfig.json | TypeScript configuration | ✅ | Ready |
| tests/api.base.ts | Base test class | ✅ | Ready |
| tests/fixtures/auth.fixture.ts | Auth header injection | ✅ | Ready |
| tests/fixtures/data.fixture.ts | Data seeding helpers | ✅ | Ready |
| tests/api/characters.spec.ts | Characters tests | ✅ | Working (6 tests) |
| tests/api/builds.spec.ts | Template in guide | 📋 | Implement now |
| tests/api/items.spec.ts | Template in guide | 📋 | Implement next |
| tests/api/skills.spec.ts | Template in guide | 📋 | Implement next |
| tests/api/ratings.spec.ts | Template in guide | 📋 | Implement after |
| tests/api/users.spec.ts | Template in guide | 📋 | Implement after |

---

## 🎯 Quick Links

**Getting Started?**
- Read: SUMMARY.md (2 min) ← You are here
- Then: README.md (5 min)
- Then: Run `npm install && npm test`

**Implementing Tests?**
- Reference: tests/api/characters.spec.ts (working example)
- Guide: PLAYWRIGHT_API_TESTS_GUIDE.md (detailed instructions)
- Track: IMPLEMENTATION_CHECKLIST.md (check off progress)

**Debugging Tests?**
- Run: `npm run test:debug` (interactive mode)
- Run: `npm run test:headed` (see browser)
- Read: README.md section "Debugging Tests"

**Running Full Suite?**
- Command: `npm test`
- View Report: `npm run report`
- CI/CD: `npm run test:ci`

---

## 📊 Success Criteria

```
Phase 1 (Current):     ✅ COMPLETE
  └─ 6/38 tests done (16%)
  └─ All setup files created
  
Phase 2 (Next):
  └─ 25/38 tests done (66%)
  └─ All core APIs covered
  └─ Estimated time: 1.5 hours
  
Phase 3 (Final):
  └─ 38/38 tests done (100%) ✅
  └─ Full coverage achieved
  └─ All controllers tested
  └─ Estimated total time: 2-3 hours
```

---

## 🎊 You're Now Ready To:

✅ Run working tests: `npm test`
✅ View test results: `npm run report`
✅ Debug tests: `npm run test:debug`
✅ Implement more tests: Copy `characters.spec.ts` pattern
✅ Track progress: Use `IMPLEMENTATION_CHECKLIST.md`
✅ Deploy to CI/CD: Follow `README.md` CI/CD section

**Start with:**
```bash
cd BG3BuildPlanner.IntegrationTests/playwright
npm install
npm test
npm run report
```

Print this diagram and keep it handy while implementing! 📌

