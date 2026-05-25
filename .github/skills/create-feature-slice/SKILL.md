---
name: create-feature-slice
description: 'Use when: The user wants to generate a new Feature Slice (Core, View, Flow) using the Modern Unity Clean Architecture (2026) pattern with .asmref files.'
user-invocable: true
---

# Skill: Create Feature Slice

This skill automates the creation of a standard Vertical Slice in the `Assets/_Game/Features/` directory.

## Prerequisite

Ensure the Master Assemblies exist in `Assets/_Game/Assemblies/`:

- `App.Core.asmdef`
- `App.View.asmdef`
- `App.Flow.asmdef`

## Workflow

1. Identify the product feature name (e.g., `Shop`, `Economy`, `Combat`).
2. Create the directory tree under `Assets/_Game/Features/[FeatureName]/`:
    - `/Core`
    - `/View`
    - `/Flow`
3. Generate the following `.asmref` files to bridge the feature to the global assemblies:
    - `Assets/_Game/Features/[FeatureName]/Core/[FeatureName].Core.asmref`: `{"reference": "App.Core"}`
    - `Assets/_Game/Features/[FeatureName]/View/[FeatureName].View.asmref`: `{"reference": "App.View"}`
    - `Assets/_Game/Features/[FeatureName]/Flow/[FeatureName].Flow.asmref`: `{"reference": "App.Flow"}`
4. Update the user that the structure is ready for code.

## Bundled Assets

- **Editor Script**: `Assets/_Game/Editor/FeatureSliceGenerator.cs` is already available in the project to allow the user to perform this same action via the Unity Editor menu: `Tools > Architecture > Create Feature Slice`.

## Namespace Convention

Always use block-scoped namespaces:

```csharp
namespace App.[FeatureName].[Layer]
{
    // Code here
}
```

Example: `namespace App.Shop.Core { }`
