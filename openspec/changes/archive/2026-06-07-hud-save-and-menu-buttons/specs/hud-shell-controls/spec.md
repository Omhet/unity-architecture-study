## ADDED Requirements

### Requirement: HUD shell displays a top action row with Save and Back to Menu buttons

The HUD shell view SHALL render an action row above the status bar containing two buttons: a "Save" button on the left and a "Back to Menu" button on the right.

#### Scenario: Action row is visible during gameplay

- **WHEN** the HudShellView builds its view
- **THEN** an action row element SHALL be present above the status bar in the visual hierarchy

#### Scenario: Save button is rendered on the left side

- **WHEN** the action row is built
- **THEN** a "Save" button SHALL appear positioned at the left edge of the row

#### Scenario: Back to Menu button is rendered on the right side

- **WHEN** the action row is built
- **THEN** a "Back to Menu" button SHALL appear positioned at the right edge of the row

### Requirement: Save button publishes ManualSaveEvent through ICommandPublisher

The HUD shell view SHALL publish a `ManualSaveEvent` command when the Save button is clicked.

#### Scenario: Clicking Save button triggers event

- **WHEN** the user clicks the Save button
- **THEN** the HudShellView SHALL call `_publisher.PublishAsync(new ManualSaveEvent())`

### Requirement: Back to Menu button publishes ExitToMenuEvent through ICommandPublisher

The HUD shell view SHALL publish an `ExitToMenuEvent` command when the Back to Menu button is clicked.

#### Scenario: Clicking Back to Menu button triggers event

- **WHEN** the user clicks the Back to Menu button
- **THEN** the HudShellView SHALL call `_publisher.PublishAsync(new ExitToMenuEvent())`

### Requirement: SaveFlowHandler saves the active slot on ManualSaveEvent

The `SaveFlowHandler` SHALL handle `ManualSaveEvent` by saving the currently active save slot.

#### Scenario: Manual save event triggers save pipeline

- **WHEN** a `ManualSaveEvent` is published and routed to `SaveFlowHandler`
- **THEN** the handler SHALL call `SlotManager.GetActiveSlot()` to obtain the active slot index
- **AND THEN** the handler SHALL await `SaveLoadSystem.SaveSlotAsync(activeSlot)` to persist game state

### Requirement: ManualSaveEvent is a command struct in Flow.Events namespace

The system SHALL define `ManualSaveEvent` as a readonly struct implementing `ICommand` in the `App.Flow.Events` namespace.

#### Scenario: Event type exists and implements ICommand

- **WHEN** the codebase is compiled
- **THEN** `App.Flow.Events.ManualSaveEvent` SHALL exist as a readonly struct implementing `VitalRouter.ICommand`

### Requirement: SaveFlowHandler is registered in the VitalRouter pipeline

The `SaveFlowHandler` SHALL be mapped in the game lifetime scope's VitalRouter configuration.

#### Scenario: Handler is registered for routing

- **WHEN** `GameLifetimeScope.Configure()` executes
- **THEN** `SaveFlowHandler` SHALL be included in the `routing.Map<T>()` calls within `RegisterVitalRouter`

### Requirement: Action row buttons match existing HUD styling conventions

The action row and its buttons SHALL use USS classes consistent with existing HUD elements (rounded corners, semi-transparent backgrounds, hover states).

#### Scenario: Buttons have consistent visual style

- **WHEN** the action row is rendered
- **THEN** the buttons SHALL use `.hud-action-button` class with styling matching `.hud-tab-button` conventions (border-radius, background-color, color, hover state)

#### Scenario: Action row layout separates buttons to opposite edges

- **WHEN** the action row is rendered
- **THEN** the row SHALL use `justify-content: space-between` to position Save on the left and Back to Menu on the right
