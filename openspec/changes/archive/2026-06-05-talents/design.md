## Context

The game currently has three services that produce outputs with hardcoded quantities of 1:

- `GeneratorService.TryGenerate()` — adds 1 resource unit
- `CraftService.Craft()` — produces 1 product unit
- `OrderService.TryCompleteOrder()` — rewards money directly from order definition

Each service follows the established pattern: State (reactive), Registry (config lookup), Service (business logic). The player progression system tracks level/XP and triggers shop refreshes on level-up via `LevelUpFlowHandler`.

A talent system introduces a cross-cutting modifier layer that scales these outputs based on player investment, plus its own state management for points and purchases.

## Goals / Non-Goals

**Goals:**

- Give players meaningful progression choices beyond leveling up
- Keep the system fully data-driven (all talent parameters in config)
- Follow existing architecture patterns (State/Registry/Service/Flow/View)
- Minimize coupling — services query multipliers from `TalentService`, not the other way around

**Non-Goals:**

- Talent trees or prerequisites between talents
- Talent leveling or tiered unlocks
- UI animations, tooltips, or confirmation dialogs
- Server-side validation (single-player only)

## Decisions

### D1: Stackable purchases with per-talent point caps

Each talent can be purchased multiple times (costing 1 point each), up to a `MaxPoints` limit defined in config. State tracks `Dictionary<string, int> PointsSpent` rather than a simple set of owned IDs. This gives players incremental investment choices instead of binary unlock decisions.

### D2: Config-driven parameters per talent

Each `TalentEntry` defines `Cost`, `IncreasePerPoint`, and `MaxPoints`. No magic numbers in code — the same system supports `+10% × 10 points` or `+25% × 4 points` with only config changes.

### D3: Services inject TalentService for multipliers (Option A)

`GeneratorService`, `CraftService`, `OrderService` each depend on `TalentService.GetMultiplier(talentId)` and apply it to their output amounts. This is consistent with how services already take dependencies (`EconomyService` in `OrderService`). Alternative considered: flow handlers applying multipliers, but that would require services to return base values rather than modifying state directly — a bigger architectural shift for minimal benefit.

### D4: Multiplier formula is `1.0f + (pointsSpent × IncreasePerPoint)`

Simple linear scaling. At 0 points the multiplier is 1.0x (base). Output amounts use `(int)Math.Ceiling(baseAmount * multiplier)` to ensure minimum 1 unit production and round up fractional gains.

### D5: Level-up grants exactly 1 talent point

`LevelUpFlowHandler` calls `TalentService.AddPoint()` on each level increment. Simple, predictable pacing — players earn points at the same rate they level.

### D6: Talents available from level 1 (no gating)

Unlike shop items that unlock via progression registry, all talents are immediately purchasable if the player has points. No `TalentProgressionRegistry` needed.

## Risks / Trade-offs

- [Services coupled to TalentService] → Acceptable; consistent with existing cross-service dependencies. If talents were ever removed, three services need a dependency removed — straightforward.
- [No talent prerequisites limits strategic depth] → By design for v1. Can be added later by adding `PrerequisiteTalentIds` to `TalentEntry`.
- [Linear scaling may feel flat at high levels] → Config-driven; can switch to exponential curves per-talent without code changes by adjusting `IncreasePerPoint` logic in the service.
