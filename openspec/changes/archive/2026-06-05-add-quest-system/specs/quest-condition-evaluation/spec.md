## ADDED Requirements

### Requirement: IConditionEvaluator interface defines evaluation contract

The system SHALL provide an IConditionEvaluator interface with two methods: `IsMet()` for snapshot checks and `Observe()` for reactive observation of claimable status.

#### Scenario: Snapshot check returns current state

- **WHEN** IsMet() is called on a condition evaluator
- **THEN** it returns true if the condition threshold is currently met, false otherwise

#### Scenario: Observation fires when condition becomes met

- **WHEN** Observe() is called and subscribed to
- **AND** the underlying game state changes such that the condition threshold is reached
- **THEN** the observable emits true exactly once (first time condition is met)

### Requirement: MoneyThresholdEvaluator observes economy balance

The system SHALL provide a MoneyThresholdEvaluator that evaluates whether the player's economy balance has reached a specified threshold.

#### Scenario: Condition met when balance reaches threshold

- **WHEN** a MoneyThresholdEvaluator is created with threshold 100
- **AND** EconomyState.Balance.Value reaches 100 or more
- **THEN** IsMet() returns true and Observe() emits true

#### Scenario: Condition not met below threshold

- **WHEN** a MoneyThresholdEvaluator is created with threshold 100
- **AND** EconomyState.Balance.Value is less than 100
- **THEN** IsMet() returns false

### Requirement: ResourceThresholdEvaluator observes resource amounts

The system SHALL provide a ResourceThresholdEvaluator that evaluates whether the player's amount of a specific resource has reached a specified threshold.

#### Scenario: Condition met when resource reaches threshold

- **WHEN** a ResourceThresholdEvaluator is created for resource "wood" with threshold 20
- **AND** ResourceState.Balances["wood"] reaches 20 or more
- **THEN** IsMet() returns true and Observe() emits true

#### Scenario: Condition not met below threshold

- **WHEN** a ResourceThresholdEvaluator is created for resource "wood" with threshold 20
- **AND** ResourceState.Balances["wood"] is less than 20
- **THEN** IsMet() returns false

### Requirement: ProductThresholdEvaluator observes product amounts

The system SHALL provide a ProductThresholdEvaluator that evaluates whether the player's amount of a specific product has reached a specified threshold.

#### Scenario: Condition met when product reaches threshold

- **WHEN** a ProductThresholdEvaluator is created for product "wooden_hammer" with threshold 1
- **AND** ProductState.Amounts["wooden_hammer"] reaches 1 or more
- **THEN** IsMet() returns true and Observe() emits true

#### Scenario: Condition not met below threshold

- **WHEN** a ProductThresholdEvaluator is created for product "wooden_hammer" with threshold 1
- **AND** ProductState.Amounts["wooden_hammer"] is less than 1
- **THEN** IsMet() returns false

### Requirement: Evaluators are disposable to prevent subscription leaks

The system SHALL dispose evaluator subscriptions when a quest is claimed (completed) by the QuestFlowHandler, to prevent memory leaks from stale R3 subscriptions.

#### Scenario: Evaluator disposed on quest completion

- **WHEN** the QuestFlowHandler processes a ClaimQuestEvent and calls QuestService.Claim()
- **THEN** the flow handler disposes the corresponding evaluator subscription for that quest
- **AND** no further observations are emitted for that evaluator
