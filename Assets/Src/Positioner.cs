﻿using UnityEngine;

namespace FUI {
#nullable enable
    public delegate void Positioner(UnityEngine.RectTransform rectTransform, Borders borders, System.Func<UnityEngine.Vector2>? sizeGetter);

    public static class P {
        public static Positioner Left(float? size = null, float fraction = 0) {
            return (rectTransform, borders, sizeGetter) => {
                var offset = size ?? sizeGetter?.Invoke().x ?? 0;

                rectTransform.anchorMin = new Vector2(borders.Left.Fraction, borders.Bottom.Fraction);
                rectTransform.anchorMax = new Vector2(borders.Left.Fraction + fraction, 1 - borders.Top.Fraction);

                rectTransform.offsetMin = new Vector2(borders.Left.Pixels, borders.Bottom.Pixels);
                rectTransform.offsetMax = new Vector2(borders.Left.Pixels + offset, -borders.Top.Pixels);

                borders.Left.Increment(offset, fraction);
            };
        }


        public static Positioner Right(float? size = null, float fraction = 0) {
            return (rectTransform, borders, sizeGetter) => {
                var offset = size ?? sizeGetter?.Invoke().x ?? 0;

                rectTransform.anchorMin = new Vector2(1 - borders.Right.Fraction - fraction, borders.Bottom.Fraction);
                rectTransform.anchorMax = new Vector2(1 - borders.Right.Fraction, 1 - borders.Top.Fraction);

                rectTransform.offsetMin = new Vector2(-borders.Right.Pixels - offset, borders.Bottom.Pixels);
                rectTransform.offsetMax = new Vector2(-borders.Right.Pixels, -borders.Top.Pixels);

                borders.Right.Increment(offset, fraction);
            };
        }

        public static Positioner Down(float? size = null, float fraction = 0) {
            return (rectTransform, borders, sizeGetter) => {
                var offset = size ?? sizeGetter?.Invoke().y ?? 0;

                rectTransform.anchorMin = new Vector2(borders.Left.Fraction, borders.Bottom.Fraction);
                rectTransform.anchorMax = new Vector2(1 - borders.Right.Fraction, borders.Bottom.Fraction + fraction);

                rectTransform.offsetMin = new Vector2(borders.Left.Pixels, borders.Bottom.Pixels);
                rectTransform.offsetMax = new Vector2(-borders.Right.Pixels, borders.Bottom.Pixels + offset);

                borders.Bottom.Increment(offset, fraction);
            };
        }


        public static Positioner Up(float? size = null, float fraction = 0) {
            return (rectTransform, borders, sizeGetter) => {
                var offset = size ?? sizeGetter?.Invoke().y ?? 0;

                rectTransform.anchorMin = new Vector2(borders.Left.Fraction, 1 - borders.Top.Fraction - fraction);
                rectTransform.anchorMax = new Vector2(1 - borders.Right.Fraction, 1 - borders.Top.Fraction);

                rectTransform.offsetMin = new Vector2(borders.Left.Pixels, -borders.Top.Pixels - offset);
                rectTransform.offsetMax = new Vector2(-borders.Right.Pixels, -borders.Top.Pixels);

                borders.Top.Increment(offset, fraction);
            };
        }

        public static Positioner Fill => (rectTransform, borders, sizeGetter) => {

            rectTransform.anchorMin = new Vector2(borders.Left.Fraction, borders.Bottom.Fraction);
            rectTransform.anchorMax = new Vector2(1 - borders.Right.Fraction, 1 - borders.Top.Fraction);

            rectTransform.offsetMin = new Vector2(borders.Left.Pixels, borders.Bottom.Pixels);
            rectTransform.offsetMax = new Vector2(-borders.Right.Pixels, -borders.Top.Pixels);
        };

        public static Positioner RigidFill => (rectTransform, borders, sizeGetter) => {
            Fill(rectTransform, borders, null);
            if (sizeGetter != null) {
                var size = sizeGetter();
                borders.Left.Increment(size.x, 0);
                borders.Top.Increment(size.y, 0);
            }
        };



        public static Positioner Gravity(Vector2 normalizedPosition, float? width = null, float? height = null) {
            return (rectTransform, borders, sizeGetter) => {
                rectTransform.anchorMin = normalizedPosition;
                rectTransform.anchorMax = normalizedPosition;
                var size = rectTransform.sizeDelta;

                if (width.HasValue)
                    size.x = width.Value;
                if (height.HasValue)
                    size.y = height.Value;
                rectTransform.sizeDelta = size;

                rectTransform.pivot = normalizedPosition;

            };
        }
    }
}