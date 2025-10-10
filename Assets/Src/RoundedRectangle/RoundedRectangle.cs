using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace FUI{
    [RequireComponent(typeof(CanvasRenderer))]
    public class RoundedRectangle : MaskableGraphic {

        public int Segments = 4;
        public float TopLeftRadius;
        public float TopRightRadius;
        public float BottomLeftRadius;
        public float BottomRightRadius;

        public float PaddingTop;
        public float PaddingBottom;
        public float PaddingLeft;
        public float PaddingRight;

        protected static Rect ApplyPadding(Rect rect, float paddingLeft, float paddingRight, float paddingTop, float paddingBottom){
            rect.xMin += paddingLeft;
            rect.xMax -= paddingRight;
            rect.yMin += paddingBottom;
            rect.yMax -= paddingTop;
            return rect;
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
            var rect = rectTransform.rect;
            //apply padding
            rect = ApplyPadding(rect, PaddingLeft, PaddingRight, PaddingTop, PaddingBottom);
            AddRoundedRectangle(rect, vh, Segments, TopRightRadius, TopLeftRadius, BottomLeftRadius, BottomRightRadius, color);
        }

        public void SetAllCorners(float radius) {
            SetCorners(radius, radius, radius, radius);
        }

        public void SetCorners(float topLeft, float topRight, float bottomLeft, float bottomRight) {
            var changed = TopLeftRadius != topLeft || TopRightRadius != topRight || BottomLeftRadius != bottomLeft || BottomRightRadius != bottomRight;
            if (!changed) return;
            TopLeftRadius = topLeft;
            TopRightRadius = topRight;
            BottomLeftRadius = bottomLeft;
            BottomRightRadius = bottomRight;
            SetVerticesDirty();
        }        


        public void SetPadding(float top, float bottom, float left, float right) {
            var changed = PaddingTop != top || PaddingBottom != bottom || PaddingLeft != left || PaddingRight != right;
            if (!changed) return;
            PaddingTop = top;
            PaddingBottom = bottom;
            PaddingLeft = left;
            PaddingRight = right;
            SetVerticesDirty();
        }
        
        
        protected static void AddRoundedRectangle(Rect rect, VertexHelper vh, int segments, float topRightRadius, float topLeftRadius, float bottomLeftRadius, float bottomRightRadius, Color color) {

            var startingIndex = vh.currentVertCount;
            var positions = GenerateRoundedRectanglePoints(rect, segments, topRightRadius, topLeftRadius, bottomLeftRadius, bottomRightRadius);

            foreach (var p in positions) {
                var uv = (p - rect.position) / rect.size;

                vh.AddVert(new UIVertex() {
                    position = p,
                    color = color,
                    uv0 = uv,
                });
            }

            for (int i = 0; i < positions.Length - 2; i++) {
                vh.AddTriangle(startingIndex, startingIndex + i + 2, startingIndex + i + 1);
            }
        }

        static Vector2[] GenerateRoundedRectanglePoints(Rect rect, int segments, params float[] radiusCorners){

            if (segments < 1){
                return new Vector2[]{
                    new(rect.xMax, rect.yMax),
                    new(rect.xMin, rect.yMax),
                    new(rect.xMin, rect.yMin),
                    new(rect.xMax, rect.yMin),
                };
            }

            int totalNumPoints = radiusCorners.Sum(x => x > 0 ? segments + 1 : 1);
            var points = new Vector2[totalNumPoints];

            for (int i = 0; i < radiusCorners.Length; i++){
                if (radiusCorners[i] < 0) radiusCorners[i] = 0;
            }


            Vector2[] corners ={
                new(rect.xMax - radiusCorners[0], rect.yMax - radiusCorners[0]),
                new(rect.xMin + radiusCorners[1], rect.yMax - radiusCorners[1]),
                new(rect.xMin + radiusCorners[2], rect.yMin + radiusCorners[2]),
                new(rect.xMax - radiusCorners[3], rect.yMin + radiusCorners[3]),
            };

            int index = 0;
            for (int i = 0; i < corners.Length; i++){

                Vector2 center = corners[i];
                float startAngle = i * Mathf.PI / 2;
                float endAngle = startAngle + Mathf.PI / 2;
                float radius = radiusCorners[i];
                int numVerticesPerCorner = segments + 1;

                if (radiusCorners[i] <= 0){
                    points[index] = corners[i];
                    index++;

                }
                else{

                    for (int j = 0; j < numVerticesPerCorner; j++){
                        float angle = Mathf.Lerp(startAngle, endAngle, (float)j / (numVerticesPerCorner - 1));
                        Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                        points[index] = point;
                        index++;
                    }
                }
            }

            return points;
        }

    }
}
