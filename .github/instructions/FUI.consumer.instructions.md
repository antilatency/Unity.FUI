---
description: "Use when working in a Unity project that consumes Unity.FUI from GitHub via UPM. Covers package source discovery through FUI.csproj, the rebuild-driven Form/Build model, border-consuming layout, package cache rules, and where major FUI APIs are implemented."
applyTo:
  - "Assets/**/*.cs"
  - "Library/PackageCache/com.antilatency.fui@*/**/*.cs"
---

# Unity.FUI Consumer Instructions

Use this instruction in Unity projects that depend on Unity.FUI through UPM, for example:

```json
"com.antilatency.fui": "https://github.com/antilatency/Unity.FUI.git?path=Assets/Src"
```

Because the package is referenced with `?path=Assets/Src`, the package root in the consumer project corresponds to the repository's `Assets/Src` folder, not to the repository root.

## Resolve package sources through FUI.csproj first

- Unity generates `FUI.csproj` in the consumer project.
- Open `FUI.csproj` before inspecting FUI internals.
- Do not hardcode the package cache hash in paths such as `Library/PackageCache/com.antilatency.fui@2029774fcc54/...`. That hash changes.
- Use the `<Compile Include="...">` entries in `FUI.csproj` to discover the actual physical location of package sources in the current project.
- When you explain source locations, translate them back to paths relative to the original `Assets/Src` root.

Examples:

- `Library\PackageCache\com.antilatency.fui@<hash>\Basic.cs` -> `Basic.cs`
- `Library\PackageCache\com.antilatency.fui@<hash>\Shortcuts.cs` -> `Shortcuts.cs`
- `Library\PackageCache\com.antilatency.fui@<hash>\Stack\FormStack.cs` -> `Stack/FormStack.cs`
- `Library\PackageCache\com.antilatency.fui@<hash>\Theme\Theme.cs` -> `Theme/Theme.cs`

When the user asks where some FUI type or helper is implemented, answer with the path relative to `Assets/Src`, not with the volatile `Library/PackageCache/...@<hash>/...` path.

## What FUI is

- FUI is a runtime UI framework for Unity with an IMGUI-like mental model.
- Instead of hand-authoring a deep prefab hierarchy, UI is described from state in `Build()`.
- FUI rebuilds the form when it is dirty, reuses matching elements from the previous frame, and performs layout through explicit positioners and border consumption.
- The core entry points are `Form.cs`, `Basic.cs`, and `Shortcuts.cs`.

## Default workflow

- Store UI state as fields on the form.
- Derive from `Form` in `Form.cs`.
- Override `Build()` and declare the entire interface from state.
- Call `AssignAndMakeDirty()` or `MakeDirty()` when state changes.
- Use `Basic` and `Shortcuts` as the default composition layer.
- Add project-specific helpers in consumer code instead of patching package internals whenever possible.

Prefer this import pattern:

```csharp
using FUI;
using static FUI.Basic;
using static FUI.Shortcuts;
```

Minimal form pattern:

```csharp
public class HelloWorldForm : Form {
  public string Name = "World";

  protected override void Build() {
    using (WindowBackground()) {
      Padding(8);
      Label("Hello, FUI");
      GapTop();
      InputField(Name, value => AssignAndMakeDirty(ref Name, value));
      GapTop();
      Button("Log", () => Debug.Log($"Hello, {Name}!"));
    }
  }
}
```

## Scene setup expectations

- A working runtime scene needs a `Canvas`, an `EventSystem`, and a `FormStack`.
- The built-in asset creation menu items described in the README are implemented in `EditorTools/MenuCommands.cs`.
- `Create -> FUI -> Scene` and `Create -> FUI -> Form` come from `EditorTools/MenuCommands.cs`.
- Theme assets are defined by `Theme/Theme.cs`.

## State and rebuilding

`Form` in `Form.cs` is the rebuild-driven root of the system.

Internal behavior:

- `Form.Update()` calls `RebuildIfNeeded()`.
- `Lazy == true` means the form rebuilds only when dirty.
- `MakeDirty()` marks the form for rebuild.
- `AssignAndMakeDirty(ref field, value)` assigns and marks dirty only when the value changed.
- `Rebuild()` manages the element tree, reuses compatible `GameObject`s, calls `Build()`, and cleans up unmatched children.

Working rules:

- Keep source-of-truth state on the form instance.
- Do not cache references to transient child objects from a previous rebuild unless the API is specifically designed for that.
- If UI depends on a field, changing that field should normally go through `AssignAndMakeDirty()`.

For nested value editors, use `ValueForm<T>` from `ValueForm.cs`.

- `ValueForm<T>` is the standard pattern for editing nested values.
- `ValueFormExtensions.Execute(...)` wires the current value and return callback.
- Inside `ValueForm<T>`, use `AssignAndReturn(...)` to update the value and propagate changes.

## Layout principles

FUI layout is border-consuming rather than margin-driven.

- Positioners live in `Positioner.cs` as the `P` helper class.
- Typical positioners are `P.Up()`, `P.Down()`, `P.Left()`, `P.Right()`, `P.Fill`, `P.RigidFill`, `P.Center()`, and `P.Absolute()`.
- Side positioners consume space from the corresponding side of the current container.
- `P.Fill` uses the remaining area without consuming it.
- `P.RigidFill` uses the remaining area and also shifts borders based on child size.
- `P.RowElement()` and `P.ColumnElement()` derive element size from item count.

The two parameters used by side positioners mean:

- first parameter: absolute pixels
- second parameter: relative fraction of remaining space

Examples:

- `P.Up(24)` means consume 24 pixels from the top.
- `P.Up(0, 0.5f)` means consume half of the remaining height from the top.
- `P.Up(24, 0.5f)` means consume half of the remaining height plus 24 pixels.

Internally, `Borders` in `Form.cs` tracks the remaining area through `Top`, `Bottom`, `Left`, and `Right` borders.

## Spacing rules

- Use `Padding(...)` from `Basic.cs` for container insets.
- Use `GapTop()`, `GapBottom()`, `GapLeft()`, and `GapRight()` from `Basic.cs` for spacing between siblings.
- Do not rely on margins baked into controls.
- If spacing is wrong, inspect `Padding`, `Gap*`, then the relevant `P.*` positioner.

## Groups and containers

- Containers are created through the disposable group pattern in `Basic.cs`.
- `Group(...)` starts a new control scope.
- Elements created inside the scope become children of that group.
- On dispose, FUI closes the scope and places subsequent elements as siblings.

Example mental model:

```csharp
using (Group(P.Left(0, 0.5f))) {
  Padding(4);
  Label("Left column");
}
Label("Remaining area");
```

Built-in container-style helpers in the current package snapshot:

- `Group(...)` in `Basic.cs`
- `WindowBackground(...)` in `Shortcuts.cs`
- `PopupBackground(...)` in `Shortcuts.cs`
- `ScrollRectVertical(...)` in `Basic.cs`
- `SubForm<T>()` in `Shortcuts.cs`
- `ZoomPanViewport(...)` and `FitInside(...)` in `Shortcuts.cs`

Important version note:

- The README may mention `Panel`, but the current package snapshot does not expose a public `Panel(...)` helper in runtime sources.
- If the user asks for `Panel`, treat that as documentation drift and use `Group(...)` with background modifiers, `PopupBackground(...)`, or another current helper instead.

## Tree merge and element reuse

During `Rebuild()`, FUI tries to reuse existing `GameObject`s from the previous frame.

- Low-level element creation and matching logic lives in `Basic.cs`.
- Matching is based on the generated element identifier.
- The identifier includes container marker information, original prefab information, and modifier identities.
- Unmatched old children are destroyed after rebuild.

Critical rule:

- Do not modify the element tree directly inside `Build()`.
- Do not set visual state directly on reused objects inside `Build()` if that state should participate in merge compatibility.
- Express intent through modifiers instead.

Good example:

- use `SetColor(...)` from `Modifier.cs`

Bad example:

- directly mutating `GetComponent<Graphic>().color` inside `Build()`

## Modifiers

Modifiers are reusable behavior blocks defined in `Modifier.cs`.

They can:

- add components
- set properties
- affect the element identifier and therefore reuse behavior

Common modifier families:

- visual modifiers such as `SetColor`, `SetRectangleCorners`, `SetText`, `SetFontSize`
- text helpers such as `SetTextAlignmentCenterMiddle`, `SetTextAlignmentLeftMiddle`, `SetTextOverflowEllipsis`, `SetRichTextEnabled`, `SetWordWrapping`
- behavior modifiers such as click handlers and hover or pressed highlighters
- rendering helpers such as custom shader setup

When a task requires new reusable behavior, prefer creating a custom modifier in consumer code instead of hardcoding ad-hoc state changes into `Build()`.

## Components and shortcut APIs

In FUI, a component is usually a function that composes lower-level primitives.

The main public component library is `Shortcuts.cs`.

Important helpers in `Shortcuts.cs`:

- text: `Text`, `Label`
- buttons: `Button`, `DisabledButton`, `ColorButton`, `ContentButton`
- toggles: `Checkbox`, `ToggleButton`, `ToggleGroupButtons`
- input: `LabeledInputField`, `LabeledInputFieldSpinbox`, `Spinbox`, `Dropdown`, `LabeledDropdown`, `Slider`
- root and composition: `WindowBackground`, `PopupBackground`, `SubForm<T>`, `Row`
- 3D and viewport helpers: `Viewport3D`, `ZoomPanViewport`, `FitInside`

Important helpers in `Basic.cs`:

- `Element(...)`
- `Group(...)`
- `Padding(...)`
- `Gap*`
- `InputField(...)`
- `ScrollRectVertical(...)`
- `Dialog<T>()`

Recommended consumer-side pattern:

- keep `Basic` and `Shortcuts` as the foundation
- create local wrappers such as `LocalUIShortcuts`
- extract repeated UI fragments into named helper functions

## Theme

`Theme` in `Theme/Theme.cs` is a `ScriptableObject` containing visual constants used by controls.

Important fields include:

- `WindowBackgroundColor`, `PopupBackgroundColor`
- `PrimaryColor`
- label colors and hovered or pressed variants
- input colors, outline color, selection color
- button colors and selected button colors
- `Radius`, `LineHeight`, `DefaultGap`, `OutlineThickness`, `ButtonHorizontalPadding`

Usage rules:

- forms default to `Theme.Default`
- a form can override `Theme`
- dialogs inherit the top form theme unless another theme is passed explicitly
- `Theme.OnValidate()` broadcasts runtime updates in play mode

If a task is about restyling rather than behavior, start with `Theme/Theme.cs` and the form's `Theme` assignment before changing component logic.

## Dialogs and FormStack

Runtime stack behavior is centered around `Stack/FormStack.cs` and `Stack/Dialog.cs`.

`FormStack` behavior:

- `FormStack.Instance.Push<T>()` creates and shows a new form
- the previous top form is disabled and made non-interactive
- `Pop()` removes the current top form and restores the previous one

`Dialog` behavior:

- extends `Form`
- adds a transparent glass layer
- optionally closes on Escape
- optionally closes on click outside
- provides centered, under-control, and around-control positioning helpers

Built-in dialogs:

- `Stack/MessageDialogYesNo.cs`
- `Stack/DropDownDialog.cs`
- `Stack/StringEditDialog.cs`

If a control opens a popup or modal interaction, inspect `Stack/Dialog.cs` and `Stack/FormStack.cs` together.

## Markdown, hyperlinks, images, and 3D content

- markdown conversion entry point: `MarkdownConverter.cs`
- markdown parser internals: `CommonMark/*`
- hyperlink formatting and click handling: `Hyperlink.cs`
- image helpers: `Images/Images.cs`
- 3D viewport helper: `Viewport3D(...)` in `Shortcuts.cs`
- 3D camera state types: `Viewport3D/CameraState.cs`, `Viewport3D/OrbitCameraState.cs`
- 3D interaction helpers: `Viewport3D/CameraStateController.cs`, `Viewport3D/RenderTextureResizer.cs`

Treat `CommonMark/*` and `StandaloneFileBrowser/*` as vendor-like internals unless the task explicitly targets them.

## How to inspect a feature

When the user asks how a feature works, inspect it in this order:

1. Find the public helper in `Shortcuts.cs` or `Basic.cs`.
2. Check which modifiers it applies in `Modifier.cs`.
3. Check any behavior component in `Gears/*`.
4. Check rendering or shape code such as `RoundedRectangle/*`, `Circle/*`, `Images/Images.cs`, or `Viewport3D/*`.
5. If dialogs are involved, inspect `Stack/Dialog.cs` and the specific dialog file.

Examples:

- `InputField` -> `Basic.cs` -> `FUI_InputField.cs`
- `Slider` -> `Shortcuts.cs` -> `Gears/Slider.cs`
- `Dropdown` -> `Shortcuts.cs` -> `Stack/DropDownDialog.cs`
- `Viewport3D` -> `Shortcuts.cs` -> `Viewport3D/CameraStateController.cs` and `Viewport3D/RenderTextureResizer.cs`
- markdown reader -> `MarkdownConverter.cs` -> `CommonMark/*` -> `Hyperlink.cs` if clicks matter

## Consumer-side coding guidance

- Prefer implementing app-specific UI in the consumer project, not inside `Library/PackageCache`.
- Treat `Library/PackageCache/com.antilatency.fui@<hash>` as dependency code.
- Do not edit package cache files unless the user explicitly wants to fork, vendor, or patch the package source.
- When the user asks to change library behavior, first determine whether the request can be solved by composing current APIs in consumer code.
- Keep UI code declarative.
- Extract small component functions rather than duplicating layout code.
- Use `SubForm<T>()` and `ValueForm<T>` for nested editors instead of flattening everything into one giant form.

## Source map

All paths below are relative to the package root, which is the repository folder `Assets/Src`.

- `Form.cs`: `Form`, `Borders`, dirty handling, rebuild lifecycle, control stack management.
- `ValueForm.cs`: `ValueForm<T>` and `Execute(...)` for nested value editors.
- `Basic.cs`: low-level element creation, merge and reuse, `Element`, `Group`, `Padding`, `Gap*`, `InputField`, `ScrollRectVertical`, and `Dialog<T>()`.
- `Shortcuts.cs`: high-level controls such as `Text`, `Label`, `Button`, `DisabledButton`, `ColorButton`, `ContentButton`, `Checkbox`, `ToggleButton`, `ToggleGroupButtons`, `Dropdown`, `Slider`, `Spinbox`, `WindowBackground`, `PopupBackground`, `SubForm<T>`, `Viewport3D`, and `Row`.
- `Positioner.cs`: the `Positioner` delegate and all `P.*` helpers.
- `Theme/Theme.cs`: `Theme`, `Theme.Default`, colors, sizing constants, and theme change propagation.
- `Stack/FormStack.cs`: runtime push and pop behavior for forms and dialogs.
- `Stack/Dialog.cs`: modal dialog base class, glass layer, close behavior, and positioning helpers.
- `Stack/MessageDialogYesNo.cs`, `Stack/DropDownDialog.cs`, `Stack/StringEditDialog.cs`: built-in dialog implementations.
- `Modifier.cs`: modifier base classes and common modifier implementations.
- `FUI_InputField.cs`: input field internals.
- `Gears/Slider.cs`: slider behavior internals.
- `Gears/*`: hover, pressed, pointer, drag, drop, fit, and related behavior components.
- `Images/Images.cs`: image helpers.
- `Hyperlink.cs`: TextMeshPro hyperlink helpers.
- `MarkdownConverter.cs` and `CommonMark/*`: markdown conversion and parsing.
- `RoundedRectangle/*`: rounded rectangle graphics and outline variants.
- `Circle/*`: circle graphics.
- `Viewport3D/*`: camera state, interaction, and render texture resize helpers.
- `Prefabs/Library/PrefabLibrary.cs`: prefab lookup helpers.
- `EditorTools/MenuCommands.cs`: editor-only asset and scene creation helpers.
- `StandaloneFileBrowser/*`: standalone file browser integration.

## Caution areas

- `EditorTools/MenuCommands.cs` is editor-only. Do not treat it as runtime behavior.
- `CommonMark/*` and `StandaloneFileBrowser/*` are effectively vendor-like internals.
- The README can drift from the current package snapshot. If documentation and source disagree, trust the current source files discovered through `FUI.csproj`.