# Undo

Package ID: `com.vladislavtsurikov.undo`

## Why it exists
Undo exists to wrap common Unity undo workflows in a reusable editor utility package.

## Technical overview
The package contains editor-side helpers for recording changes, integrating with the Unity undo stack, and keeping custom tools undo-friendly.

This package currently contains: Editor.

## How to use
Call the utility methods from editor scripts before mutating serialized data or objects that should participate in Unity undo and redo.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
