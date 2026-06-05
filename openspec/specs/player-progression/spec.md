# Player Progression

## Purpose

Manage player level, XP tracking, and progression-related systems.

## Requirements

### Requirement: Track Player Level and XP

The system SHALL maintain the player's current level and experience points (XP) in a reactive state.

#### Scenario: XP increase

- **WHEN** XP is added to the progression state
- **THEN** the XP value is updated and observers are notified

### Requirement: Level Up Logic

The system SHALL automatically increment the player's level when their current XP reaches or exceeds the requirement for the next level.

#### Scenario: Reaching XP threshold

- **WHEN** added XP causes total XP to exceed the current level's threshold
- **THEN** the Level is incremented and XP is subtracted by the requirement amount

### Requirement: JSON Configuration Table

The system SHALL load level requirements from a JSON-backed configuration file.

#### Scenario: Load configuration

- **WHEN** the progression system initializes
- **THEN** it loads the XP thresholds for all levels from the game configuration

### Requirement: Shop Refresh on Level Up

The system SHALL refresh the list of available shop items whenever the player's level increases.

#### Scenario: Refresh shop after level up

- **WHEN** the player level property changes
- **THEN** the ShopService is triggered to recalculate unlocked items based on the new level
