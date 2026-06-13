# Quest Management

## MODIFIED Requirements

### Requirement: All quests from catalog have progress entries on game start

The system SHALL ensure every quest definition in the catalog has a corresponding entry in `QuestState.ProgressMap` when `StartGameEvent` is published, restoring saved state where available.

#### Scenario: Progress entries created with saved state restored

- **WHEN** the game starts and `StartGameEvent` is published
- **AND** `QuestState.ProgressMap` contains saved progress for some quests (from save module)
- **THEN** every quest definition in the catalog has a `QuestProgressData` entry in `ProgressMap`
- **AND** each entry restores its `IsCompleted.Value` and `IsClaimable.Value` from the saved data if available

#### Scenario: Progress entries created with default state for new quests

- **WHEN** the game starts and `StartGameEvent` is published
- **AND** the quest registry contains definitions that have no saved progress entry
- **THEN** those quests get a new `QuestProgressData` entry with default state (`IsCompleted.Value = false`, `IsClaimable.Value = false`)

#### Scenario: Empty catalog produces empty progress map

- **WHEN** the quest catalog contains no quest definitions
- **AND** `StartGameEvent` is published
- **THEN** `QuestState.ProgressMap` remains empty

### Requirement: Quest conditions are evaluated reactively for non-completed quests

The system SHALL create condition evaluators for active (non-completed) quests and subscribe their observations to update the quest's progress `IsClaimable` reactive property. Already-completed quests SHALL NOT have an evaluator wired.

#### Scenario: Evaluator created for in-progress quest

- **WHEN** a quest definition has condition data
- **AND** the quest is not completed according to saved progress
- **THEN** an evaluator is created and subscribed so that when the condition is met, `ProgressMap[questId].IsClaimable.Value` becomes true

#### Scenario: No evaluator for completed quest

- **WHEN** a quest definition has condition data
- **AND** the quest is already completed according to saved progress
- **THEN** no evaluator is created or subscribed for that quest

### Requirement: Quest claim updates reactive progress properties

The system SHALL update `QuestProgressData.IsCompleted.Value` to true when a quest is claimed, instead of modifying an `ActiveQuest` instance.

#### Scenario: Successful claim updates progress

- **WHEN** a quest's `IsClaimable.Value` is true and `IsCompleted.Value` is false
- **AND** the player triggers a claim for that quest
- **THEN** the quest's `XpReward` (from registry) is added to progression via `ProgressionService.AddXp()`
- **AND** the quest's `ProgressMap[questId].IsCompleted.Value` becomes true

### Requirement: Quest state contains only progress data without behavior

The system SHALL store quest state as reactive properties in a dictionary keyed by quest ID, without references to interfaces or behavioral objects.

#### Scenario: ProgressData contains only reactive properties

- **WHEN** inspecting the `QuestProgressData` type
- **THEN** it contains only `IsClaimable` (`ReactiveProperty<bool>`) and `IsCompleted` (`ReactiveProperty<bool>`)
- **AND** it does NOT contain references to `IConditionEvaluator`, quest ID, XP reward, or other behavioral/config data

### Requirement: Quest catalog validation checks cross-references

The system SHALL validate that quest condition target IDs reference valid resources or products in their respective registries.

#### Scenario: Valid quest references pass validation

- **WHEN** all quest condition `targetId` values reference existing resource or product definitions
- **THEN** no validation errors are reported

## REMOVED Requirements

### Requirement: ActiveQuest instance per quest definition

**Reason**: Replaced by `ProgressMap` with reactive properties. Registry is the single source of truth for quest definitions; state tracks only player progress. No need to duplicate static config data (ID, XP reward) in runtime objects.

**Migration**: Replace all references to `ActiveQuest` with lookups into `QuestState.ProgressMap[questId]`. Quest definition data (ID, XP reward, condition) is read from `QuestRegistry`.
