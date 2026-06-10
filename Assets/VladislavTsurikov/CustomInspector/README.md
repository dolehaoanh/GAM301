# CustomInspector

Package ID: `com.vladislavtsurikov.custominspector`

## Why it exists
CustomInspector exists to build richer Unity inspector experiences with reusable infrastructure.

## Technical overview
The package provides editor-side inspector building blocks, drawing utilities, and patterns for composing custom inspector workflows.

This package currently contains: Runtime, Editor.

## How to use
Reference the package in editor assemblies and use its inspector APIs when creating custom editors, drawers, and tooling panels.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
