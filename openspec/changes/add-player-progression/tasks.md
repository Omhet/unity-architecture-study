## 1. Core Implementation (Progression)

- [ ] 1.1 Create `ProgressionState` in `App.Core.Progression` with `Level`, `Xp`, and `NextLevelXp` reactive properties
- [ ] 1.2 Create `ProgressionEntry` struct and `ProgressionRegistry` to hold level milestones
- [ ] 1.3 Create `ProgressionService` with `AddXp` logic and level-up calculations
- [ ] 1.4 Implement level transition logic in `ProgressionService` (finding next threshold from registry)

## 2. Configuration & Bootstrapping

- [ ] 2.1 Create `ProgressionCatalogConfig` and `ProgressionConfigModule`
- [ ] 2.2 Register `ProgressionState`, `ProgressionRegistry`, and `ProgressionService` in `RootLifetimeScope`
- [ ] 2.3 Register `ProgressionConfigModule` as `IConfigModule` in `RootLifetimeScope`
- [ ] 2.4 Add progression configuration to the game catalog JSON (simulated or real addressable asset)

## 3. Flow Implementation

- [ ] 3.1 Create `LevelUpFlowHandler` in `App.Flow.Handlers`
- [ ] 3.2 Implement `IInitializable` in `LevelUpFlowHandler` to subscribe to `Level` changes
- [ ] 3.3 Call `ShopService.RefreshAvailability` on level change and at initial startup
- [ ] 3.4 Register `LevelUpFlowHandler` in `GameLifetimeScope`

## 4. UI Implementation (View)

- [ ] 4.1 Update `HudShellView` to include `ProgressionState` dependency
- [ ] 4.2 Modify `BuildStatusBar` to add Level and XP labels
- [ ] 4.3 Bind `ProgressionState.Level` and `ProgressionState.Xp` to UI elements in `BindView`
- [ ] 4.4 Update `UnbindView` and `DisposeView` for new subscriptions

## 5. Verification

- [ ] 5.1 Verify that adding XP correctly triggers UI updates
- [ ] 5.2 Verify that reaching XP threshold increments level
- [ ] 5.3 Verify that shop items unlock automatically on level up
