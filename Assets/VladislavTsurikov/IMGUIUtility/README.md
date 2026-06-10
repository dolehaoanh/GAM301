# IMGUIUtility

Package ID: `com.vladislavtsurikov.imguiutility`

## Why it exists
IMGUIUtility exists to collect reusable IMGUI helpers for editor tooling that still uses immediate mode UI.

## Technical overview
The package provides drawing helpers, layout helpers, and small abstractions that reduce duplication in custom IMGUI-based editor windows and inspectors.

This package currently contains: Runtime, Editor.

## How to use
Reference it from editor assemblies and call the utility methods when building IMGUI inspectors, windows, or overlays.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
