using UnityEngine;

namespace FUI {
#nullable enable
    public delegate void Positioner(UnityEngine.RectTransform rectTransform, Borders borders, System.Func<UnityEngine.Vector2> sizeGetter);

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



        public static Positioner Absolute(Vector2 position, float? width = null, float? height = null, Vector2? anchor = null, Vector2? pivot = null) {

            if (pivot == null)
                pivot = anchor;

            return (rectTransform, borders, sizeGetter) => {
                var vectorHalf = new Vector2(0.5f, 0.5f);
                pivot ??= vectorHalf;
                anchor ??= vectorHalf;

                Vector2 size = default;
                if (width == null || height == null) {
                    size = sizeGetter();
                }
                if (width.HasValue)
                    size.x = width.Value;
                if (height.HasValue)
                    size.y = height.Value;

                rectTransform.anchorMin = anchor.Value;
                rectTransform.anchorMax = anchor.Value;
                rectTransform.pivot = pivot.Value;
                rectTransform.sizeDelta = size;
                rectTransform.anchoredPosition = position;
            };
        }

        /*
    void PopulateTabs(Texture[] textures, Action<int> callback, int height = 150, int gap = 4) {
        int index = 0;
        float fraction = 1f / textures.Length;
        float elementGap = gap * (textures.Length + 1) / textures.Length;
        using (Group(P.Up(height))) {
            Padding(gap);
            for (int i = 0; i < textures.Length; i++) {
                //string? title = textures[i];
                var texture = textures[i];
                Image(P.Left(-elementGap, fraction), texture, (g, e) => {

                    if (e.pointerPress == g && e.button == PointerEventData.InputButton.Left) {
                        callback(i);
                        return true;
                    }
                    return false;
                });
                GapLeft(gap);
            }
        }

*/


        public static Positioner RowElement(int count, float gap = 0, float externalPaddingCompensation = 0) {
            return (rectTransform, borders, sizeGetter) => {

                float fraction = 1f / count;
                float gapCompensation = (gap * (count - 1) + externalPaddingCompensation) / count;

                Left(-gapCompensation, fraction)(rectTransform, borders, sizeGetter);
                borders.Left.Increment(gap, 0);
            };
        }
        
        public static Positioner ColumnElement(int count, float gap = 0, float externalPaddingCompensation = 0) {
            return (rectTransform, borders, sizeGetter) => {

                float fraction = 1f / count;
                float gapCompensation = (gap * (count - 1) + externalPaddingCompensation) / count;

                Down(-gapCompensation, fraction)(rectTransform, borders, sizeGetter);
                borders.Bottom.Increment(gap, 0);
            };
        }




    }
}