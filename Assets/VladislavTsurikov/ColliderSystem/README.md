# ColliderSystem

Package ID: `com.vladislavtsurikov.collidersystem`

## Why it exists
ColliderSystem exists to organize collider-related logic as a reusable package instead of scattering it through gameplay code.

## Technical overview
The package contains shared collider abstractions and support code for authoring, querying, or synchronizing collider data in a modular way.

This package currently contains: Runtime, Editor.

## How to use
Reference the package from systems that work with collision data and use its types to build your own runtime or editor-side collider workflows.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
