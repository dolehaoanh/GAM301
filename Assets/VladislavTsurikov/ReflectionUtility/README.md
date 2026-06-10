# ReflectionUtility

Package ID: `com.vladislavtsurikov.reflectionutility`

## Why it exists
ReflectionUtility exists to centralize reflection helpers that many editor and runtime systems need.

## Technical overview
The package provides utility methods for type inspection, member lookup, and reflective access so higher-level packages can stay cleaner.

This package currently contains: Runtime.

## How to use
Reference it from packages that need dynamic type discovery or reflective access and call the helper API instead of reimplementing reflection code.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
