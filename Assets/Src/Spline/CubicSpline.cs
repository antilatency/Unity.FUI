using System;
using UnityEngine;
using UnityEngine.UI;

namespace FUI {
    [RequireComponent(typeof(CanvasRenderer))]
    public class CubicSpline : MaskableGraphic {
        private Vector2 pointA;
        private Vector2 tangentA;
        private Vector2 pointB;        
        private Vector2 tangentB;

        private float innerThickness = 1;
        private float outerThickness = 1;
        private int numSegments = 64;

        public Vector2 PointA {
            get => pointA;
            set {
                if (pointA == value) return;
                pointA = value;
                SetVerticesDirty();
            }
        }
        public Vector2 TangentA {
            get => tangentA;
            set {
                if (tangentA == value) return;
                tangentA = value;
                SetVerticesDirty();
            }
        }

        public Vector2 PointB {
            get => pointB;
            set {
                if (pointB == value) return;
                pointB = value;
                SetVerticesDirty();
            }
        }
        
        public Vector2 TangentB {
            get => tangentB;
            set {
                if (tangentB == value) return;
                tangentB = value;
                SetVerticesDirty();
            }
        }
        public float InnerThickness {
            get => innerThickness;
            set {
                if (innerThickness == value) return;
                innerThickness = value;
                SetVerticesDirty();
            }
        }
        public float OuterThickness {
            get => outerThickness;
            set {
                if (outerThickness == value) return;
                outerThickness = value;
                SetVerticesDirty();
            }
        }
        public int NumSegments {
            get => numSegments;
            set {
                if (numSegments == value) return;
                numSegments = value;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            FillMesh(vh);
        }

        protected void FillMesh(VertexHelper vh) {
            vh.Clear();
            var rect = rectTransform.rect;


            var a = rect.min + rect.size * pointA;

            var ac = rect.min + rect.size * (pointA+tangentA);

            var b = rect.min + rect.size * pointB;
            var bc = rect.min + rect.size * (pointB + tangentB);

            var positions = new Vector2[2 * numSegments + 2];
            var normals = new Vector2[2 * numSegments + 2];

            for (int i = 0; i <= numSegments; i++) {
                float t = (float)i / numSegments;
                Vector2 point = GetBezierPoint(t,a,ac,bc,b);
                Vector2 tangent = GetBezierTangent(t, a, ac, bc, b).normalized;

                Vector2 normal = new Vector2(-tangent.y, tangent.x);
                normals[2 * i] = normal;
                normals[2 * i + 1] = -normal;

                positions[2 * i] = point + normal * outerThickness;
                positions[2 * i + 1] = point - normal * innerThickness;
            }

            for (int i = 0; i < positions.Length; i++) {
                vh.AddVert(new UIVertex() {
                    position = positions[i],
                    color = color,
                    uv0 = normals[i],
                });
            }

            for (int i = 0; i < numSegments; i++) {
                vh.AddTriangle(2 * i, 2 * i + 1, 2 * i + 2);
                vh.AddTriangle(2 * i + 1, 2 * i + 3, 2 * i + 2);
            }
        }



        private Vector2 GetBezierPoint(float t, Vector2 a, Vector2 ac, Vector2 bc, Vector2 b) {
            // Bezier spline formula for position
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            return uuu * a +           // (1 - t)^3 * StartPoint
                   3 * uu * t * ac + // 3 * (1 - t)^2 * t * ControlPoint1
                   3 * u * tt * bc + // 3 * (1 - t) * t^2 * ControlPoint2
                   ttt * b;              // t^3 * EndPoint
        }

        private Vector2 GetBezierTangent(float t, Vector2 a, Vector2 ac, Vector2 bc, Vector2 b) {
            // Bezier spline formula for tangent (derivative of position formula)
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            return -(-3 * uu * a +
                   (3 * uu - 6 * u * t) * ac +
                   (6 * u * t - 3 * tt) * bc +
                   3 * tt * b);
        }

    }
}