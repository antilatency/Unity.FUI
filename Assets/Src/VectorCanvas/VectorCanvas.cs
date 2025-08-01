using System.Linq.Expressions;

using UnityEngine;
using UnityEngine.UI;
#nullable enable




namespace FUI {
    
    public static partial class Shortcuts {

        public static void VectorCanvas(VectorCanvas.DrawingDelegate drawingDelegate, Positioner? positioner = null) {
            var form = Form.Current;
            var element = form.Element(null, M.AddVectorCanvas(drawingDelegate));
            (positioner ?? P.Fill)
                (element, form.CurrentBorders, () => new Vector2(100, 100));
        }
    }

    public static partial class M {

        public static Modifier AddVectorCanvas(VectorCanvas.DrawingDelegate drawingDelegate) =>
            new(
                $"AddVectorCanvas",
                x => x.AddComponent<VectorCanvas>(),
                x => {
                    var canvas = x.GetComponent<VectorCanvas>();
                    canvas.OnDrawing = drawingDelegate;
                    canvas.SetVerticesDirty();
                }
            );
    }


    [RequireComponent(typeof(CanvasRenderer))]
    public class VectorCanvas : MaskableGraphic {

        public delegate void DrawingDelegate(VectorCanvasContext context);

        public DrawingDelegate? OnDrawing;

        protected override void OnPopulateMesh(VertexHelper vertexHelper) {
            var context = new VectorCanvasContext(vertexHelper, rectTransform.rect);
            vertexHelper.Clear();
            OnDrawing?.Invoke(context);
        }

    }

    public class VectorCanvasContext {
        VertexHelper _vertexHelper;
        private Rect _rect;

        public Color Color { get; set; } = Color.white;

        public float Thickness { get; set; } = 1f;

        public Vector2 Size { get; private set; }

        public Vector2 Origin = Vector2.zero;

        public Vector2 offset => _rect.position + Origin * _rect.size;

        public VectorCanvasContext(VertexHelper vertexHelper, Rect rect) {
            _vertexHelper = vertexHelper;
            _rect = rect;
            Size = rect.size;
        }

        public void Line(Vector2 start, Vector2 end) {
            start += offset;
            end += offset;
            Vector2 directionNormalized = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-directionNormalized.y, directionNormalized.x) * Thickness * 0.5f;
            var startIndex = _vertexHelper.currentVertCount;
            _vertexHelper.AddVert(start - perpendicular, Color, Vector2.zero);
            _vertexHelper.AddVert(end - perpendicular, Color, Vector2.zero);
            _vertexHelper.AddVert(end + perpendicular, Color, Vector2.zero);
            _vertexHelper.AddVert(start + perpendicular, Color, Vector2.zero);
            _vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            _vertexHelper.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
        }

        /*
                    Vector2 radius = rect.size * 0.5f;

            var segments = Math.Max(NumSegments, 1);
            var positions = new Vector2[segments+2];
            positions[0] = center;

            for (int i = 0; i < segments+1; i++) {
                var ni = (float)i / segments;
                float angle = (StartAngle + ni * Angle) * 2 * Mathf.PI;
                var x = Mathf.Cos(angle);
                var y = Mathf.Sin(angle);
                positions[i+1] = center + new Vector2(x, y) * radius;
            }

            foreach (var p in positions){
                var uv = (p - rect.position) / rect.size;
                vh.AddVert(new UIVertex(){
                    position = p,
                    color = color,
                    uv0 = uv,
                });
            }

            for (int i = 0; i < positions.Length - 2; i++){
                vh.AddTriangle(0, i + 2, i + 1);
            }
        */



        public void Circle(Vector2 center, float radius, int segments = 64, float startAngle = 0, float angle = 1) {
            center += offset;
            var positions = new Vector2[segments + 2];
            positions[0] = Vector2.zero;

            for (int i = 0; i < segments + 1; i++) {
                var ni = (float)i / segments;
                float currentAngle = (startAngle + ni * angle) * 2 * Mathf.PI;
                var x = Mathf.Cos(currentAngle);
                var y = Mathf.Sin(currentAngle);
                positions[i + 1] = new Vector2(x, y);
            }

            var startIndex = _vertexHelper.currentVertCount;
            foreach (var p in positions) {
                //var uv = (p - Rect.position) / Rect.size;
                _vertexHelper.AddVert(new UIVertex() {
                    position = center + p * radius,
                    color = Color,
                    uv0 = 0.5f * p + new Vector2(0.5f, 0.5f)
                });
            }

            for (int i = 0; i < positions.Length - 2; i++) {
                _vertexHelper.AddTriangle(startIndex, startIndex + i + 2, startIndex + i + 1);
            }
        }

        public void Rectangle(Vector2 position, Vector2 size) {
            var rect = new Rect(position, size);
            Rectangle(rect);
        }

        public void Rectangle(Rect rect) {
            rect.position += offset;
            var startIndex = _vertexHelper.currentVertCount;

            _vertexHelper.AddVert(new UIVertex() { position = new Vector2(rect.xMin, rect.yMin), color = Color, uv0 = Vector2.zero });
            _vertexHelper.AddVert(new UIVertex() { position = new Vector2(rect.xMax, rect.yMin), color = Color, uv0 = Vector2.right });
            _vertexHelper.AddVert(new UIVertex() { position = new Vector2(rect.xMax, rect.yMax), color = Color, uv0 = Vector2.one });
            _vertexHelper.AddVert(new UIVertex() { position = new Vector2(rect.xMin, rect.yMax), color = Color, uv0 = Vector2.up });

            _vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            _vertexHelper.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
        }


        /*
        var rect = rectTransform.rect;
            var center = rect.center;
            Vector2 radius = rect.size * 0.5f;

            var segments = Math.Max(NumSegments, 3);
            var positions = new Vector2[2*segments + 2];
            var normals = new Vector2[2*segments + 2];


            for (int i = 0; i < segments + 1; i++) {
                var ni = (float)i / segments;
                float angle = (StartAngle + ni * Angle) * 2 * Mathf.PI;
                var x = Mathf.Cos(angle);
                var y = Mathf.Sin(angle);
                normals[2 * i] = new Vector2(x, y);
                normals[2 * i + 1] = new Vector2(-x, -y);

                positions[2*i] = center + new Vector2(x, y) * (radius + Vector2.one*outerThickness);
                positions[2*i+1] = center + new Vector2(x, y) *  (radius - Vector2.one * innerThickness);
            }

            for (int i = 0; i < positions.Length; i++) {
                Vector2 p = positions[i];
                Vector2 n = normals[i];

                vh.AddVert(new UIVertex() {
                    position = p,
                    color = color,
                    uv0 = n,
                });
            }

            for (int i = 0; i < segments; i++) {
                vh.AddTriangle(2*i + 0, 2 * i + 1, 2 * i + 2);
                vh.AddTriangle(2*i + 1, 2 * i + 3, 2 * i + 2);
            }
        
        */

        public void CircleOutline(Vector2 center, float radius, int segments = 64, float startAngle = 0, float angle = 1) {
            center += offset;
            segments = Mathf.Max(segments, 3);
            var positions = new Vector2[2 * segments + 2];
            var normals = new Vector2[2 * segments + 2];
            for (int i = 0; i < segments + 1; i++) {
                var ni = (float)i / segments;
                float currentAngle = (startAngle + ni * angle) * 2 * Mathf.PI;
                var x = Mathf.Cos(currentAngle);
                var y = Mathf.Sin(currentAngle);
                normals[2 * i] = new Vector2(x, y);
                normals[2 * i + 1] = new Vector2(-x, -y);

                positions[2 * i] = center + new Vector2(x, y) * (radius);
                positions[2 * i + 1] = center + new Vector2(x, y) * (radius - Thickness);
            }

            var startIndex = _vertexHelper.currentVertCount;
            foreach (var p in positions) {
                _vertexHelper.AddVert(new UIVertex() {
                    position = p,
                    color = Color,
                    uv0 = 0.5f * p + new Vector2(0.5f, 0.5f)
                });
            }

            for (int i = 0; i < segments; i++) {
                _vertexHelper.AddTriangle(startIndex + 2 * i + 0, startIndex + 2 * i + 1, startIndex + 2 * i + 2);
                _vertexHelper.AddTriangle(startIndex + 2 * i + 1, startIndex + 2 * i + 3, startIndex + 2 * i + 2);
            }
        }

    }



}
