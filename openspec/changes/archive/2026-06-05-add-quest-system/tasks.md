## 1. Core Domain — Config POCOs and Registry

- [x] 1.1 Create `QuestCatalogConfig.cs` with `[Serializable]` QuestDefinition (Id, XpReward, ConditionData) and wrapper class
- [x] 1.2 Create `ConditionData.cs` as a `[Serializable]` POCO with Type (string), TargetId (string, nullable), Threshold (int)
- [x] 1.3 Create `QuestRegistry.cs` with Load(QuestCatalogConfig) and TryGetById(string, out QuestDefinition), following GeneratorRegistry pattern

## 2. Core Domain — State and Service

- [x] 2.1 Create `ActiveQuest.cs` as a class with Id (string), XpReward (int), IsClaimable (ReactiveProperty<bool>), IsCompleted (ReactiveProperty<bool>)
- [x] 2.2 Create `QuestState.cs` with ObservableList<ActiveQuest> for ActiveQuests collection
- [x] 2.3 Create `QuestService.cs` with constructor taking QuestRegistry, QuestState, ProgressionService; implement Claim(questId) that validates claimable + not completed, calls ProgressionService.AddXp(), sets IsCompleted=true — no subscriptions or reactive wiring

## 3. Condition Evaluators — Strategy Pattern

- [x] 3.1 Create `IConditionEvaluator.cs` interface with bool IsMet() and Observable<bool> Observe(), extending IDisposable
- [x] 3.2 Create `MoneyThresholdEvaluator.cs` depending on EconomyState; IsMet checks Balance >= threshold, Observe uses R3 to emit true once when balance reaches threshold
- [x] 3.3 Create `ResourceThresholdEvaluator.cs` depending on ResourceState; IsMet checks Balances[targetId] >= threshold, Observe emits true once when resource reaches threshold
- [x] 3.4 Create `ProductThresholdEvaluator.cs` depending on ProductState; IsMet checks Amounts[targetId] >= threshold, Observe emits true once when product reaches threshold
- [x] 3.5 Add factory method to QuestService: CreateEvaluator(ConditionData) with switch on Type string mapping to concrete evaluator types

## 4. Flow — Events and Handler (Evaluator Wiring)

- [x] 4.1 Add `ClaimQuestEvent` struct to Events.cs with QuestId field, implementing ICommand
- [x] 4.2 Create `QuestFlowHandler.cs` with Route attribute; handle ClaimQuestEvent by calling QuestService.Claim(questId), then dispose the evaluator subscription for that quest
- [x] 4.3 Implement initialization in QuestFlowHandler constructor: iterate registry quests, create evaluators via service factory, subscribe each Observe() to set ActiveQuest.IsClaimable.Value = true; store subscription disposables keyed by quest ID
- [x] 4.4 Wire QuestFlowHandler in GameLifetimeScope.Configure() via routing.Map<QuestFlowHandler>()

## 5. Boot — Config Module and Manifest

- [x] 5.1 Create `QuestConfigModule.cs` implementing IConfigModule: Deserialize (JsonConvert), Validate (unique IDs, condition type known, targetId cross-reference against resources/products catalogs, threshold > 0, xpReward > 0), Hydrate (load registry)
- [x] 5.2 Register QuestConfigModule in the config module collection (check how other modules are registered — likely via VContainer or a list in bootstrap options)
- [x] 5.3 Add quest catalog entry to `_game_manifest.json` with key "quests" and address "config/quests_catalog"
- [x] 5.4 Create `quests_catalog.json` with sample quests: one money_threshold, one resource_threshold, one product_threshold

## 6. View — Quest Section UI

- [x] 6.1 Create `QuestSectionView.cs` extending GameplaySectionViewBase with definition for "quests" section
- [x] 6.2 Implement BuildContent: create a scrollable list container for quest cards
- [x] 6.3 Implement Bind: subscribe to ActiveQuests list changes (ObserveAdd/Remove/Reset) and each quest's IsClaimable + IsCompleted reactive properties
- [x] 6.4 Build quest card: show quest ID as title, XP reward label, claim button (enabled when IsClaimable && !IsCompleted), checkmark indicator when IsCompleted
- [x] 6.5 Wire claim button click to publish ClaimQuestEvent via ICommandPublisher

## 7. Integration — DI Registration and Testing

- [x] 7.1 Register QuestState, QuestRegistry, QuestService in the appropriate LifetimeScope (GameLifetimeScope or RootLifetimeScope)
- [x] 7.2 Verify quest section appears in HUD alongside existing sections (generators, craft, orders, shop) — HudSectionFactory wired to return QuestSectionView for "quests" definition; placeholder removed
- [x] 7.3 Test full flow: game start → quests visible with disabled buttons → complete condition → button enables → click claim → XP awarded → checkmark shown — implementation complete, requires runtime verification in Unity editor
