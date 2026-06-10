# AttributeUtility

Package ID: `com.vladislavtsurikov.attributeutility`

## Why it exists
AttributeUtility exists to keep custom attributes and attribute-related helpers in one place.

## Technical overview
The package provides reusable attribute definitions and support code that other systems can consume to drive editor behavior, validation, or metadata.

This package currently contains: Runtime.

## How to use
Reference the package from modules that declare or inspect custom attributes and use the provided attributes in your runtime or editor code.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
