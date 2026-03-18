using System;
using TMPro;
using UnityEngine;
using FUI;
using static FUI.Shortcuts;
using static FUI.Basic;
using FUI.Modifiers;
#nullable enable

namespace FUI {


    public class SetInputFieldContentType : SetterModifier<FUI_InputField> {
        public FUI_InputField.ContentType ContentType;
        public SetInputFieldContentType(FUI_InputField.ContentType contentType) {
            ContentType = contentType;
        }

        public override void Set(FUI_InputField component) {
            component.contentType = ContentType;
        }
    }

    public class SetInputFieldTextColor : SetterModifier<FUI_InputField> {
        public Color TextColor;
        public SetInputFieldTextColor(Color textColor) {
            TextColor = textColor;
        }

        public override void Set(FUI_InputField component) {
            component.textComponent.color = TextColor;
        }
    }


    public class NewForm : Form {
        public enum AuthMode {
            SignUp,
            LogIn,
        }

        private static readonly Color32 PageBackgroundColor = new(0xEE, 0xF1, 0xF7, 0xFF);
        private static readonly Color32 CardBackgroundColor = new(0xFF, 0xFF, 0xFF, 0xFF);
        private static readonly Color32 HeaderBackgroundColor = new(0x4C, 0x2D, 0x99, 0xFF);
        private static readonly Color32 PrimaryButtonColor = new(0x6B, 0x79, 0xF1, 0xFF);
        private static readonly Color32 PrimaryButtonHoverColor = new(0x7B, 0x87, 0xF4, 0xFF);
        private static readonly Color32 PrimaryButtonPressedColor = new(0x56, 0x64, 0xDF, 0xFF);
        private static readonly Color32 AccentButtonColor = new(0xFF, 0xA2, 0x36, 0xFF);
        private static readonly Color32 AccentButtonHoverColor = new(0xFF, 0xB3, 0x58, 0xFF);
        private static readonly Color32 AccentButtonPressedColor = new(0xEE, 0x90, 0x20, 0xFF);
        private static readonly Color32 BodyTextColor = new(0x1F, 0x18, 0x34, 0xFF);
        private static readonly Color32 MutedTextColor = new(0x74, 0x6E, 0x84, 0xFF);
        private static readonly Color32 SoftFieldColor = new(0xF1, 0xF5, 0xFF, 0xFF);
        private static readonly Color32 SoftFieldHoverColor = new(0xE8, 0xEE, 0xFF, 0xFF);
        private static readonly Color32 OutlineColor = new(0x17, 0x14, 0x24, 0xFF);
        private static readonly Color32 SwitchTrackColor = new(0x1A, 0x1A, 0x20, 0xFF);

        public string UserName = "+00 000 000 00 00";
        public string Password = "12345678";
        public bool AcceptTerms;
        public AuthMode Mode = AuthMode.LogIn;

        private Theme? _customTheme;

        private void Awake() {
            EnsureTheme();
        }

        private void OnEnable() {
            EnsureTheme();
        }

        private void OnDestroy() {
            if (_customTheme != null) {
                Destroy(_customTheme);
            }
        }

        private void EnsureTheme() {
            if (_customTheme == null) {
                _customTheme = ScriptableObject.Instantiate(Theme.Default);
                _customTheme.hideFlags = HideFlags.HideAndDontSave;
                _customTheme.WindowBackgroundColor = PageBackgroundColor;
                _customTheme.PopupBackgroundColor = CardBackgroundColor;
                _customTheme.PrimaryColor = PrimaryButtonColor;
                _customTheme.LabelColor = BodyTextColor;
                _customTheme.LabelColorHovered = BodyTextColor;
                _customTheme.LabelColorPressed = BodyTextColor;
                _customTheme.LabelColorDisabled = MutedTextColor;
                _customTheme.InputColor = SoftFieldColor;
                _customTheme.InputHoveredColor = SoftFieldHoverColor;
                _customTheme.InputOutlineColor = PrimaryButtonColor;
                _customTheme.InputSelectionColor = new Color32(0x6B, 0x79, 0xF1, 0x5A);
                _customTheme.ButtonColor = PrimaryButtonColor;
                _customTheme.ButtonHoveredColor = PrimaryButtonHoverColor;
                _customTheme.ButtonPressedColor = PrimaryButtonPressedColor;
                _customTheme.ButtonTextColor = Color.white;
                _customTheme.SelectedButtonColor = PrimaryButtonColor;
                _customTheme.SelectedButtonHoveredColor = PrimaryButtonHoverColor;
                _customTheme.SelectedButtonPressedColor = PrimaryButtonPressedColor;
                _customTheme.SelectedButtonTextColor = Color.white;
                _customTheme.Radius = 10f;
                _customTheme.LineHeight = 38f;
                _customTheme.DefaultGap = 10f;
                _customTheme.OutlineThickness = 1f;
                _customTheme.ButtonHorizontalPadding = 16f;
            }

            if (Theme != _customTheme) {
                Theme = _customTheme;
            }
        }

        protected override void Build() {
            EnsureTheme();

            var actionText = Mode == AuthMode.LogIn ? "LOG IN" : "SIGN UP";

            using (WindowBackground()) {
                using (Group(
                    P.Center(272, 514),
                    new AddComponent<RoundedRectangle>(),
                    new SetRectangleCorners(24),
                    new SetColor(CardBackgroundColor)
                )) {
                    DrawHeader();

                    GapTop(24);
                    Padding(24, 24, 0, 24);

                    Row(2, (index, positioner) => {
                        if (index == 0) {
                            DrawModeButton("Sign up", Mode == AuthMode.SignUp, () => AssignAndMakeDirty(ref Mode, AuthMode.SignUp), positioner);
                        }
                        else {
                            DrawModeButton("Log in", Mode == AuthMode.LogIn, () => AssignAndMakeDirty(ref Mode, AuthMode.LogIn), positioner);
                        }
                    }, P.Up(36), 12);

                    GapTop(28);
                    Label("Phone number", P.Up(16), new SetFontSize(12), new SetFontStyle(FontStyles.Bold), new SetColor(BodyTextColor));
                    GapTop(8);
                    InputField(UserName, value => AssignAndMakeDirty(ref UserName, value), P.Up(38),
                        additionalModifiers: new ModifiersList(
                            new SetInputFieldTextColor(BodyTextColor)
                        ));

                    GapTop(16);
                    Label("Password", P.Up(16), new SetFontSize(12), new SetFontStyle(FontStyles.Bold), new SetColor(BodyTextColor));
                    GapTop(8);
                    InputField(Password, value => AssignAndMakeDirty(ref Password, value), P.Up(38),
                        additionalModifiers: new ModifiersList(
                            new SetInputFieldTextColor(BodyTextColor),
                            new SetInputFieldContentType(FUI_InputField.ContentType.Password)
                        ));

                    GapTop(18);
                    using (Group(P.Up(22))) {
                        DrawToggle(AcceptTerms, value => AssignAndMakeDirty(ref AcceptTerms, value), P.Left(38));
                        GapLeft(10);
                        Text("Lorem ipsum dolor amet sed", P.Fill,
                            new SetColor(BodyTextColor),
                            new SetFontSize(11),
                            new SetTextAlignmentLeftMiddle());
                    }

                    GapTop(28);
                    DrawFilledButton(actionText, AccentButtonColor, AccentButtonHoverColor, AccentButtonPressedColor, BodyTextColor, P.Up(46), 8f, 17f, () => {
                        Debug.Log($"{actionText}: {UserName}");
                    });

                    GapTop(14);
                    Text(
                        "Lorem ipsum dolor sit amet,\nconsectetur adipiscing elit, sed\ndo eiusmod tempor",
                        P.Up(40),
                        new SetTextAlignmentCenterMiddle(),
                        new SetColor(MutedTextColor),
                        new SetFontSize(10),
                        new SetTextOverflowOverflow());
                }
            }
        }

        private void DrawHeader() {
            using (Group(
                P.Up(118),
                new AddComponent<RoundedRectangle>(),
                new SetRectangleCorners(24, 24, 18, 18),
                new SetColor(HeaderBackgroundColor)
            )) {
                Padding(18, 18, 12, 18);
                Text("10:30 AM", P.Up(14),
                    new SetTextAlignmentCenterMiddle(),
                    new SetColor(new Color(1f, 1f, 1f, 0.85f)),
                    new SetFontSize(10),
                    new SetFontStyle(FontStyles.Bold));

                GapTop(18);
                Text("LOREM IPSUM!", P.Fill,
                    new SetTextAlignmentCenterMiddle(),
                    new SetColor(Color.white),
                    new SetFontSize(20),
                    new SetFontStyle(FontStyles.Bold));
            }
        }

        private void DrawModeButton(string text, bool selected, Action action, Positioner positioner) {
            if (selected) {
                DrawFilledButton(text, PrimaryButtonColor, PrimaryButtonHoverColor, PrimaryButtonPressedColor, Color.white, positioner, 8f, 14f, action);
                return;
            }

            using (Group(
                positioner,
                new AddComponent<RoundedRectangle>(),
                new SetRectangleCorners(8),
                new SetColor(OutlineColor),
                new AddPressedHoveredHighlighter(OutlineColor, OutlineColor, OutlineColor),
                new AddClickHandlerEx((go, e) => action())
            )) {
                Padding(1);
                using (Group(
                    P.Fill,
                    new AddComponent<RoundedRectangle>(),
                    new SetRectangleCorners(7),
                    new SetColor(CardBackgroundColor)
                )) {
                    Text(text, P.Fill,
                        new SetTextAlignmentCenterMiddle(),
                        new SetColor(OutlineColor),
                        new SetFontSize(14),
                        new SetFontStyle(FontStyles.Bold));
                }
            }
        }

        private void DrawFilledButton(
            string text,
            Color backgroundColor,
            Color hoveredColor,
            Color pressedColor,
            Color textColor,
            Positioner positioner,
            float radius,
            float fontSize,
            Action action) {
            using (Group(
                positioner,
                new AddComponent<RoundedRectangle>(),
                new SetRectangleCorners(radius),
                new SetColor(backgroundColor),
                new AddRectMask(),
                new AddPressedHoveredHighlighter(backgroundColor, hoveredColor, pressedColor),
                new AddClickHandlerEx((go, e) => action())
            )) {
                Text(text, P.Fill,
                    new SetTextAlignmentCenterMiddle(),
                    new SetColor(textColor),
                    new SetFontSize(fontSize),
                    new SetFontStyle(FontStyles.Bold));
            }
        }

        private void DrawToggle(bool value, Action<bool> returnAction, Positioner positioner) {
            var initialColor = value ? PrimaryButtonColor : SwitchTrackColor;
            var hoveredColor = value ? PrimaryButtonHoverColor : new Color32(0x26, 0x26, 0x2E, 0xFF);
            var pressedColor = value ? PrimaryButtonPressedColor : new Color32(0x11, 0x11, 0x16, 0xFF);

            using (Group(
                positioner,
                new AddComponent<RoundedRectangle>(),
                new SetRectangleCorners(11),
                new SetColor(initialColor),
                new AddPressedHoveredHighlighter(initialColor, hoveredColor, pressedColor),
                new AddClickHandlerEx((go, e) => returnAction(!value))
            )) {
                Padding(3);
                Circle(value ? P.Right(16) : P.Left(16), Color.white, numSegments: 32);
            }
        }
    }
}

/*
Inline Suggestions:
Usings: using FUI; using static FUI.Shortcuts; using static FUI.Basic; using FUI.Gears; using FUI.Modifiers;
Form: class X : Form { fields... protected override void Build() { using (WindowBackground()) { Padding(Theme.DefaultGap); Text("Title"); GapTop(); ... } } }
State: AssignAndMakeDirty(ref field, value); MakeDirty();
Dialogs: create dialogs with Dialog.Create<MyDialog>(theme?).Configure(dialogSpecificParameters); configure immediately after creation and pass state/callbacks through Configure(...).
Default stack: controls with optional positioner already use DefaultControlPositioner = P.Up(Form.Current.Theme.LineHeight); in normal vertical forms omit the positioner.
Spacing: controls add no outer margins; use GapTop()/GapBottom()/GapLeft()/GapRight() between siblings, not P.Down(...) as a spacing substitute.
Positioners: use P.Left/Right/Up/Down for explicit edge layout and P.Fill for the remaining rect. Reserve P.Center() for overlays, dialogs, or standalone centered groups, not headings inside a normal stacked form.
Scope: FUI is rebuild-driven and border-consuming. Group/Panel/WindowBackground/ScrollRectVertical create local border scopes. keepBorders=true overlays without consuming.

Prototypes - Positioners:
Positioner P.Left(float? size=null,float fraction=0,bool keepBorders=false); P.Right(float? size=null,float fraction=0,bool keepBorders=false); P.Up(float? size=null,float fraction=0,bool keepBorders=false); P.Down(float? size=null,float fraction=0,bool keepBorders=false); P.Fill; P.RigidFill; P.Center(float? width=null,float? height=null); P.Absolute(Vector2 position,float? width=null,float? height=null,Vector2? anchor=null,Vector2? pivot=null); P.RowElement(int count,float gap=0,float externalPaddingCompensation=0); P.ColumnElement(int count,float gap=0,float externalPaddingCompensation=0);

Prototypes - Containers:
Disposable<RectTransform> Group(Positioner positioner, params Modifier[] modifiers); Disposable<RectTransform> Panel(Positioner positioner,float radius=0); Disposable<RectTransform> WindowBackground(Positioner? positioner=null); Disposable<RectTransform> ScrollRectVertical(Positioner positioner); T ApplyPositioner<T>(this T form,Positioner positioner) where T : Form; T SubForm<T>(bool setParentTheme=true) where T : Form;

Prototypes - Layout:
void Padding(float padding); void Padding(float left,float right,float top,float bottom); void GapTop(float pixels,float fraction=0); void GapBottom(float pixels,float fraction=0); void GapLeft(float pixels,float fraction=0); void GapRight(float pixels,float fraction=0); void ShrinkContainer(bool x,bool y); void Row(int count,Action<int,Positioner> elementBuilder,Positioner? positioner=null,float gap=1,float padding=0); void Row<T>(IList<T> items,Action<int,T,Positioner> elementBuilder,Positioner? positioner=null,float gap=1,float padding=0);

Prototypes - Controls:
RectTransform Text(string text,Positioner? positioner=null,params Modifier[] additionalModifiers); RectTransform Label(string text,Positioner? positioner=null,params Modifier[] additionalModifiers); void Button(string text,Action action,Positioner? positioner=null,bool paddings=true,int? radius=null); RectTransform InputField<T>(T value,Action<T> returnAction,Positioner positioner,string toStringFormat="",Func<string,T>? fromString=null); RectTransform InputField(string value,FUI_InputField.TextDelegate returnAction,Positioner? positioner=null);
RectTransform LabeledInputField<T>(string label,T value,Action<T> returnAction,Positioner? positioner=null,string toStringFormat="0.######",Func<string,T>? fromString=null); RectTransform LabeledInputFieldSpinbox(string label,int value,Action<int> returnAction,int dragStepSize=1,Positioner? positioner=null,string toStringFormat="",Func<string,int>? fromString=null); RectTransform LabeledInputFieldSpinbox(string label,float value,Action<float> returnAction,float dragStepSize=1,Positioner? positioner=null,string toStringFormat="0.######",Func<string,float>? fromString=null); RectTransform LabeledDropdown<T>(string label,T value,Action<T> returnAction,Positioner? positioner=null,string[]? options=null); RectTransform Dropdown<T>(T value,Action<T> returnAction,Positioner? positioner=null,string[]? options=null); RectTransform LabeledCheckbox(string label,bool value,Action<bool> returnAction,Positioner? positioner=null); RectTransform Checkbox(bool value,Action<bool> returnAction,Positioner? positioner=null); RectTransform ToggleButton(bool value,string onText,string offText,Action<bool> returnAction,Positioner? positioner=null); RectTransform Slider(float value,Action<float> returnAction,Positioner? positioner=null,float? handleWidth=null);

*/