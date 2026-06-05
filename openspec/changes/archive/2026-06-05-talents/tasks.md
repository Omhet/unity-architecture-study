## 1. Core — Talent Data Models & Config

- [x] 1.1 Create `TalentEntry.cs` in `Core/Talents/` with serializable fields: Id, Name, Cost, IncreasePerPoint, MaxPoints
- [x] 1.2 Create `TalentCatalogConfig.cs` in `Core/Talents/` as a serializable wrapper holding `TalentEntry[] Talents`
- [x] 1.3 Create `TalentConfigModule.cs` in `Boot/ConfigModules/` implementing `IConfigModule` to load talent config from JSON

## 2. Core — TalentState, Registry, Service

- [x] 2.1 Create `TalentState.cs` with `ReactiveProperty<int> AvailablePoints` and `Dictionary<string, int> PointsSpent`
- [x] 2.2 Register `TalentState` in VContainer (add to appropriate lifetime scope or config bootstrap)
- [x] 2.3 Create `TalentRegistry.cs` with `Load(TalentEntry[])` and `TryGetById(string id, out TalentEntry)` methods
- [x] 2.4 Wire `TalentConfigModule` to hydrate `TalentRegistry` during config initialization
- [x] 2.5 Create `TalentService.cs` with: `AddPoint()`, `TryPurchase(talentId)`, `GetMultiplier(talentId)`, `GetPointsSpent(talentId)` — inject TalentState + TalentRegistry

## 3. Core — Apply Multipliers in Existing Services

- [x] 3.1 Inject `TalentService` into `GeneratorService` and apply generator boost multiplier to resource output amount using `(int)Math.Ceiling(baseAmount * multiplier)`
- [x] 3.2 Inject `TalentService` into `CraftService` and apply craft boost multiplier to product output amount
- [x] 3.3 Inject `TalentService` into `OrderService` and apply order boost multiplier to money reward

## 4. Flow — Events, Handlers, Level-Up Integration

- [x] 4.1 Add `PurchaseTalentEvent : ICommand` struct to `Flow/Events/Events.cs` with `TalentId` field
- [x] 4.2 Create `TalentFlowHandler.cs` in `Flow/Handlers/` that routes `PurchaseTalentEvent` → `TalentService.TryPurchase()`
- [x] 4.3 Modify `LevelUpFlowHandler.cs` to inject `TalentService` and call `AddPoint()` for each level increment in `HandleLevelChange`
- [x] 4.4 Register `TalentFlowHandler` in `GameLifetimeScope.Configure()` VitalRouter mapping

## 5. View — TalentSectionView

- [x] 5.1 Create `TalentSectionView.cs` extending `GameplaySectionViewBase` with dependencies: TalentState, TalentRegistry, TalentService, ICommandPublisher
- [x] 5.2 Implement `BuildContent` — display available points balance label and talent list container
- [x] 5.3 Implement talent row builder — show name, current/max investment (e.g., "1/10"), buy button
- [x] 5.4 Implement `Bind` — subscribe to `AvailablePoints` changes and `PointsSpent` updates for reactive UI refresh
- [x] 5.5 Wire buy buttons to publish `PurchaseTalentEvent`, disable button when max reached or insufficient points

## 6. View — Register Talent Section in HUD

- [x] 6.1 Update `HudSectionFactory.cs` to inject talent dependencies and replace the "talents" placeholder case with a real `TalentSectionView`
- [x] 6.2 Verify the existing "talents" entry in `HudSectionRegistry` definitions renders correctly (already exists as placeholder)

## 7. Config — Create Talent JSON Data

- [x] 7.1 Create talent config JSON file with three talents: `generator_boost`, `craft_boost`, `order_boost` (cost=1, increasePerPoint=0.1, maxPoints=10)
- [x] 7.2 Register the JSON config in the config loading pipeline so `TalentConfigModule` can find it
