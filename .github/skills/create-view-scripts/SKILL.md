---
name: create-view-scripts
description: 'Use when: The user wants to create or scaffold UI Toolkit View scripts for a feature using code-first layout and USS-based styling in the Modern Unity Clean Architecture.'
user-invocable: true
---

# Skill: Create View Scripts

This skill scaffolds View-layer scripts for a feature under Vertical Slicing, with optional USS styling.

## Target Paths

- C# View scripts: `Assets/_Game/Scripts/Features/[FeatureName]/View/`
- Feature styles: `Assets/_Game/UI/[FeatureName]/`

## Guardrails (Hard Rules)

1. View scripts are presentation-only:
    - Allowed: Build layout, bind read-only model state, publish intent commands.
    - Forbidden: Domain/business logic, save/load, scene loading decisions.
2. Views publish `ICommand` intents; they never call domain services directly.
3. Use class-based styling via USS (`AddToClassList`) rather than inline styles whenever possible.
4. Stylesheet reference is optional; layout must render correctly even when no stylesheet is assigned.
5. If assigned, attach a single scene-referenced stylesheet.
6. View lifecycle must cleanly unsubscribe/dispose bindings.

## Standard View Script Structure

Use the following method order for consistency:

1. `InitializeRoot()`
2. `AttachStyles()`
3. `BuildLayout()`
4. `CacheElements()`
5. `BindState()`
6. `WireEvents()`
7. `DisposeBindings()`

## Style Attachment Pattern (Code-First)

```csharp
private void AttachStyles(VisualElement root)
{
   if (_styleSheet != null) root.styleSheets.Add(_styleSheet);
}
```

## Outputs

When requested, generate:

1. One or more View scripts under `Assets/_Game/Scripts/Features/[FeatureName]/View/`.
2. An optional feature USS under `Assets/_Game/UI/[FeatureName]/`.
3. Optional constants for element/class names to avoid magic strings.
4. Brief user summary confirming created files and optional stylesheet hookup.
