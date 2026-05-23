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

## 2. Project Structure & Namespaces (`Layer.Feature`)

Always use file-scoped namespaces following the `Layer.Feature` convention. Never include the company or project name unless specifically requested.

- **`Boot`:** Scene bootstrapping, VContainer scopes, handling game launch.
- **`Core`:** Pure C# domain logic (POCOs) and Models. Zero Unity dependencies (no MonoBehaviours, no UnityEngine UI).
- **`Infra`:** I/O wrapper systems (Saving, Asset Loading, raw Unity Input reading).
- **`View`:** MonoBehaviours, UI Toolkit, Audio, VFX.
- **`Flow`:** The Orchestration layer linking inputs, executing core logic, and handling domain events.

## 3. Structural Components, Roles & Definitions

### Banned Terms (Do not use these in architecture)

- 🚫 **`Controller`**: Too vague. Specify if it is a Translator or a Handler.
- 🚫 **`Presenter`**: Implies tight coupling to a View. We split this into View -> Translator -> Handler.
- 🚫 **`Orchestrator` / `Coordinator`**: Use specific Handlers.

### A. The Core Domain (Pure C# - The "Brain")

- **`Model`**: A pure data container holding reactive state (`R3.ReactiveProperty`). It has NO game logic.
- **`Service`**: Pure C# business rules. Takes raw data, does math or validation, and strictly updates the Model's Rx State directly.
- **`DTO`** (Data Transfer Object): A dumb data structure used only for saving to disk or loading JSON configs. No observables.

### B. The Presentation Layer (Unity MonoBehaviours - The "Body")

- **`View`**: A strict, passive visual component. It takes data and shows it, or takes a physical user click and emits an `Observable<Vector2>`. It has ZERO game logic. Always extracts a pure C# Interface (e.g., `IMainMenuView`) to allow headless unit testing of the Flow layer. Views are registered _into_ VContainer via `builder.RegisterComponent(viewBase).AsImplementedInterfaces()`.

### C. The Flow Layer (Event Handling - The "Nerves")

The "glue" that connects isolated systems together using VitalRouter.

- **`Translator`**: A pure C# bridge class. Its ONLY job is to subscribe to a `View`'s generic UI signal (like `R3.Observable`) and translate it into a globally meaningful `VitalRouter.ICommand` (an "Intent"). (e.g., `MenuInputTranslator`).
- **`Handler`**: Replaces the "Controller". A Handler uses source-generator declarative routing (partial class with `[Routes]`). It listens to commands and decides what to do: call a Service to do logic, play a sound, etc. (e.g., `SceneFlowHandler` handles `PlayIntentCommand`).

#### 1. Input-to-Logic Flow (Command Sourcing)

- UI `View` emits generic R3 event (`Observable<Unit>`).
- `Translator` maps this to a VitalRouter `ICommand` struct representing user _intent_.
- A `Handler` catches the command, calling domain `Service` logic.
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
