using System;
using UnityEngine;
using UnityEngine.UI;

namespace FUI {
    [RequireComponent(typeof(CanvasRenderer))]
    public class CircleOutline : MaskableGraphic {
        private float startAngle = 0;
        private float angle = 1;
        private float innerThickness = 1;
        private float outerThickness = 1;

        private int numSegments = 64;

        public float StartAngle {
            get => startAngle;
            set {
                if (startAngle == value) return;
                startAngle = value;
                SetVerticesDirty();
            }
        }
        public float Angle {
            get => angle;
            set {
                if (angle == value) return;
                angle = value;
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
        }
    }
}