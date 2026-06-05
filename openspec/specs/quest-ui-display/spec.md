# Quest UI Display

## Purpose

Provide reactive HUD views for displaying quest cards, claim buttons, and completion indicators driven by quest state.

## ADDED Requirements

### Requirement: Quest section view displays all active quests

The system SHALL provide a HUD section view that renders one card per ActiveQuest from QuestState.ActiveQuests.

#### Scenario: All quests visible in list

- **WHEN** QuestState contains 3 active quests
- **THEN** the view renders 3 quest cards

#### Scenario: Empty state when no quests

- **WHEN** QuestState.ActiveQuests is empty
- **THEN** the view shows an empty list or placeholder

### Requirement: Quest card shows claim button based on claimable status

The system SHALL render a claim button that is enabled only when the quest's IsClaimable is true and IsCompleted is false.

#### Scenario: Claim button enabled when condition met

- **WHEN** a quest's IsClaimable is true and IsCompleted is false
- **THEN** the quest card displays an enabled "Claim" button

#### Scenario: Claim button disabled when condition not met

- **WHEN** a quest's IsClaimable is false and IsCompleted is false
- **THEN** the quest card displays a disabled "Claim" button

### Requirement: Completed quest shows checkmark instead of claim button

The system SHALL display a completed indicator (checkmark) instead of a claim button when the quest's IsCompleted is true.

#### Scenario: Checkmark shown for completed quest

- **WHEN** a quest's IsCompleted is true
- **THEN** the quest card displays a checkmark indicator and no claim button

### Requirement: Claim button publishes command to flow handler

The system SHALL publish a ClaimQuestEvent (ICommand) when the player clicks an enabled claim button, which the QuestFlowHandler routes to QuestService.Claim().

#### Scenario: Click publishes claim event

- **WHEN** the player clicks an enabled "Claim" button on a quest card
- **THEN** a ClaimQuestEvent with the quest's Id is published via ICommandPublisher

### Requirement: View reactively updates when quest state changes

The system SHALL subscribe to QuestState.ActiveQuests and each quest's IsClaimable/IsCompleted reactive properties so the UI updates automatically without manual refresh.

#### Scenario: UI updates when claimable status changes

- **WHEN** a quest's IsClaimable property changes from false to true
- **THEN** the corresponding quest card's claim button becomes enabled without user interaction

#### Scenario: UI updates when completed status changes

- **WHEN** a quest's IsCompleted property changes from false to true
- **THEN** the corresponding quest card replaces the claim button with a checkmark indicator
