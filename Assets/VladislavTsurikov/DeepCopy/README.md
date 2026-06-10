# DeepCopy

Package ID: `com.vladislavtsurikov.deepcopy`

## Why it exists
DeepCopy exists to duplicate object graphs without rewriting copy code for every model type.

## Technical overview
The package provides helpers for deep-copying data so systems can work on isolated copies instead of mutating shared state accidentally.

This package currently contains: Runtime.

## How to use
Pass supported objects through the deep copy API whenever you need a separate editable copy of a data structure.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
