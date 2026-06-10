# Nody

Package ID: `com.vladislavtsurikov.nody`

## Why it exists
Nody exists to support node-based workflows in Unity projects.

## Technical overview
The package contains graph and node abstractions that can be used to build visual scripting-like tools, editors, or structured processing graphs.

This package currently contains: Runtime, Editor.

## How to use
Reference the package from graph-based tools, create your node models, and build runtime or editor workflows on top of the graph API.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
