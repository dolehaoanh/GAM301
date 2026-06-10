# ScriptableObjectUtility

Package ID: `com.vladislavtsurikov.scriptableobjectutility`

## Why it exists
ScriptableObjectUtility exists to make ScriptableObject workflows easier to reuse across packages.

## Technical overview
The package contains helpers for creating, finding, editing, and maintaining ScriptableObject assets in both editor and runtime-friendly patterns.

This package currently contains: Runtime, Editor.

## How to use
Use the utility API in packages that rely on ScriptableObject assets and integrate the helpers into your asset creation or lookup workflows.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
