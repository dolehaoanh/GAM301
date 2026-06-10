# SceneDataSystem

Package ID: `com.vladislavtsurikov.scenedatasystem`

## Why it exists
SceneDataSystem exists to structure scene-specific data in a reusable, package-friendly way.

## Technical overview
The package contains models and helpers for storing, resolving, or synchronizing data that belongs to a scene or scene-scoped workflow.

This package currently contains: Runtime, Editor.

## How to use
Use the package types in systems that attach data to scenes, then query or update that data during scene load and runtime execution.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
