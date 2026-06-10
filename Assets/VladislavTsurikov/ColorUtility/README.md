# Color

Package ID: `com.vladislavtsurikov.color`

## Why it exists
ColorUtility exists to keep common color operations in one lightweight package.

## Technical overview
The package provides reusable helpers for color conversion, manipulation, and other small operations that are useful across tools and runtime code.

This package currently contains: Runtime.

## How to use
Reference the package and call the helper methods wherever you need repeatable color math or color formatting.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
