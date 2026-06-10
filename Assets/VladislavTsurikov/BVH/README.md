# BVH

Package ID: `com.vladislavtsurikov.bvh`

## Why it exists
BVH exists to provide fast spatial partitioning for systems that need efficient lookups in 3D space.

## Technical overview
The package implements bounding volume hierarchy data structures and traversal helpers for broad-phase queries and other spatial search tasks.

This package currently contains: Runtime.

## How to use
Build or update a BVH from your spatial data, then query it when you need fast overlap, nearest, or region-based lookups.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
