---
name: create-uitoolkit-view-scripts
description: 'Use when: The user wants to create or scaffold UI Toolkit View scripts for a feature using code-first layout and optional USS styling.'
user-invocable: true
---

# Skill: Create UI Toolkit View Scripts

This skill scaffolds UI Toolkit-specific View scripts for a feature under Vertical Slicing. It applies the guardrails from `create-view-scripts` and adds UI Toolkit implementation rules.

## Target Paths

- C# View scripts: `Assets/_Game/Scripts/Features/[FeatureName]/View/`
- Optional feature styles: `Assets/_Game/UI/[FeatureName]/`

## UI Toolkit Rules

1. Build layout in code-first style using `VisualElement` tree creation.
2. Use a single optional stylesheet reference field (`StyleSheet _styleSheet`).
3. UI must render correctly without stylesheet assigned.
4. If a stylesheet is assigned, attach it once in `AttachStyles()`.
5. Prefer class-based styling (`AddToClassList`) over inline style values.
6. Publish user intents as `ICommand`; do not invoke domain services from the View.

## Standard View Script Structure

1. `InitializeRoot()`
2. `AttachStyles()`
3. `BuildLayout()`
4. `CacheElements()`
5. `BindState()`
6. `WireEvents()`
7. `DisposeBindings()`

## Style Attachment Pattern

```csharp
private void AttachStyles(VisualElement root)
{
    if (_styleSheet != null) root.styleSheets.Add(_styleSheet);
}
```

## Outputs

When requested, generate:

1. One or more UI Toolkit View scripts under `Assets/_Game/Scripts/Features/[FeatureName]/View/`.
2. Optional USS file under `Assets/_Game/UI/[FeatureName]/`.
3. Optional constants for element/class names to avoid magic strings.
4. Brief user summary confirming created files and optional stylesheet hookup.
