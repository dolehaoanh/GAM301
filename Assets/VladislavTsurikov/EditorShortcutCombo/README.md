# ShortcutCombo

Package ID: `com.vladislavtsurikov.shortcutcombo`

## Why it exists
EditorShortcutCombo exists to simplify editor shortcut combinations that would otherwise be reimplemented in multiple tools.

## Technical overview
The package contains editor-only input helpers for detecting and managing shortcut combinations in custom Unity tooling.

This package currently contains: Editor.

## How to use
Reference it from editor tools that need shortcut handling and wire the helper API into your window or panel event flow.

Typical setup steps:
1. Add the package to the project as a local folder or a UPM Git dependency.
2. Reference its asmdef from the modules that need this functionality.
3. Use the runtime API, editor UI, or helper utilities that the package exposes.
