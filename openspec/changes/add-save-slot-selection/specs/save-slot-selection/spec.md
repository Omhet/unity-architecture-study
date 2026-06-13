## ADDED Requirements

### Requirement: Save Slots Panel Displays in Main Menu

The main menu SHALL provide a save slots panel that lists all configured save slots with metadata and action buttons, toggled in-place within the MainMenuView.

#### Scenario: Player opens slots panel from main menu

- **WHEN** the player clicks the "Start Game" button on the main menu
- **THEN** the title and Start Game button SHALL be hidden and the save slots panel SHALL become visible

#### Scenario: Player returns to main menu from slots panel

- **WHEN** the player clicks a back control in the save slots panel
- **THEN** the save slots panel SHALL be hidden and the title and Start Game button SHALL become visible again

### Requirement: Slot Rows Show Metadata

Each slot row in the save slots panel SHALL display the slot index, last played timestamp (if the slot has data), or an "(empty)" indicator (if the slot has no data).

#### Scenario: Non-empty slot displays last played date

- **WHEN** a slot contains save data with a `LastPlayed` timestamp
- **THEN** the slot row SHALL display the slot index and the formatted last played date

#### Scenario: Empty slot displays empty indicator

- **WHEN** a slot has no save data
- **THEN** the slot row SHALL display the slot index and an "(empty)" indicator instead of a date

### Requirement: Slot Play Button Sets Active Slot and Loads Game

Each slot row SHALL have a Play button that sets the clicked slot as active and triggers the game load flow.

#### Scenario: Player clicks Play on any slot

- **WHEN** the player clicks the Play button on a slot row
- **THEN** the system SHALL set that slot as the active slot via `SlotManager.SetActiveSlot(slotIndex)` and SHALL publish `LoadGameEvent` to trigger scene transition

#### Scenario: Play on empty slot loads fresh state

- **WHEN** the player clicks Play on an empty slot (no save data)
- **THEN** the system SHALL set that slot as active and load it, resulting in default domain state (same behavior as loading a non-existent slot via `SaveLoadSystem.LoadSlotAsync`)

### Requirement: Slot Delete Button Removes Save Data

Non-empty slot rows SHALL have a Delete button that immediately removes the slot's save data without confirmation.

#### Scenario: Player deletes a non-empty slot

- **WHEN** the player clicks the Delete button on a non-empty slot row
- **THEN** the system SHALL delete the slot's save data via `SlotManager.DeleteSlotAsync(slotIndex)` and SHALL refresh the slots panel to reflect the change

#### Scenario: Empty slots do not show Delete button

- **WHEN** a slot has no save data
- **THEN** the slot row SHALL NOT display a Delete button

### Requirement: Slots Panel Refreshes After Deletion

After a slot is deleted, the save slots panel SHALL refresh to reflect the updated slot state.

#### Scenario: Panel updates after delete

- **WHEN** a slot deletion completes successfully
- **THEN** the slots panel SHALL re-query `SlotManager.ListSlotsAsync()` and update all row displays accordingly
