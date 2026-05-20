# Modern Unity Clean Architecture Guide (2026)

This project follows a strict, highly decoupled Modern Unity Architecture stack. All AI agents and developers must adhere to these rules when generating or modifying code.

## 1. The Core Tech Stack

- **Dependency Injection:** VContainer
- **Asynchronous Flow:** UniTask
- **Reactive State (Rx):** R3 (State management & local bindings)
- **Event Bus:** MessagePipe (Decoupled Domain Event broadcasting)
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

## 3. Structural Components & Flow Execution

### A. The Models (State)

- Pure data containers living in the `Core` layer using R3.
- Store primitive data via `ReactiveProperty<T>` and collections via `ObservableList<T>`.
- Expose to `View` layers securely via `IReadOnlyReactiveProperty<T>`.

### B. Core Services (Logic & Math)

- Pure C# classes taking `Models` via VContainer constructor injection.
- Execute validation and business rules, then strictly update the Model's Rx State directly.
- **Strict Rule:** Core logic does not push events, play sounds, or format UI text. It only modifies state.

### C. Views & UI (Presentation)

- `MonoBehaviours` that inject a Read-Only Model.
- They `.Subscribe()` directly to the model's properties (R3) and update specific visual elements dynamically when the data changes.
- Do not write logic, math, or damage calculations here.

### D. The Flow Layer (Input & Event Handling)

The "glue" that connects isolated systems together.

#### 1. Input-to-Logic Flow

- `Infra.InputReader` reads raw Gamepad/Mouse data.
- A Controller (e.g., `Flow.Combat.PlayerInputHandler`) listens to the infrastructure input.
- It calls domain services (e.g., `DamageService.ApplyDamage()`) directly via Dependency Injection.
- It evaluates the result and publishes a semantic Domain Event via MessagePipe (e.g., `_publisher.Publish(new EnemyKilledEvent())`).

#### 2. The Pragmatic Event Handler Pattern (One Handler Per Event)

Use a single, unified handler per domain event in the `Flow` layer to manage side effects. This prioritizes cohesion and readability.
For example, `EnemyKilledEventHandler` injects `AudioService`, `VfxService`, etc., subscribes to `EnemyKilledEvent`, and sequentially calls `_audio.Play()`, `_vfx.Spawn()`, etc.

## 4. Booting and Scene Flow Structure (Additive Loading)

- **Boot:** VContainer root scope initiates singletons.
- **Loading:** Addressables load data, `Infra.SaveService` asynchronously reads `Newtonsoft.Json`. VContainer Game Scope is built and Models get populated.
- **Gameplay Start:** Game scene loads additively, Views `Awake()` and bind to Models.

## Code Standards

- Use modern C# features (file-scoped namespaces, pattern matching, async/await).
- Avoid `Debug.Log` loops, rely on proper logging or breakpoints.
- Always use `UniTask` instead of Coroutines.
- Models and logic must be unit-testable outside of Unity.
