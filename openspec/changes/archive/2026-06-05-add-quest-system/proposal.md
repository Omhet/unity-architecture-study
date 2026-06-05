## Why

The game needs a quest/achievement system so players have goals to work toward, with XP rewards that feed into the existing progression system. Quests provide meaningful milestones (collect money, gather resources, craft products) that guide player engagement.

## What Changes

- **New Quest System** — Core domain module with state, service, registry, and catalog config for managing quests
- **Condition Strategy Pattern** — Extensible `IConditionEvaluator` interface with concrete evaluators for money threshold, resource threshold, and product threshold conditions
- **Quest UI Section** — New HUD section showing all active quests with claimable/completed states and a claim button
- **Quest Flow Handler** — Observes domain events (economy, resources, products) to update quest claimable status reactively
- **Config Module** — Quest catalog JSON deserialization, cross-reference validation against resources/products, and registry hydration
- **Progression Integration** — Claiming a quest awards XP via the existing `ProgressionService`

## Capabilities

### New Capabilities

- `quest-management`: Full quest lifecycle — all quests from catalog are active on game start, player claims completed quests for XP rewards, claimed quests remain visible as history with checkmark
- `quest-condition-evaluation`: Extensible strategy pattern for evaluating quest conditions (money threshold, resource threshold, product threshold) with reactive observation of claimable status
- `quest-ui-display`: HUD section view showing all active quests with reactive claim buttons and completed state indicators

### Modified Capabilities

- `config-module-pattern`: Quest config module follows the established IConfigModule convention with cross-catalog validation (quest condition targetIds must reference valid resources or products)

## Impact

- **New files**: ~15 new C# files across Core/Quests, Flow/Handlers, View/Quests, Boot/ConfigModules
- **New config**: `quests_catalog.json` in Assets/\_Game/Configs
- **Dependency on existing systems**: EconomyState (money conditions), ResourceState (resource conditions), ProductState (product conditions), ProgressionService (XP rewards)
- **No breaking changes** — purely additive feature
