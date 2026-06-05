# Quest Management

## Purpose

Manage the quest lifecycle including loading from catalog, reactive condition evaluation, claiming rewards, and maintaining quest history.

## ADDED Requirements

### Requirement: All quests from catalog are active on game start

The system SHALL load all quest definitions from the quest catalog and create an ActiveQuest instance for each one when the game starts.

#### Scenario: Quests loaded from catalog

- **WHEN** the game starts and the quest catalog is loaded
- **THEN** every quest definition in the catalog has a corresponding ActiveQuest instance in QuestState

#### Scenario: Empty catalog produces no quests

- **WHEN** the quest catalog contains no quest definitions
- **THEN** QuestState.ActiveQuests is empty

### Requirement: Quest conditions are evaluated reactively

The system SHALL evaluate each quest's condition by creating an IConditionEvaluator (via QuestService factory) that the QuestFlowHandler subscribes to, observing relevant game state and signaling when the condition is met.

#### Scenario: Money threshold condition met

- **WHEN** a quest has a money_threshold condition with threshold 100
- **AND** the player's economy balance reaches 100 or more
- **THEN** the QuestFlowHandler receives the observation and sets the quest's IsClaimable property to true

#### Scenario: Resource threshold condition met

- **WHEN** a quest has a resource_threshold condition targeting "wood" with threshold 20
- **AND** the player's wood resource amount reaches 20 or more
- **THEN** the QuestFlowHandler receives the observation and sets the quest's IsClaimable property to true

#### Scenario: Product threshold condition met

- **WHEN** a quest has a product_threshold condition targeting "wooden_hammer" with threshold 1
- **AND** the player's wooden_hammer product amount reaches 1 or more
- **THEN** the QuestFlowHandler receives the observation and sets the quest's IsClaimable property to true

### Requirement: Quest claim awards XP and marks completed

The system SHALL allow the player to claim a quest when its condition is met, awarding XP through ProgressionService and marking the quest as permanently completed.

#### Scenario: Successful claim

- **WHEN** a quest's IsClaimable is true and IsCompleted is false
- **AND** the player triggers a claim for that quest
- **THEN** the quest's XpReward is added to progression via ProgressionService.AddXp()
- **AND** the quest's IsCompleted becomes true

#### Scenario: Cannot claim unmet quest

- **WHEN** a quest's IsClaimable is false
- **AND** the player triggers a claim for that quest
- **THEN** no XP is awarded and IsCompleted remains false

#### Scenario: Cannot re-claim completed quest

- **WHEN** a quest's IsCompleted is true
- **AND** the player triggers a claim for that quest
- **THEN** no XP is awarded and state does not change

### Requirement: Completed quests remain visible as history

The system SHALL keep completed quests in the active list so they remain visible to the player with a completed indicator.

#### Scenario: Quest visible after completion

- **WHEN** a quest has been claimed and IsCompleted is true
- **THEN** the quest remains in QuestState.ActiveQuests
- **AND** the view displays it with a completed indicator instead of a claim button

### Requirement: Condition evaluators are extensible via strategy pattern

The system SHALL use an IConditionEvaluator interface so new condition types can be added without modifying existing quest infrastructure.

#### Scenario: New condition type added

- **WHEN** a new IConditionEvaluator implementation is created for a new condition type
- **AND** the factory in QuestService maps the new type string to the evaluator
- **THEN** quests with that condition type are evaluated correctly without changes to QuestState, QuestRegistry, or existing evaluators

### Requirement: Quest state is serializable data without behavior

The system SHALL store quest state as pure reactive data (strings, ints, bools) without references to interfaces or behavioral objects.

#### Scenario: ActiveQuest contains only serializable fields

- **WHEN** inspecting the ActiveQuest type
- **THEN** it contains only Id (string), XpReward (int), IsClaimable (ReactiveProperty<bool>), and IsCompleted (ReactiveProperty<bool>)
- **AND** it does NOT contain references to IConditionEvaluator or other behavioral interfaces

### Requirement: Quest catalog validation checks cross-references

The system SHALL validate that quest condition targetIds reference valid resource or product IDs from their respective catalogs during configuration loading.

#### Scenario: Valid resource target passes validation

- **WHEN** a quest has a resource_threshold condition with targetId "wood"
- **AND** the resources catalog contains a resource with id "wood"
- **THEN** no validation error is reported

#### Scenario: Invalid resource target fails validation

- **WHEN** a quest has a resource_threshold condition with targetId "diamond"
- **AND** the resources catalog does not contain a resource with id "diamond"
- **THEN** a validation error is reported identifying the unknown resource reference

#### Scenario: Valid product target passes validation

- **WHEN** a quest has a product_threshold condition with targetId "wooden_hammer"
- **AND** the products catalog contains a product with id "wooden_hammer"
- **THEN** no validation error is reported

#### Scenario: Money threshold requires no target validation

- **WHEN** a quest has a money_threshold condition
- **THEN** no cross-reference validation is performed for targetId
