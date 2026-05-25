# Modern Unity Clean Architecture Guide (2026)

This project follows a strict, highly decoupled Modern Unity Architecture stack. All AI agents and developers must adhere to these rules when generating or modifying code.

## 1. The Core Tech Stack

- **Dependency Injection:** VContainer
- **Asynchronous Flow:** UniTask
- **Reactive State (Rx):** R3 (State management & local bindings)
- **Event Bus & Command Sourcing:** VitalRouter (Decoupled Domain Event broadcasting and Intent logging via zero-allocation structs)
- **Animations/Tweens:** PrimeTween, Animancer
- **Data & Saving:** Addressables (Asset Loading), Newtonsoft.Json (Human-readable Saves Configs)
- **Input:** Unity New Input System

## 2. Project Structure & Namespaces (`App.Feature.Layer`)

All game specific code resides in `Assets/_Game/` to maintain clean separation from third-party plugins.

We use **Feature-First (Vertical Slicing) with Layer-First Compilation (`asmref`)**.

- **Master Assemblies:** Stored in `Assets/_Game/Scripts/Assemblies/` (one `asmdef` per folder):
    - `App.Core`
    - `App.View`
    - `App.Flow`
    - `App.Infra`
    - `App.Boot`
- **Feature Folders:** `Assets/_Game/Scripts/Features/[FeatureName]/`
    - `/Core`: Domain logic (Uses `.asmref` pointing to `App.Core`).
    - `/View`: MonoBehaviours (Uses `.asmref` pointing to `App.View`).
    - `/Flow`: Handlers/Event Routing (Uses `.asmref` pointing to `App.Flow`).

- **Boot Folders:** `Assets/_Game/Scripts/Boot/` (DI composition, LifetimeScopes, startup wiring).
- **Infra Folders:** `Assets/_Game/Scripts/Infra/` (engine/SDK/data adapters).
- **Systems Folders:** `Assets/_Game/Scripts/Systems/` (cross-feature presentation modules like Audio/VFX/Notifications).

- **UI Assets:** Stored in `Assets/_Game/UI/[FeatureName]/` (UXML, USS).

Always use standard block-scoped namespaces matching the directory structure (e.g. `namespace App.Economy.Core { }`). Never include the company name.

Core architectural layer definitions:

- **`Boot`:** Scene bootstrapping, VContainer scopes, handling game launch.
- **`Core`:** Pure C# domain logic (POCOs) and Models. Zero Unity dependencies (no MonoBehaviours, no UnityEngine UI).
- **`Infra`:** I/O wrapper systems (Saving, Asset Loading, raw Unity Input reading).
- **`View`:** MonoBehaviours, UI Toolkit, Audio, VFX.
- **`Flow`:** The Orchestration layer linking inputs, executing core logic, and handling domain events.

### Concise Rules of Thumb

1. Put gameplay rules/state in `Features/*/Core|Flow|View`.
2. Put DI wiring and LifetimeScopes in `Boot` (never in feature slices).
3. Put external SDKs, save/load, telemetry, platform services in `Infra`.
4. Put reusable cross-feature visuals (audio/vfx/popups) in `Systems`.
5. Use folder-aligned namespaces (`App.<Area>.<Layer>`), block-scoped only.
6. Keep one `asmdef` per folder and one directional dependency graph.
7. `App.Boot` is composition root only; no other assembly references it.
8. If a class references Flow types, it must compile in `App.Flow` or `App.Boot`, not `App.View`.

### Assembly Dependency Rules (Practical)

- `App.Core` -> no project-layer dependencies.
- `App.Infra` -> may depend on `App.Core`.
- `App.View` -> may depend on `App.Core`.
- `App.Flow` -> may depend on `App.Core`, `App.View`, `App.Infra`.
- `App.Boot` -> may depend on `App.Core`, `App.View`, `App.Flow`, `App.Infra`.

## 3. Structural Components, Roles & Definitions

### Banned Terms (Do not use these in architecture)

- 🚫 **`Controller`**: Too vague. Use specific Route Handlers.
- 🚫 **`Presenter`**: Implies tight coupling to a View. We use View -> Handler.
- 🚫 **`Orchestrator` / `Coordinator`**: Use specific Handlers.
- 🚫 **`Translator`**: Boilerplate. Views should directly publish VitalRouter intent commands.
- 🚫 **`Manager`**: Vague God Object smell. Use action-based names for Facades (e.g., `AudioPlayer`, `VfxSpawner`, `SceneLoader`).
- 🚫 **`System`**: Do not use for presentation logic. Only acceptable for low-level I/O infra (e.g., `SaveSystem`).

### A. The Core Domain (Pure C# - The "Brain")

- **`Model`**: A pure data container holding reactive state (`R3.ReactiveProperty`). It has NO game logic.
- **`Service`**: Pure C# business rules. Takes raw data, does math or validation, and strictly updates the Model's Rx State directly.
- **`DTO`** (Data Transfer Object): A dumb data structure used only for saving to disk or loading JSON configs. No observables.

### B. The Presentation Layer (Unity MonoBehaviours - The "Body")

- **`View`**: A strict visual component. It takes data and shows it, or detects a physical user interaction and directly publishes a global `VitalRouter.ICommand` (an "Intent"). It has ZERO core game logic. Views are registered _into_ VContainer via `builder.RegisterComponent(viewBase)`.

### C. The Flow Layer (Event Handling - The "Nerves")

The "glue" that connects isolated systems together using VitalRouter.

- **`Handler`**: Replaces the "Controller". A Handler uses source-generator declarative routing (partial class with `[Routes]`). It listens to commands from Views or Services and decides what to do: call a Service to do logic, play a sound, etc. (e.g., `SceneFlowHandler` handles `PlayIntentCommand`). Handlers are grouped by **Technical System** or **Feature** (e.g., `AudioFlowHandler`, `VfxFlowHandler`, `EconomyFlowHandler`) to keep dependencies strictly isolated. Do not name handlers after a specific event (e.g., `CoinEarnedEventHandler` 🚫).

#### 1. Input-to-Logic Flow (Command Sourcing)

- UI `View` detects interaction and directly publishes a VitalRouter `ICommand` struct representing user _intent_ (e.g. `PlayIntentCommand`).
- A `Handler` catches the intent command, calling domain `Service` logic.
- Upon success, the `Handler` or `Service` fires a new `ICommand` domain event (e.g., `CoinEarnedCommand`).

#### 2. VitalRouter Infrastructure

- **`Router`**: The global message bus.
- **`Route`**: The destination method marked with `[Route]` inside a Handler.
- **`Interceptor`**: Middleware filters (e.g., `CommandLoggingInterceptor`) to catch commands globally and log them for End-to-End Test Replays.

## 4. Booting and Scene Flow Structure (Additive Loading)

- **Boot:** VContainer root scope initiates singletons.
- **Loading:** Addressables load data, `Infra.SaveService` asynchronously reads `Newtonsoft.Json`. VContainer Game Scope is built and Models get populated.
- **Gameplay Start:** Game scene loads additively, Views `Awake()` and bind to Models.

## Code Standards

- Use standard block-scoped namespaces since file-scoped namespaces are not supported.
- Use pattern matching, async/await where possible.
- Avoid `Debug.Log` loops, rely on proper logging or breakpoints.
- Always use `UniTask` instead of Coroutines.
- Models and logic must be unit-testable outside of Unity.

## AI Tools & Skills

- Feature Slice Creation: Use the provided `FeatureSliceGenerator.cs` (via Unity Editor Menu) or ask the agent to "Generate a Feature Slice for [FeatureName]" utilizing instructions mapped in workspace settings.
