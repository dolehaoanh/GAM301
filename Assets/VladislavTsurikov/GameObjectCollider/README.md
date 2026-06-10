# GameObjectCollider

Package ID: `com.vladislavtsurikov.gameobjectcollider`

## Why it exists
GameObjectCollider exists to connect collider workflows to standard GameObject-based authoring.

## Technical overview
The package contains editor-side or authoring-side helpers that let collider data integrate with regular Unity scene objects and components.

This package currently contains: Editor.

## How to use
Install it in projects that need GameObject authoring support for collider-related systems and use the provided components or editor tools.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
