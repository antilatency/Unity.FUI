using FUI.Gears;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FUI {
#nullable enable

    public struct Modifier {

        public delegate void Procedure(GameObject value);

        public string Id;
        public Procedure? Creator;
        public Procedure? Updater;

        public Modifier(string id, Procedure? creator, Procedure? updater) {
            Id = id;
            Creator = creator;
            Updater = updater;
        }
    }


    public static class M {

        private static Modifier MakeSetter(bool mutable, Modifier.Procedure action, [System.Runtime.CompilerServices.CallerMemberName] string id = "") {
            return new Modifier(
                id,
                mutable ? null : action,
                mutable ? action : null
                );
        }

        public static Modifier SetColor(Color color, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<Graphic>().color = color; });

        public static Modifier SetRectangleCorners(float radius = 4, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<RoundedRectangle>().SetAllCorners(radius); });

        public static Modifier SetText(string text, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().text = text; });

        public static Modifier SetFontSize(float fontSize, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().fontSize = fontSize; });


        public static Modifier SetTextAlignment(HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Left, VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Capline, bool mutable = true)
            => MakeSetter(mutable, x => {
                x.GetComponent<TMP_Text>().horizontalAlignment = horizontalAlignment;
                x.GetComponent<TMP_Text>().verticalAlignment = verticalAlignment;
            });


        public static Modifier AddComponent<T>() where T : Component =>
            new Modifier(
                $"AddComponent<{typeof(T).FullName}>",
                x => x.gameObject.AddComponent<T>(),
                null
                );

        public static Modifier AddClickHandler(Action<GameObject> click) =>
            new Modifier(
                "AddClickHandler",
                x => x.AddComponent<PointerClickHandler>(),
                x => x.GetComponent<PointerClickHandler>().OnClick = click
                );
        
        
        public static Modifier SetCustomShader(string shaderName) =>
            new Modifier(
                "SetCustomShader",
                x => x.AddComponent<CustomShader>(),
                x => x.GetComponent<CustomShader>().ShaderName = shaderName
                );


        public static Modifier AddPressedHoveredHighlighter(Color color, Color hoveredColor, Color pressedColor) =>
            new Modifier(
                "AddPressedHoveredHighlighter",
                x => {
                    var highlighter = x.AddComponent<ConfigurablePressedHoveredHighlighter>();
                    highlighter.initialColor = color;
                    highlighter.pressedColor = pressedColor;
                    highlighter.hoveredColor = hoveredColor;
                },
                null
                );

        public static Modifier SetFormToNotify(int NumExtraIterations) =>
            new Modifier(
                "SetFormToNotify",
                null,
                x => {
                    var notifier = x.GetComponent<AbstractUserInputNotifier>();
                    notifier.SetFormToNotify(Form.Current);
                    notifier.ExtraIterations = NumExtraIterations;
                }
                );

        public static Modifier DisableRaycastTarget() =>
            new Modifier(
                "DisableRaycastTarget",
                x => x.GetComponent<Graphic>().raycastTarget = false,
                null
                );

    }



}