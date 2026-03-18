# Unity.FUI
Unity.FUI is a runtime UI framework for Unity with an IMGUI-like mental model.

Instead of hand-authoring a deep prefab hierarchy, you describe the UI from state in `Build()`. FUI rebuilds the form when it is dirty, reuses matching elements from the previous frame, and performs layout through explicit positioners and border consumption.


## Minimal example
```csharp
using UnityEngine;
using FUI;
using static FUI.Shortcuts;
using static FUI.Basic;

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

This is the default workflow:

- Store UI state as fields on the form.
- Override `Build()` and declare the entire interface from that state.
- Use `AssignAndMakeDirty()` or `MakeDirty()` when state changes. That 

Unity scene requires a specific initial setup, including a `Canvas` , an `EventSystem`, and a `FormStack` in the scene.

To create preconfigured scene use `Create -> FUI -> Scene` in Assets context menu.

To create a new form, use `Create -> FUI -> Form` and then add it to the scene on a `FormStack` child. 


## State and rebuilding
`Form` is the rebuild-driven root of the system.

Internal behavior:
- `Form.Update()` calls `RebuildIfNeeded()`.
- `Lazy == true` means the form rebuilds only when dirty.
- `MakeDirty()` marks the form for rebuild.
- `AssignAndMakeDirty(ref field, value)` is a syntactic sugar that assigns a new value to a field and calls `MakeDirty()` if the value changed.
- `Rebuild()` manages the element tree, reusing existing `GameObject`s when possible. It calls `Build()` method overridden by the user to populate the form.

```csharp
class MyForm : Form {
    public bool Enabled;

    protected override void Build() {
        using (WindowBackground()) {
            Padding(8);
            ToggleButton(Enabled, "Enabled", "Disabled"
                , value => AssignAndMakeDirty(ref Enabled, value)
            );
        }
    }
}
```

In this example, `ToggleButton` appearance depends on the `Enabled` field. It will change its color and text depending on the value. When the user clicks the button, the lambda is called with the new value. Inside the lambda, we call `AssignAndMakeDirty()` to assign the new value to the `Enabled` field and mark the form dirty. On the next frame, `Build()` will be called again, and the `ToggleButton` will be rebuilt with the new value of `Enabled`, reflecting the change in the UI.


For nested value editors, `ValueForm<T>` gives you a focused pattern for editing a value and returning changes through `_returnAction`.


## Layout principles
FUI layout is border-consuming rather than margin-driven.

To declare the behavior of a control, you use positioners from the `P` class such as `P.Up()`, `P.Left()`, or `P.Fill`. If our control uses `P.Up(0, 0.5f)`, it will be placed at the top of the current container and consume half of the vertical space. The remaining space is available for the next control.
The second control, for example, can use `P.Left(100)` , so it will be placed at the left of the remaining lower half of the container, consuming 100 pixels of horizontal space. The third control uses `P.Fill`, so it will take the rest of the space in that area.

As you may notice, there are 2 parameters for side positioners: 
- the first one is absolute pixels,
- the second one is a relative fraction of the remaining space.
So `P.Up(24)` means "consume 24 pixels from the top", while `P.Up(0, 0.5f)` means "consume half of the vertical space from the top". You can also combine them, for example, `P.Up(24, 0.5f)` means "50% + 24 pixels from the top".

This approach is compatible with Unity's anchoring system and allows for window resizing without calling `Build()` again.

Internally, `Borders` class is responsible for tracking the remaining space in the current container. 
`Borders` has 4 `Border` properties: `Top`, `Bottom`, `Left`, and `Right`.


There are positioners that consume space from the sides by modifying the corresponding `Border`:
- `P.Up()` and `P.Down()` consume vertical space from the top and bottom, respectively.
- `P.Left()` and `P.Right()` consume horizontal space from the left and right, respectively.
- `P.RowElement()` and `P.ColumnElement()` will work like `P.Left()` and `P.Up()`, calculating the size based on the number of elements.

`P.Fill` uses the remaining rectangle without consuming it.
`P.RigidFill` is similar but also shifting the borders. This allows to calculate the outer size of the container based on the size of the children.

Also, there are several **absolute** positioners that do not consume space and do not modify borders:
- `P.Center()` is for overlays, dialogs, and standalone centered blocks.
- `P.Absolute()` is for arbitrary absolute positioning.

And the last category of positioning tools are gaps and padding. They change the borders without placing any controls. They are used for **spacing** between siblings and container **insets**:
- `Padding(...)` for container insets.
- `GapTop()`, `GapBottom()`, `GapLeft()`, `GapRight()` for spacing between siblings. Parameters are the same as for side positioners: absolute pixels, relative fraction, or both.

Common rule for the controls: they should not contain any margins. For spacing between controls, use `Gap*`s. For spacing inside the container, use `Padding()`. 

## Groups (Containers)
Containers are inplemented using Disposable pattern. When you create a container, it returns a struct that implements `IDisposable`, and starts a new control scope. Then all elements created inside that scope will be children of the container. When the scope ends, the container is closed and the next elements will be siblings of that container.

```csharp
using (Group(P.Left(0, 0.5f))) { // start a new group accupying the left half of the screen
    Padding(4); // add padding inside the group
    Label("Left column"); // by default P.Up(Theme.LineHeight) is used if no positioner is specified
} // IDisposable.Dispose() is called here
Label("Remaining area"); // this label will be placed at the top of the remaining right half of the screen 
```
There are several built-in group types:
- `Group` is a simple container with no additional behavior.
- `Panel` adds a background and padding.
- `WindowBackground` is a full-screen panel that is used as the root of forms and dialogs.

## Tree merge
During `Rebuild()`, FUI tries to reuse existing `GameObject`s from the previous frame. To determine if an existing object can be reused, FUI generates an string identifier for current element and looks for the existing child with the same identifier. The identifier is not unique per se, but it describes the internal structure of the element. 

The identifier contains 3 parts:
- `#` if the element can have user-defined children
- the source prefab or original object, if there is one,
- list of **Modifiers**

The identifier is stored in the name of the `GameObject`.

FUI components are functions. Each call either creates a new `GameObject` or finds a compatible existing one to reuse.
After each iteration, all unmatched children are destroyed.

### There is one internal rule to keep in mind:
You should not modify the element tree directly inside `Build()`, because such changes are not tracked by the merge algorithm. An object with manual changes may be reused by another element on the next frame. Instead, express the intent through modifiers.

For example, we **should not** manually change the color of an element by calling `GetComponent<Graphic>().color = ...` inside `Build()`.

Instead, we should use the `SetColor(...)` modifier to apply the color. In this case, the Identifier will contain `SetColor`, and the GameObject will be reused only by the element that also changes the color, so there is no risk of a property being changed by another element and left unattended.

## Modifiers

Modifiers are building blocks of components. They are reusable pieces of behavior that can be applied to any element.

They can
- add components
- set properties

Here are some examples of modifiers:

This is a red rounded rect and we can click on it to log a message:
```csharp
Element(
	P.Center(160, 48),
	new AddComponent<RoundedRectangle>(),
	new SetRectangleCorners(12),
	new SetColor(Color.red),
	new AddClickHandler(() => Debug.Log("Clicked the red rect"))
)
```


## Components
In FUI, a component is usually just a function that composes lower-level primitives.

`Shortcuts` is the built-in component library. It demonstrates the intended composition style:

- `Label`, `Text`
- `Button`, `DisabledButton`, `ColorButton`, `ContentButton`
- `InputField`, `LabeledInputField`, `LabeledInputFieldSpinbox`
- `Checkbox`, `ToggleButton`, `ToggleGroupButtons`
- `Dropdown`, `LabeledDropdown`
- `Panel`, `WindowBackground`, `ScrollRectVertical`
- `Row`

The recommended project pattern is to keep `Basic` and `Shortcuts` as the foundation, then add your own `LocalUIShortcuts` or domain-specific helpers in your game code.

For example, a custom component can be a small compositional wrapper:

```csharp
public static RectTransform LabeledString(string label, string value, Action<string> onChanged) {
	using (var group = Labeled(label)) {
		InputField(value, onChanged, P.Fill);
		return group.Value;
	}
}
```

That style keeps UI code declarative and keeps low-level setup out of forms.

In is good practice to create a function for each part of your UI. For example:
```csharp
public static RectTransform StausBar(string status, string softwareVersion) {
    var container = Group(P.Down(24));
    Padding(4);
    Label(status, P.Left());
    Label(softwareVersion, P.Right());
    return container;
}
```

## Theme
`Theme` is a ScriptableObject that contains the visual constants used by controls.

Notable fields include:

- `WindowBackgroundColor`, `PopupBackgroundColor`
- `LabelColor` and related hovered/pressed/disabled variants
- `InputColor`, `InputHoveredColor`, `InputOutlineColor`, `InputSelectionColor`
- `ButtonColor`, hovered/pressed colors, and text colors
- `SelectedButtonColor` variants
- `Radius`, `LineHeight`, `DefaultGap`, `OutlineThickness`, `ButtonHorizontalPadding`

Usage patterns:

- Forms default to `Theme.Default`.
- A form can override `Theme` with another asset or a cloned runtime theme.
- Dialogs inherit the theme of the current top form unless another theme is passed explicitly.
- `Theme.OnValidate()` broadcasts changes to scene listeners in play mode, and forms mark themselves dirty when their active theme changes.

In practice, themes make it easy to keep layout logic stable while restyling controls globally or per-form.


## Dialogs and FormStack
`FormStack` provides runtime stacking for forms and dialogs.

Behavior of the stack:

- `FormStack.Instance.Push<T>()` creates and shows a new form.
- The previous top form is disabled and its canvas group becomes non-interactive.
- `Pop()` removes the current top form and restores the previous one.

`Dialog` builds on top of `Form` and adds modal behavior:

- transparent glass layer,
- optional close on Escape,
- optional close on click outside,
- positioning helpers such as centered, under-control, and around-control windows.

Typical usage:

```csharp
Dialog<MessageDialogYesNo>().Configure(result => {
	Debug.Log($"Dialog result: {result}");
}, "Do you want to proceed?", "Proceed", "Cancel");
```


## Examples
These examples are adapted from the runtime helpers and test scenes in this repository.

### Use nested groups to split the window into sidebar and content areas.
```csharp
protected override void Build() {
	using (WindowBackground()) {
		using (Group(P.Left(220))) {
			Padding(8);
			Label("Sidebar");
			GapTop();
			Button("Refresh", () => { });
		}

		using (Group(P.Fill)) {
			Padding(8);
			Label("Content");
		}
	}
}
```

### Use toggle groups when one field selects exactly one option.
```csharp
public int SelectedIndex;

protected override void Build() {
	using (WindowBackground()) {
		var options = new[] { "A", "B", "C" };
		ToggleGroupButtons(SelectedIndex, options, value => AssignAndMakeDirty(ref SelectedIndex, value));
	}
}
```

### Combine spinboxes, dropdowns, checkboxes, and sliders with the same state-driven pattern.
```csharp
public enum EditMode { Translate, Rotate, Scale }

public float Speed = 1f;
public EditMode Mode = EditMode.Translate;
public bool Enabled = true;

protected override void Build() {
	using (WindowBackground()) {
		Padding(8);
		LabeledInputFieldSpinbox("Speed", Speed, value => AssignAndMakeDirty(ref Speed, value), 0.1f);
		GapTop();
		LabeledDropdown("Mode", Mode, value => AssignAndMakeDirty(ref Mode, value));
		GapTop();
		LabeledCheckbox("Enabled", Enabled, value => AssignAndMakeDirty(ref Enabled, value));
		GapTop();
		Slider(Speed / 10f, value => AssignAndMakeDirty(ref Speed, 10f * value));
	}
}
```

### Extract small UI parts into component functions instead of repeating layout code.
```csharp
public static RectTransform StatusBar(string status, string softwareVersion) {
	using (var group = Group(P.Down(24))) {
		Padding(4);
		Label(status, P.Left());
		Label(softwareVersion, P.Right());
		return group.Value;
	}
}
```

### Create project-specific modifiers when a control needs custom behavior.
```csharp
public class SetInputFieldContentType : SetterModifier<FUI_InputField> {
	public FUI_InputField.ContentType ContentType;

	public SetInputFieldContentType(FUI_InputField.ContentType contentType) {
		ContentType = contentType;
	}

	public override void Set(FUI_InputField component) {
		component.contentType = ContentType;
	}
}

InputField(
	Password,
	value => AssignAndMakeDirty(ref Password, value),
	P.Up(38),
	additionalModifiers: new ModifiersList(
		new SetInputFieldContentType(FUI_InputField.ContentType.Password)
	)
);
```

### Use SubForms to edit complex nested data structures with reusable form classes.
```csharp
class Vector3Form : ValueForm<Vector3> {
	protected override void Build() {
		LabeledInputFieldSpinbox("X", Value.x, v => AssignAndReturn(ref Value.x, v));
		GapTop();
		LabeledInputFieldSpinbox("Y", Value.y, v => AssignAndReturn(ref Value.y, v));
		GapTop();
		LabeledInputFieldSpinbox("Z", Value.z, v => AssignAndReturn(ref Value.z, v));
	}
}

public Vector3 Position = new Vector3(1, 2, 3);

protected override void Build() {
	using (WindowBackground()) {
        Padding(4);
        SubForm<Vector3Form>()
            .Execute(Position, value => AssignAndMakeDirty(ref Position, value))
            .ApplyPositioner(P.Up());		
	}
}
```

### Open dialogs from buttons and handle the result with a callback.
```csharp
Button("Show Yes/No Dialog", (g, e) => {
	Dialog<MessageDialogYesNo>().Configure(result => {
		Debug.Log("Dialog closed with result: " + result);
	}, "Do you want to proceed?", "Proceed", "Cancel");
});
```



### Convert markdown to Unity rich text and show it in a scrollable reader.
```csharp
var unityText = new MarkdownConverter().Convert(markdownText);

using (WindowBackground()) {
	using (ScrollRectVertical(P.Fill)) {
		Padding(4);
		Label(
			unityText,
			P.Up(),
			new SetRichTextEnabled(true),
			new SetWordWrapping(TMPro.TextWrappingModes.Normal),
			new AddClickHandlerEx(Hyperlink.Handle) //Hyperlink handling
		);
	}
}
```

### Use Viewport3D to display a 3D scene in the UI.
```csharp
public Camera? Camera3D;
private RenderTexture? RenderTexture;
public OrbitCameraState CameraState = new OrbitCameraState() {
	Target = Vector3.zero,
	Distance = 5f,
	Yaw = 0.125f,
	Pitch = 0.1f
};

protected override void Update() {
	if (RenderTexture == null) {
		var descriptor = new RenderTextureDescriptor(256, 256) {
			graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB,
			depthBufferBits = 24,
			msaaSamples = 4,
			sRGB = false
		};
		RenderTexture = new RenderTexture(descriptor);
	}

	if (Camera3D != null) {
		Camera3D.targetTexture = RenderTexture;
		CameraState.SetupCamera(Camera3D);
	}

	base.Update();
}

protected override void Build() {
	using (WindowBackground()) {
		Viewport3D(P.Fill, RenderTexture!, CameraState);
	}
}
```

## Summary
Unity.FUI is best understood as a declarative runtime UI layer built around four ideas:

- forms rebuild from state,
- layout consumes borders explicitly,
- modifiers define reusable behavior and drive tree merging,
- components are just composable functions.

If you keep those rules in mind, the code stays compact, reusable, and much easier to reason about than a manually curated runtime hierarchy.
