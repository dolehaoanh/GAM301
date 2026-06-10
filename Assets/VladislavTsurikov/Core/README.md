# Core

Package ID: `com.vladislavtsurikov.core`

## Why it exists
Core exists to hold the shared foundation used by many other VladislavTsurikov packages.

## Technical overview
The package contains base interfaces, common models, and low-level helpers that other modules build on top of.

This package currently contains: Runtime, Editor.

## How to use
Install Core as an upstream dependency, reference its asmdef in dependent packages, and use its base contracts in shared architecture code.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
