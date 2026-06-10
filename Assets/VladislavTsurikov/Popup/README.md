# Popup

`Popup` is a small Unity Editor package for building compact popup tools on top of UI Toolkit.

It provides:

- a reusable `Popup` window shell
- popup positioning relative to an activator rect
- optional header, dragging, and resize behavior
- dynamic popup sizing through virtual methods
- selector-based popups such as `AddPopup`
- specialized popups such as `LayerMaskPopup`

## Main Classes

### `Popup`

Base `EditorWindow` for popup tools.

Override these members to control behavior:

- `ShowHeader`
- `CanDragWindow`
- `CanResizeWindow`
- `GetPreferredWindowSize()`
- `PopulateContent(VisualElement host)`

Useful helpers:

- `RefreshWindowSize()`
- `SetWindowSize(Vector2 desiredSize)`
- `ComputeSearchListHeight(int visibleRowCount)`

### `PopupUtility`

Utility for opening popup windows from IMGUI code:

```csharp
PopupUtility.OpenPopup<MyPopup>(
    activatorRect,
    "My Popup",
    null,
    popup =>
    {
        // initialize popup state here
    });
```

### `AddPopup`

Selector popup for choosing one item from a categorized list.

It supports:

- category hierarchy
- flat search results
- disabled items
- dynamic sizing based on visible rows

### `LayerMaskPopup`

Popup for editing Unity layer masks with:

- search
- compact list rows
- checkmark-based selection
- dynamic sizing

## Basic Usage

1. Create a class that inherits from `Popup`.
2. Override `PopulateContent(VisualElement host)` and build the UI there.
3. Override window behavior if needed.
4. Open the popup through `PopupUtility.OpenPopup<TPopup>(...)`.

Example:

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Popup.Editor;

public sealed class ExamplePopup : Popup
{
    protected override bool ShowHeader => true;

    protected override Vector2 GetPreferredWindowSize()
    {
        return new Vector2(240f, 160f);
    }

    protected override void PopulateContent(VisualElement host)
    {
        host.Add(new Label("Hello from Popup"));
    }
}
```

Open it:

```csharp
PopupUtility.OpenPopup<ExamplePopup>(
    activatorRect,
    "Example",
    null,
    _ => { });
```

## Styling

The popup shell uses:

- `Resources/Popup/Popup.uxml`
- `Resources/Popup/PopupStyle.uss`

Selector-specific styles live in:

- `Resources/Selector/SelectorPopup.uss`

If you create a custom popup, you can either:

- reuse the shell styles as-is
- add your own styles inside the popup content

## Existing Examples

Current examples in this package:

- `Editor/AddPopup.cs`
- `Editor/LayerMaskPopup.cs`

These are good references for:

- creating popup content
- working with dynamic sizing
- integrating popup tools into IMGUI-based inspectors
