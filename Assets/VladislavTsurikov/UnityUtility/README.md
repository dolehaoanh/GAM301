# UnityUtility

Package ID: `com.vladislavtsurikov.unityutility`

## Why it exists
UnityUtility exists to hold common Unity-specific helper code that does not belong to one feature package.

## Technical overview
The package provides general-purpose Unity helpers used by multiple systems for scene objects, assets, or engine-facing workflows.

This package currently contains: Runtime, Editor.

## How to use
Reference it from other packages and use the shared helpers instead of duplicating common Unity API glue code.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
