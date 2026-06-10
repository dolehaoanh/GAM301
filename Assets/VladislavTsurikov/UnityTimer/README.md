# Timer

Package ID: `com.vladislavtsurikov.timer`

## Why it exists
UnityTimer exists to provide lightweight timer helpers for delayed and time-based workflows.

## Technical overview
The package contains timer abstractions and helper logic for scheduling work or tracking elapsed time without rewriting timer code per system.

This package currently contains: Runtime.

## How to use
Create timers in your runtime systems, update them as needed, and react when the scheduled delay or interval completes.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
