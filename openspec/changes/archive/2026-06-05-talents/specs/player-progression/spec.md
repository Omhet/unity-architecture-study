# Player Progression

## ADDED Requirements

### Requirement: Level Up Grants Talent Point

The system SHALL grant 1 talent point to the player each time their level increases.

#### Scenario: Single level up grants one point

- **WHEN** the player's level increases by 1
- **THEN** 1 talent point is added to the player's available talent points via TalentService

#### Scenario: Multiple level ups grant multiple points

- **WHEN** the player levels up multiple times in a single XP addition (e.g., from level 3 to level 5)
- **THEN** each level increment grants 1 talent point (total of 2 points for +2 levels)
