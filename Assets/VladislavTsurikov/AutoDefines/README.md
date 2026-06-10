# AutoDefines

Package ID: `com.vladislavtsurikov.autodefines`

## Why it exists
AutoDefines exists to manage scripting define symbols automatically instead of maintaining them by hand.

## Technical overview
The package contains editor logic that evaluates project state and updates define symbols so optional integrations can enable themselves when requirements are present.

This package currently contains: Editor.

## How to use
Install the package in the editor, configure the define rules if needed, and let the tool update scripting defines as project dependencies change.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
