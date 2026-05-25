---
name: create-view-scripts
description: 'Use when: The user wants to create or scaffold general View scripts for a feature, independent of a specific presentation framework.'
user-invocable: true
---

# Skill: Create View Scripts

This skill scaffolds general View-layer scripts for a feature under Vertical Slicing.

## Target Paths

- C# View scripts: `Assets/_Game/Scripts/Features/[FeatureName]/View/`

## Guardrails (Hard Rules)

1. View scripts are presentation-only:
    - Allowed: Build layout, bind read-only model state, publish intent commands.
    - Forbidden: Domain/business logic, save/load, scene loading decisions.
2. Views publish `ICommand` intents; they never call domain services directly.
3. Keep View implementation framework-focused and move all business decisions to Flow/Core layers.
4. Keep bindings and subscriptions deterministic and disposable.
5. View lifecycle must cleanly unsubscribe/dispose bindings.

## Standard View Script Structure

Use the following method order for consistency:

1. `InitializeRoot()`
2. `AttachStyles()`
3. `BuildLayout()`
4. `CacheElements()`
5. `BindState()`
6. `WireEvents()`
7. `DisposeBindings()`

## Outputs

When requested, generate:

1. One or more View scripts under `Assets/_Game/Scripts/Features/[FeatureName]/View/`.
2. Optional constants for element/class names to avoid magic strings.
3. Brief user summary confirming created files and wiring points for commands/state.
4. If a framework-specific View is requested (e.g., UI Toolkit), delegate to the matching specialized skill.
