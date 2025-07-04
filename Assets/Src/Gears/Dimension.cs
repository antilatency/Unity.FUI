using UnityEngine;

namespace FUI.Gears
{
    public class Dimension : FormNotifier
    {
        public enum DimensionsMask
        {
            None = 0,
            Width = 1 << 0,
            Height = 1 << 1,
            Both = Width | Height
        }

        float _width = float.NaN;
        float _height = float.NaN;

        public float Width => _width;
        public float Height => _height;

        public DimensionsMask Mask = DimensionsMask.None;

        protected override void OnRectTransformDimensionsChange()
        {
            RectTransform rectTransform = (RectTransform)transform;
            Rect rect = rectTransform.rect;
            var size = rect.size;

            bool notify = false;
            if (Mask == DimensionsMask.None)
                return;

            if (Mask.HasFlag(DimensionsMask.Width))
            {
                if (Mathf.Approximately(_width, size.x))
                    return;
                //Debug.Log($"WidthObserver: Width changed from {_width} to {size.x}");
                _width = rect.width;
                notify = true;
            }
            if (Mask.HasFlag(DimensionsMask.Height))
            {
                if (Mathf.Approximately(_height, size.y))
                    return;
                _height = rect.height;
                notify = true;
            }

            if (notify)
                NotifyForm();
        }
    }
}
