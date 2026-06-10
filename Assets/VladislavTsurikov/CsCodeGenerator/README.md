# CsCodeGenerator

Package ID: `com.vladislavtsurikov.cscodegenerator`

## Why it exists
CsCodeGenerator exists to automate repetitive C# source generation tasks.

## Technical overview
The package contains utilities for composing source files, formatting generated content, and writing predictable code output from editor tooling.

This package currently contains: Runtime.

## How to use
Use the generator API from editor scripts that need to emit C# code and integrate the generated files into your project pipeline.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
