# Talent Core

## Purpose

Manage talent definitions, player talent state (available points, per-talent investment), and purchase logic with config-driven validation.

## Requirements

### Requirement: Track Available Talent Points

The system SHALL maintain a reactive count of the player's available (unspent) talent points.

#### Scenario: Initial state

- **WHEN** the game starts with no level-ups yet
- **THEN** available talent points are 0

#### Scenario: Points increase on level up

- **WHEN** the player levels up
- **THEN** available talent points increase by 1

### Requirement: Track Points Spent Per Talent

The system SHALL maintain a mapping of each talent ID to the number of points the player has invested in that talent.

#### Scenario: No points spent initially

- **WHEN** the game starts
- **THEN** all talents have 0 points spent

#### Scenario: Points recorded after purchase

- **WHEN** the player purchases a talent
- **THEN** the points spent for that talent ID increases by 1

### Requirement: Load Talent Definitions From Configuration

The system SHALL load talent definitions from a JSON-backed configuration, including id, name, cost, increasePerPoint, and maxPoints.

#### Scenario: Load talents on initialization

- **WHEN** the talent system initializes
- **THEN** all talent entries are loaded from the game configuration into the registry

### Requirement: Registry Lookup By ID

The system SHALL provide a lookup method to retrieve a talent definition by its unique id.

#### Scenario: Find existing talent

- **WHEN** a valid talent id is queried
- **THEN** the corresponding talent definition is returned

#### Scenario: Unknown talent id

- **WHEN** an invalid talent id is queried
- **THEN** null or false is returned indicating no match

### Requirement: Purchase Talent With Validation

The system SHALL allow purchasing a talent only if the player has enough available points AND has not reached the talent's maxPoints limit.

#### Scenario: Successful purchase

- **WHEN** the player has sufficient available points and the talent has not reached its maxPoints cap
- **THEN** one available point is deducted and one point is added to that talent's spent count

#### Scenario: Insufficient points

- **WHEN** the player does not have enough available points
- **THEN** the purchase fails and no state changes occur

#### Scenario: Max points reached

- **WHEN** the talent has already reached its maxPoints limit
- **THEN** the purchase fails and no state changes occur

### Requirement: Calculate Multiplier For Talent

The system SHALL calculate a multiplier for any talent based on the formula `1.0f + (pointsSpent × IncreasePerPoint)`.

#### Scenario: No points invested

- **WHEN** 0 points are spent on a talent
- **THEN** the multiplier is 1.0x

#### Scenario: Points invested

- **WHEN** 3 points are spent on a talent with increasePerPoint of 0.1
- **THEN** the multiplier is 1.3x

### Requirement: Query Current Investment For Talent

The system SHALL provide a method to query how many points are currently invested in a specific talent.

#### Scenario: Query invested points

- **WHEN** a valid talent id is queried
- **THEN** the number of points spent on that talent is returned
