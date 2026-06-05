# Talent View

## Purpose

Display the player's talent point balance and available talents with purchase controls in the HUD.

## Requirements

### Requirement: Display Available Talent Points

The `TalentSectionView` SHALL display the player's current available (unspent) talent points.

#### Scenario: Show zero initially

- **WHEN** the view mounts and the player has 0 available points
- **THEN** the balance displays "0"

#### Scenario: Update on point change

- **WHEN** the player's available talent points change (level up or purchase)
- **THEN** the displayed balance updates reactively

### Requirement: Display Talent List With Investment Levels

The `TalentSectionView` SHALL display all talents from the registry, showing each talent's current investment level and max cap.

#### Scenario: Show all talents

- **WHEN** the view mounts
- **THEN** all configured talents are displayed in a list with their names and current/max points (e.g., "0/10")

#### Scenario: Update investment display on purchase

- **WHEN** the player purchases a talent
- **THEN** that talent's investment level updates reactively (e.g., "0/10" → "1/10")

### Requirement: Purchase Button Per Talent

Each talent in the list SHALL have a buy button that triggers a `PurchaseTalentEvent` when clicked.

#### Scenario: Buy button publishes event

- **WHEN** the player clicks a talent's buy button
- **THEN** a `PurchaseTalentEvent` is published with that talent's id

### Requirement: Disable Button When Unavailable

The buy button SHALL be disabled when the player cannot purchase the talent (insufficient points or max reached).

#### Scenario: Disabled at max points

- **WHEN** a talent has reached its maxPoints cap
- **THEN** the buy button is disabled

#### Scenario: Disabled with insufficient points

- **WHEN** the player has 0 available points
- **THEN** all buy buttons are disabled

### Requirement: Follow Section View Pattern

The `TalentSectionView` SHALL extend `GameplaySectionViewBase` and follow the BuildContent/Bind/Unbind lifecycle.

#### Scenario: View follows lifecycle

- **WHEN** the view is mounted/unmounted
- **THEN** subscriptions are created/disposed following the base class pattern
