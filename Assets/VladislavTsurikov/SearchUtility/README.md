# SearchUtility

Package ID: `com.vladislavtsurikov.searchutility`

## Why it exists
SearchUtility exists to provide a reusable search UI building block for editor tools.

## Technical overview
The package contains shared UI Toolkit search elements and supporting code so multiple tools can expose consistent searchable lists.

This package currently contains: Editor.

## How to use
Reference it from editor packages, create a search view, bind your item source, and react to filtering or selection changes in your tool UI.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
