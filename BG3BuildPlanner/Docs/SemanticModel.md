# Semantic Database Model (BG3 Build Planner)

## Overview
This semantic model describes the core domain entities, their attributes, and relationships for the BG3 Build Planner. It maps directly to the EF Core entity classes and the SQLite schema generated from them.

## Entities and Attributes

### User
- Primary key: Id
- Attributes: Username, Email, PasswordHash
- Relationships:
  - One-to-many with Build (User.Id -> Build.UserId)
  - One-to-many with Rating (User.Id -> Rating.UserId)

### Character
- Primary key: Id
- Attributes: Name, PortraitUrl, Race, Background, Level, CreatedAt
- Relationships:
  - One-to-many with Build (Character.Id -> Build.CharacterId)

### Build
- Primary key: Id
- Attributes: Title, Description, Difficulty, CreatedAt
- Foreign keys:
  - UserId -> User.Id
  - CharacterId -> Character.Id
- Relationships:
  - Many-to-one with User
  - Many-to-one with Character
  - One-to-many with Rating (Build.Id -> Rating.BuildId)
  - Many-to-many with Skill (Builds <-> Skills)
  - Many-to-many with Item (Builds <-> Items)
  - One-to-one/one-to-many with AbilityScore (AbilityScore.BuildId -> Build.Id)

### Skill
- Primary key: Id
- Attributes: Name, Description, RequiredLevel, ImageUrl
- Relationships:
  - Many-to-many with Build

### Item
- Primary key: Id
- Attributes: Name, Type, Rarity, Power
- Relationships:
  - Many-to-many with Build

### Rating
- Primary key: Id
- Attributes: Score, Comment, CreatedAt
- Foreign key:
  - BuildId -> Build.Id
  - UserId -> User.Id
- Relationships:
  - Many-to-one with Build
  - Many-to-one with User

### AbilityScore
- Primary key: Id
- Attributes: Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma
- Foreign key:
  - BuildId -> Build.Id
- Relationships:
  - Many-to-one with Build (conceptually one set of scores per build)

## Cardinality Summary
- User 1..* Build
- User 1..* Rating
- Character 1..* Build
- Build 1..* Rating
- Build *..* Skill
- Build *..* Item
- Build 1..1 AbilityScore (intended), currently modeled as Build 1..* AbilityScore unless constrained

## Notes on Implementation
- Many-to-many relationships use EF Core conventions, which create implicit join tables in SQLite.
- Required properties are enforced in code with required members and by non-nullable columns in SQLite.
- Enum types:
  - Difficulty: Explorer, Balanced, Tactician
  - ItemType: Weapon, Armor, Accessory

## Source Files
- Data entities: Data folder
- DbContext: ApplicationDbContext
