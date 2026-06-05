## Context

The game has established domain modules (Generators, Resources, Products, Recipes, Economy, Progression, Shop) following a consistent pattern: pure reactive State, Service for domain logic, Registry for config lookup, CatalogConfig for JSON deserialization, with FlowHandlers bridging events to services. The quest system is a new domain that integrates with existing systems — it observes economy/resources/products state and awards XP through ProgressionService.

## Goals / Non-Goals

**Goals:**

- All quests from catalog are active simultaneously on game start (no chain, no unlocking)
- Quests have conditions evaluated reactively via strategy pattern evaluators
- Claimable status is state-carried (`ReactiveProperty<bool>`) — view binds directly with zero logic
- Completed quests remain visible as history (achievement-style, one-time claim)
- State is pure serializable data; behavior lives in Service
- Extensible condition types without modifying quest infrastructure (SOLID: Open/Closed Principle)

**Non-Goals:**

- Quest chains or branching tracks
- Repeatable/resettable quests
- Quest expiration or failure
- Quest prerequisites or dependencies between quests
- Runtime serialization of evaluators (reconstructed from config on load)

## Decisions

### 1. FlowHandler Owns Evaluator Subscriptions, Not Service or State

**Decision:** `IConditionEvaluator` instances are created by the service (factory method) but subscribed to and managed by `QuestFlowHandler`. The service provides evaluators; the flow handler wires observations into state.

**Rationale:**

- Keeps QuestService as pure domain logic — no R3 subscriptions, no reactive wiring
- Flow handlers already own observation patterns in this codebase (they bridge events to services)
- Separation of concerns: Service = "what can I do?", FlowHandler = "when should it happen?"
- On save/load: state restores flags, flow handler reconstructs evaluators and re-wires on game start

**Alternatives considered:**

- Evaluator reference in ActiveQuest → breaks serialization, mixes behavior with data
- Service owns subscriptions → couples domain logic to reactive infrastructure, harder to test

### 2. Strategy Pattern for Conditions (IConditionEvaluator)

**Decision:** Interface with two methods: `bool IsMet()` (snapshot check) and `Observable<bool> Observe()` (reactive stream).

**Rationale:**

- Adding a new condition type requires only one new class + one factory switch arm — zero changes to quest infrastructure
- The interface encapsulates both the check AND the observation, so callers don't need domain knowledge
- `Observe()` returns an R3 Observable that fires `true` when condition is met, fitting the existing reactive pattern

**Alternatives considered:**

- Enum-based type switching in Service → violates Open/Closed, requires modifying Service for each new type
- ScriptableObject evaluators → Unity-specific overhead, unnecessary complexity for data-driven conditions

### 3. Flat ConditionData POCO with Type Discriminator

**Decision:** Single `ConditionData` class with `Type` (string discriminator), `TargetId` (nullable), and `Threshold` (int).

**Rationale:**

- All three initial condition types share the same field shape — no waste
- Simple JSON deserialization, no polymorphic attributes needed
- Factory in Service maps `Type` string to concrete evaluator class
- Extensible: new fields can be added if future conditions need them

**Alternatives considered:**

- Separate POCO per condition type → requires `[JsonPolymorphic]` or manual deserialization logic
- Flat fields on QuestDefinition → scatters condition data at the same level as quest metadata

### 4. State-Carried Claimable Status (ReactiveProperty<bool>)

**Decision:** `ActiveQuest.IsClaimable` is a `ReactiveProperty<bool>` that the FlowHandler wires up via evaluator observations.

**Rationale:**

- View binds directly to the property — zero conditional logic in UI layer
- FlowHandler controls when it flips: evaluator's `Observe()` fires → handler sets `IsClaimable.Value = true`
- On claim (routed through FlowHandler): `IsCompleted = true`, subscription disposed, `IsClaimable` effectively frozen

**Alternatives considered:**

- View queries Service on demand → requires polling or explicit trigger events from view
- Service owns subscriptions → couples domain logic to reactive infrastructure

### 5. Quest Catalog Validation with Cross-Reference Checks

**Decision:** `QuestConfigModule.Validate()` checks that condition `TargetId` references valid resource/product IDs from other catalogs.

**Rationale:**

- Fails fast on boot rather than silently failing at runtime when a quest can never be claimed
- Follows existing pattern: `RecipeConfigModule` validates input resources and output products against their catalogs
- Uses established helpers: `ConfigValidationHelper.BuildIdSet()` for cross-reference sets

## Risks / Trade-offs

| Risk                                                                | Mitigation                                                                                                               |
| ------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| Evaluator subscriptions leak if not disposed on claim               | FlowHandler explicitly disposes subscription in On(ClaimQuestEvent); completed quests skip wiring during game start init |
| Condition type string typos in JSON → runtime error                 | Validation catches unknown types; factory throws with clear message for unhandled types                                  |
| All quests active = performance concern with hundreds of evaluators | Not a current concern; catalog is data-driven and small. Can add pagination/filtering to view later                      |
| Evaluator reconstruction on load must match original behavior       | Evaluators are deterministic functions of config + state — no randomness, always reproducible                            |

## Component Map

```
Assets/_Game/Scripts/Core/Quests/
  QuestCatalogConfig.cs        ← [Serializable] JSON POCOs (QuestDefinition, ConditionData)
  QuestRegistry.cs             ← Load from config, lookup by ID
  QuestState.cs                ← ObservableList<ActiveQuest>, pure reactive data
  QuestService.cs              ← Claim(), CreateEvaluator() factory — no subscriptions

Assets/_Game/Scripts/Core/Quests/Conditions/
  IConditionEvaluator.cs       ← interface: IsMet(), Observe()
  MoneyThresholdEvaluator.cs   ← EconomyState dependency
  ResourceThresholdEvaluator.cs← ResourceState dependency
  ProductThresholdEvaluator.cs ← ProductState dependency

Assets/_Game/Scripts/Flow/Handlers/
  QuestFlowHandler.cs          ← On(GameStartEvent) → create evaluators + wire observations,
                                ←   On(ClaimQuestEvent) → claim, dispose subscriptions

Assets/_Game/Scripts/View/Quests/
  QuestSectionView.cs          ← HUD section, binds to ActiveQuests list + IsClaimable/IsCompleted

Assets/_Game/Scripts/Boot/ConfigModules/
  QuestConfigModule.cs         ← Deserialize, Validate (cross-ref), Hydrate

Assets/_Game/Configs/
  quests_catalog.json          ← quest definitions with conditions
```
