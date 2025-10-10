using UnityEngine;
using UnityEngine.UI;

namespace FUI{
    [RequireComponent(typeof(CanvasRenderer))]
    public class RoundedRectangleWithOutline : RoundedRectangle {
        [SerializeField]
        private float _outlineWidth;
        public virtual float OutlineWidth { get { return _outlineWidth; } set { if (SetPropertyUtility.SetStruct(ref _outlineWidth, value)) SetVerticesDirty(); } }
        
        [SerializeField]
        private Color _outlineColor = Color.black;
        public virtual Color OutlineColor { get { return _outlineColor; } set { if (SetPropertyUtility.SetColor(ref _outlineColor, value)) SetVerticesDirty(); } }

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
            var rect = rectTransform.rect;
            //apply padding
            rect = ApplyPadding(rect, PaddingLeft, PaddingRight, PaddingTop, PaddingBottom);
            if (_outlineWidth == 0) {
                AddRoundedRectangle(rect, vh, Segments, TopRightRadius, TopLeftRadius, BottomLeftRadius, BottomRightRadius, color);
            }
            else {
                if (_outlineWidth > 0) {
                    var outlineRect = ApplyPadding(rect, -_outlineWidth, -_outlineWidth, -_outlineWidth, -_outlineWidth);
                    AddRoundedRectangle(outlineRect, vh, Segments, TopRightRadius + _outlineWidth, TopLeftRadius + _outlineWidth, BottomLeftRadius + _outlineWidth, BottomRightRadius + _outlineWidth, OutlineColor);
                    AddRoundedRectangle(rect, vh, Segments, TopRightRadius, TopLeftRadius, BottomLeftRadius, BottomRightRadius, color);
                }
                else {
                    AddRoundedRectangle(rect, vh, Segments, TopRightRadius, TopLeftRadius, BottomLeftRadius, BottomRightRadius, OutlineColor);
                    var innerRect = ApplyPadding(rect,  -_outlineWidth, -_outlineWidth, -_outlineWidth, -_outlineWidth);
                    AddRoundedRectangle(innerRect, vh, Segments, TopRightRadius + _outlineWidth, TopLeftRadius + _outlineWidth, BottomLeftRadius + _outlineWidth, BottomRightRadius + _outlineWidth, color);
                }
            }

            /*if (OutlineWidth > 0) {
                var outlineRect = ApplyPadding(rect, -OutlineWidth, -OutlineWidth, -OutlineWidth, -OutlineWidth);

                AddRoundedRectangle(rect, vh, Segments, TopRightRadius, TopLeftRadius, BottomLeftRadius, BottomRightRadius, OutlineColor);
                AddRoundedRectangle(outlineRect, vh, Segments, Mathf.Max(0, TopRightRadius - OutlineWidth), Mathf.Max(0, TopLeftRadius - OutlineWidth), Mathf.Max(0, BottomLeftRadius - OutlineWidth), Mathf.Max(0, BottomRightRadius - OutlineWidth), color);
            }
            else {
                AddRoundedRectangle(rect, vh, Segments, TopRightRadius, TopLeftRadius, BottomLeftRadius, BottomRightRadius, color);
            }*/
        }

    }
}
