# 📋 Test Data Reference Guide

## Quick Reference for Test Data Across All APIs

---

## 🎭 Characters API

### Create Request
```json
{
  "name": "Aragorn",
  "race": "Human",
  "class": "Ranger",
  "level": 10
}
```

### Response
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Aragorn",
  "race": "Human",
  "class": "Ranger",
  "level": 10,
  "createdAt": "2026-07-10T12:00:00Z",
  "deletedAt": null
}
```

### Test Pattern
```typescript
// Required for builds
characterId: "550e8400-e29b-41d4-a716-446655440000"
```

---

## 🏗️ Builds API

### Create Request
```json
{
  "characterId": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Frost Mage Build",
  "description": "Ice-focused damage build",
  "notes": "Focus on spell power"
}
```

### Response
```json
{
  "id": "650e8400-e29b-41d4-a716-446655440001",
  "characterId": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Frost Mage Build",
  "description": "Ice-focused damage build",
  "notes": "Focus on spell power",
  "createdAt": "2026-07-10T12:05:00Z",
  "deletedAt": null
}
```

### Dependencies
```
Build → Character (required)
  Must provide valid characterId
```

### Test Pattern
```typescript
// Seed character first
const char = await seedCharacter(request, baseURL, userId, 'Hero');

// Then use characterId for build
const buildData = {
  characterId: char.id,  // ✅ Required
  name: 'Build Name'
};
```

---

## 📦 Items API

### Create Request - Weapon
```json
{
  "name": "Longsword",
  "type": "Weapon",
  "rarity": "Rare",
  "power": 15
}
```

### Create Request - Armor
```json
{
  "name": "Plate Armor",
  "type": "Armor",
  "rarity": "Uncommon",
  "power": 12
}
```

### Create Request - Accessory
```json
{
  "name": "Ring of Protection",
  "type": "Accessory",
  "rarity": "Legendary",
  "power": 20
}
```

### Enums
```typescript
type: "Weapon" | "Armor" | "Accessory"
rarity: "Common" | "Uncommon" | "Rare" | "Legendary"
power: number (0-100)
```

### Response
```json
{
  "id": "750e8400-e29b-41d4-a716-446655440002",
  "name": "Longsword",
  "type": "Weapon",
  "rarity": "Rare",
  "power": 15,
  "createdAt": "2026-07-10T12:10:00Z"
}
```

### Test Pattern
```typescript
// Test each type separately
const itemData = {
  name: `Item_${Date.now()}`,
  type: 'Weapon',           // Try: Armor, Accessory
  rarity: 'Rare',           // Try: Common, Uncommon, Legendary
  power: 15
};
```

### Validation Notes
- Hard delete (not soft) - no deletedAt field
- Must provide valid type/rarity
- Power must be numeric

---

## ✨ Skills API

### Create Request
```json
{
  "name": "Fireball",
  "description": "Deals 30 fire damage to all enemies",
  "level": 3
}
```

### Response
```json
{
  "id": "850e8400-e29b-41d4-a716-446655440003",
  "name": "Fireball",
  "description": "Deals 30 fire damage to all enemies",
  "level": 3,
  "createdAt": "2026-07-10T12:15:00Z",
  "deletedAt": null
}
```

### Soft Delete Behavior
```typescript
// After DELETE:
const deleteResponse = await apiContext.delete('/api/skills/{id}');
expect(deleteResponse.status()).toBe(204);

// Skill no longer in list:
const listResponse = await apiContext.get('/api/skills');
const skills = await listResponse.json();
const found = skills.find(s => s.id === deletedId);
expect(found).toBeUndefined();  // Soft deleted
```

### Test Pattern
```typescript
// Validate .Active() filter
const skillData = {
  name: `Skill_${Date.now()}`,
  description: 'Test skill description',
  level: 1
};
```

---

## ⭐ Ratings API

### Create Request
```json
{
  "buildId": "650e8400-e29b-41d4-a716-446655440001",
  "rating": 5,
  "comment": "Excellent build! Very effective."
}
```

### Response
```json
{
  "id": "950e8400-e29b-41d4-a716-446655440004",
  "buildId": "650e8400-e29b-41d4-a716-446655440001",
  "userId": "test-user-001",
  "rating": 5,
  "comment": "Excellent build! Very effective.",
  "createdAt": "2026-07-10T12:20:00Z",
  "deletedAt": null
}
```

### Authorization
```typescript
// Anonymous GET (allowed)
const response = await apiContext.get('/api/ratings');  // ✅

// Create requires auth
const response = await apiContext.post('/api/ratings', {
  headers: { 'X-Test-UserId': userId },  // ✅ Required
  data: ratingData
});
```

### Business Rules
```typescript
// Owner cannot rate own build
const ownerRating = await apiContext.post('/api/ratings', {
  headers: { 'X-Test-UserId': buildOwnerId },  // ❌ Forbidden
  data: { buildId: ownBuildId, rating: 5 }
});
expect([400, 403]).toContain(ownerRating.status());

// Different user can rate
const otherUserRating = await apiContext.post('/api/ratings', {
  headers: { 'X-Test-UserId': 'other-user' },  // ✅ Allowed
  data: { buildId: ownBuildId, rating: 5 }
});
expect(otherUserRating.status()).toBe(201);
```

### Dependencies
```
Rating → Build (required)
Rating → User (auto from X-Test-UserId header)
  Build owner cannot rate own build
```

### Test Pattern
```typescript
// Setup: Create build with one user
const buildResponse = await apiContext.post('/api/builds', {
  headers: { 'X-Test-UserId': buildOwnerId },
  data: { characterId, name: 'Build' }
});
const build = await buildResponse.json();

// Test 1: Other user rates (allowed)
const rating1 = await apiContext.post('/api/ratings', {
  headers: { 'X-Test-UserId': otherUserId },
  data: { buildId: build.id, rating: 5 }
});
expect(rating1.status()).toBe(201);

// Test 2: Owner rates (forbidden)
const rating2 = await apiContext.post('/api/ratings', {
  headers: { 'X-Test-UserId': buildOwnerId },
  data: { buildId: build.id, rating: 5 }
});
expect([400, 403]).toContain(rating2.status());
```

---

## 👤 Users API

### Create Request
```json
{
  "username": "adventurer_123",
  "email": "adventurer@game.com",
  "password": "SecurePass123!"
}
```

### Password Requirements
```
Length: 8+ characters
Must contain:
  - Uppercase letter (A-Z)
  - Lowercase letter (a-z)
  - Number (0-9)
  - Special character (!@#$%^&*)

Examples:
  "SecurePass123!"    ✅ Valid
  "MyGame2024@"       ✅ Valid
  "weak"              ❌ Too short
  "ALLUPPERCASE123!"  ❌ Only uppercase + special
```

### Response (Identity Integration)
```json
{
  "id": "b7c4e400-e29b-41d4-a716-446655440005",
  "username": "adventurer_123",
  "email": "adventurer@game.com",
  "createdAt": "2026-07-10T12:25:00Z",
  "deletedAt": null
}
```

### Note
```
Password is NOT returned in response (security best practice)
```

### Validation Rules
```typescript
// Unique username
const user1 = await apiContext.post('/api/users', {
  headers: { 'X-Test-UserId': userId },
  data: {
    username: 'uniqueName',
    email: 'email1@test.com',
    password: 'SecurePass123!'
  }
});
expect(user1.status()).toBe(201);

const user2 = await apiContext.post('/api/users', {
  headers: { 'X-Test-UserId': userId },
  data: {
    username: 'uniqueName',  // ❌ Duplicate
    email: 'email2@test.com',
    password: 'SecurePass123!'
  }
});
expect([400, 422]).toContain(user2.status());  // Validation error
```

### Test Pattern
```typescript
// Generate unique data per test
const username = `testuser_${Date.now()}`;
const email = `test_${Date.now()}@example.com`;

const userData = {
  username: username,           // Unique per test
  email: email,                 // Unique per test
  password: 'SecurePass123!'    // Valid format
};

// Test weak password separately
const weakData = {
  username: `weak_${Date.now()}`,
  email: `weak_${Date.now()}@test.com`,
  password: 'weak'  // ❌ Invalid
};
```

### Authorization
```typescript
// All endpoints require X-Test-UserId header
const response = await apiContext.post('/api/users', {
  headers: { 'X-Test-UserId': userId },  // Required
  data: userData
});
```

---

## 📁 Profile Files API (Optional)

### Upload Request
```
POST /api/profile/files
Content-Type: multipart/form-data

file: <image file>
```

### Upload Restrictions
```typescript
MaxSize: 5 MB
AllowedTypes: image/jpeg, image/png, image/gif, image/webp
AllowedExtensions: .jpg, .jpeg, .png, .gif, .webp
```

### Response
```json
{
  "id": "a5d7e400-e29b-41d4-a716-446655440006",
  "userId": "test-user-001",
  "originalFileName": "my-avatar.jpg",
  "storedFileName": "550e8400-e29b-41d4-a716-446655440000.jpg",
  "relativePath": "/uploads/users/550e8400-e29b-41d4-a716-446655440000.jpg",
  "fileSize": 25600,
  "uploadedAt": "2026-07-10T12:30:00Z",
  "current": false
}
```

### File Storage
```
Physical location: wwwroot/uploads/users/
Naming: {guid}.{original_extension}
```

### Test Pattern
```typescript
// Requires FormData for file upload
const formData = new FormData();
formData.append('file', fileBlob, 'test.jpg');

const response = await apiContext.post('/api/profile/files', {
  headers: { 'X-Test-UserId': userId },
  data: formData
});

expect(response.status()).toBe(201);
const file = await response.json();
expect(file.originalFileName).toBe('test.jpg');
expect(file.fileSize).toBeGreaterThan(0);
```

### Authorization
```typescript
// All endpoints require authentication
const response = await apiContext.get('/api/profile/files', {
  headers: { 'X-Test-UserId': userId }  // Required
});
```

---

## 🔑 Common Headers

### Authentication (Protected Endpoints)
```typescript
headers: {
  'X-Test-UserId': 'test-user-001',
  'Content-Type': 'application/json'
}
```

### No Authentication (Public Endpoints)
```typescript
// GET /api/ratings (list)
headers: {
  'Content-Type': 'application/json'
  // No X-Test-UserId needed
}
```

---

## 🧪 Test Data Generators

### Unique Character Name
```typescript
const characterName = `Character_${Date.now()}`;
```

### Unique Build Name
```typescript
const buildName = `Build_${Date.now()}`;
```

### Unique Item Name
```typescript
const itemName = `Item_${Date.now()}`;
```

### Unique Skill Name
```typescript
const skillName = `Skill_${Date.now()}`;
```

### Unique User Credentials
```typescript
const timestamp = Date.now();
const username = `user_${timestamp}`;
const email = `test_${timestamp}@example.com`;
const password = 'SecurePass123!';
```

### Unique Rating Comment
```typescript
const comment = `Comment_${Date.now()}`;
```

---

## 📊 Relationships Summary

```
User
  ├─→ Builds (owner)
  ├─→ Ratings (user_id)
  └─→ ProfileFiles (owner)

Build
  ├─→ Character (required)
  ├─→ User (owner)
  └─→ Ratings (multiple)

Character
  ├─→ User (owner)
  └─→ Builds (multiple)

Item
  └─→ (standalone - no relationships)

Skill
  └─→ (standalone - no relationships)

Rating
  ├─→ Build (required)
  ├─→ User (from header)
  └─ Business rule: owner ≠ rater
```

---

## ✅ Validation Checklist

When implementing tests, verify:

- [ ] **Characters:** name, race, class, level required
- [ ] **Builds:** characterId required, character must exist
- [ ] **Items:** type/rarity enum validation
- [ ] **Skills:** soft delete removes from Active list
- [ ] **Ratings:** owner cannot rate own build
- [ ] **Users:** password strength validation, unique username/email
- [ ] **Headers:** X-Test-UserId required for POST/PUT/DELETE (except public GET)
- [ ] **Unique data:** Use `Date.now()` to avoid conflicts

---

## 🎯 Copy-Paste Data Sets

### Characters
```typescript
{ name: `Char_${Date.now()}`, race: 'Human', class: 'Fighter', level: 5 }
{ name: `Char_${Date.now()}`, race: 'Elf', class: 'Ranger', level: 8 }
{ name: `Char_${Date.now()}`, race: 'Dwarf', class: 'Paladin', level: 7 }
```

### Items
```typescript
{ name: `Item_${Date.now()}`, type: 'Weapon', rarity: 'Rare', power: 15 }
{ name: `Item_${Date.now()}`, type: 'Armor', rarity: 'Uncommon', power: 10 }
{ name: `Item_${Date.now()}`, type: 'Accessory', rarity: 'Legendary', power: 20 }
```

### Skills
```typescript
{ name: `Skill_${Date.now()}`, description: 'Test skill', level: 1 }
{ name: `Skill_${Date.now()}`, description: 'Test skill', level: 5 }
```

### Users
```typescript
{ username: `user_${Date.now()}`, email: `u_${Date.now()}@test.com`, password: 'SecurePass123!' }
```

### Ratings
```typescript
{ buildId: id, rating: 5, comment: `Very good build_${Date.now()}` }
{ buildId: id, rating: 3, comment: `Needs improvement_${Date.now()}` }
```

---

*This reference is structured to help you quickly lookup test data for each API when implementing tests.*

