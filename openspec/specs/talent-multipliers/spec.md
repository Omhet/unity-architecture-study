# Talent Multipliers

## Purpose

Apply talent-based multipliers to output amounts in GeneratorService, CraftService, and OrderService.

## Requirements

### Requirement: Generator Service Applies Talent Multiplier

The `GeneratorService` SHALL apply the generator boost talent multiplier when generating resources from a generator.

#### Scenario: Base generation with no points

- **WHEN** a generator produces output with 0 talent points invested in generator boost
- **THEN** the resource amount is unchanged (base value × 1.0)

#### Scenario: Generation with talent points

- **WHEN** a generator produces output with talent points invested in generator boost
- **THEN** the resource amount is `Ceiling(baseAmount × multiplier)` where multiplier comes from TalentService

#### Scenario: Minimum production of 1

- **WHEN** the base generation amount multiplied by the multiplier would round down below 1
- **THEN** at least 1 unit is produced

### Requirement: Craft Service Applies Talent Multiplier

The `CraftService` SHALL apply the craft boost talent multiplier when crafting products from recipes.

#### Scenario: Base crafting with no points

- **WHEN** a recipe produces output with 0 talent points invested in craft boost
- **THEN** the product amount is unchanged (base value × 1.0)

#### Scenario: Crafting with talent points

- **WHEN** a recipe produces output with talent points invested in craft boost
- **THEN** the product amount is `Ceiling(baseAmount × multiplier)` where multiplier comes from TalentService

### Requirement: Order Service Applies Talent Multiplier

The `OrderService` SHALL apply the order boost talent multiplier when rewarding money for completing orders.

#### Scenario: Base reward with no points

- **WHEN** an order completes with 0 talent points invested in order boost
- **THEN** the money reward is unchanged (order.Reward × 1.0)

#### Scenario: Reward with talent points

- **WHEN** an order completes with talent points invested in order boost
- **THEN** the money reward is `Ceiling(order.Reward × multiplier)` where multiplier comes from TalentService
