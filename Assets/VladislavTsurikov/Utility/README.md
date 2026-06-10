# Utility

Package ID: `com.vladislavtsurikov.utility`

## Why it exists
Utility exists to keep broadly useful helper code in a single shared package.

## Technical overview
The package contains low-level reusable utility methods and small support types that are useful across many unrelated modules.

This package currently contains: Runtime, Editor.

## How to use
Use it as a foundational dependency when a package needs common helper functionality that is not specific to Unity subsystems.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
