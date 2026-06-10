# SceneUtility

Package ID: `com.vladislavtsurikov.sceneutility`

## Why it exists
SceneUtility exists to collect practical scene-management helpers used across tools and gameplay code.

## Technical overview
The package provides shared scene-related utilities for loading, lookup, validation, and other scene workflow tasks.

This package currently contains: Runtime, Editor.

## How to use
Reference the package from scene-heavy systems and call the provided helpers when working with scene paths, scene objects, or scene lifecycle logic.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
