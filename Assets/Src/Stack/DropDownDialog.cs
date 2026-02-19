using UnityEngine;
using FUI.Gears;
using static FUI.Basic;
using static FUI.Shortcuts;
using TMPro;
using System;
using FUI.Modifiers;
using System.Collections.Generic;

#nullable enable
namespace FUI {
    public class MenuDialog : EnumDialog {
        public float Width = 200;

        /*public static void Open<T>(GameObject parentControlTransform, T value, Action<T> returnAction, float width, string[]? options = null) {
            var form = EnumDialog.Open<T, MenuDialog>(parentControlTransform, value, returnAction, options);
            form.Width = width;
        }*/


        public void Configure<T>(GameObject parentControlTransform, T value, Action<T> returnAction, float width, string[]? options = null) {
            Configure(parentControlTransform, value, returnAction, options);
            Width = width;
        }


        protected override void UpdatePosition(RectTransform window) {
            if (_parentControlTransform == null) return;
            PositionWindowAround(window, _parentControlTransform, Width);
        }
    }

    public class DropDownDialog : EnumDialog {
        /*public static void Open<T>(GameObject parentControlTransform, T value, Action<T> returnAction, string[]? options = null) {
            Open<T, DropDownDialog>(parentControlTransform, value, returnAction, options);
        }*/

        protected override void UpdatePosition(RectTransform window) {
            if (_parentControlTransform == null) return;
            PositionWindowUnder(window, _parentControlTransform);
        }
    }

    public abstract class EnumDialog : AbstractValueDialogDynamicReturn<int> {

        protected RectTransform _parentControlTransform = null!;
        private string[] _options = null!;
        private Dictionary<int, int>? _indexToEnumValue;

        public virtual void Configure<T>(GameObject parentControl, T value, Action<T> returnAction, string[]? options = null) {
            int index;
            if (value is Enum) {
                index = EnumHelper.ValueToIndex(value);
                _indexToEnumValue = EnumHelper.IndexToValue(value.GetType());
                if (options == null) {
                    options = EnumHelper.NamesTrimmed(value.GetType());
                }
            }
            else {
                index = Convert.ToInt32(value);
                if (options == null) {
                    throw new Exception("Options must be provided for non-enum types");
                }
            }

            base.Configure(index, returnAction);
            _options = options;
            _parentControlTransform = parentControl.GetComponent<RectTransform>();
        }

        protected override void Populate() {

            for (int i = 0; i < _options.Length; i++) {
                int index = i;
                var selected = i == Value;
                var first = i == 0;
                var last = i == _options.Length - 1;
                var backgroundColor = selected ? Theme.SelectedButtonColor : Theme.PopupBackgroundColor;
                var backgroundColorHovered = selected ? Theme.SelectedButtonHoveredColor : Theme.ButtonHoveredColor;
                var backgroundColorPressed = selected ? Theme.SelectedButtonPressedColor : Theme.ButtonPressedColor;
                var textColor = selected ? Theme.SelectedButtonTextColor : Theme.ButtonTextColor;
                var text = _options[i];
                var radius = Theme.Radius;
                var buttonHorizontalPadding = Theme.ButtonHorizontalPadding;
                Action action = selected
                ? () => { Close(); }
                : () => {
                    var returnValue = index;
                    if (_indexToEnumValue != null) {
                        returnValue = _indexToEnumValue[index];
                    }
                    Close();
                    Return(returnValue);
                };

                using (Group(DefaultControlPositioner
                    , new AddComponent<RoundedRectangle>()
                    , new SetRectangleCorners(
                        first ? radius : 0,
                        first ? radius : 0,
                        last ? radius : 0,
                        last ? radius : 0
                        )
                    , new AddPressedHoveredHighlighter(
                        backgroundColor,
                        backgroundColorHovered,
                        backgroundColorPressed
                        )
                    , new AddClickHandler(action)
                    )
                    ) {
                    Padding(buttonHorizontalPadding, buttonHorizontalPadding, 0, 0);
                    Label(text, P.RigidFill
                        , new SetColor(textColor)
                        , new SetTextAlignment(HorizontalAlignmentOptions.Left, VerticalAlignmentOptions.Middle)
                        );
                }
            }
            ShrinkContainer(false, true);
        }
    }
}
