# Talent Flow

## Purpose

Bridge the talent system between flow events (level-up, purchase) and core services.

## Requirements

### Requirement: Level Up Grants Talent Point

The `LevelUpFlowHandler` SHALL grant 1 talent point to the player each time their level increases.

#### Scenario: Single level up

- **WHEN** the player levels up by 1
- **THEN** 1 talent point is added to available points via TalentService

#### Scenario: Multiple level ups at once

- **WHEN** the player levels up multiple times in a single XP addition (e.g., from level 3 to level 5)
- **THEN** each level increment grants 1 talent point (total of 2 points for +2 levels)

### Requirement: Purchase Talent Event

The system SHALL define a `PurchaseTalentEvent` command that carries the talent ID to purchase.

#### Scenario: Event created with talent id

- **WHEN** a purchase is triggered from the UI
- **THEN** a `PurchaseTalentEvent` is published with the target talent's id

### Requirement: Talent Flow Handler Processes Purchase

The system SHALL have a `TalentFlowHandler` that listens for `PurchaseTalentEvent` and delegates to `TalentService.TryPurchase()`.

#### Scenario: Valid purchase event

- **WHEN** a `PurchaseTalentEvent` is received with a valid talent id
- **THEN** the handler calls TalentService.TryPurchase(talentId)

#### Scenario: Invalid talent id

- **WHEN** a `PurchaseTalentEvent` is received with an unknown talent id
- **THEN** the purchase fails silently (no state changes, no crash)
