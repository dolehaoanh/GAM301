# CPUNoise

Package ID: `com.vladislavtsurikov.cpunoise`

## Why it exists
CPUNoise exists to generate noise values on the CPU for tools and runtime systems that need procedural data.

## Technical overview
The package contains CPU-side noise generation utilities that can be used for terrain, masks, randomization, or procedural simulation inputs.

This package currently contains: Runtime.

## How to use
Call the noise API from your generation code and feed the resulting values into gameplay, rendering, or editor tooling workflows.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
