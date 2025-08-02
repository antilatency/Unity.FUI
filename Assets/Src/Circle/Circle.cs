using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FUI {


    [RequireComponent(typeof(CanvasRenderer))]
    public class Circle : MaskableGraphic {
        private float startAngle = 0;
        private float angle = 1;
        private int numSegments = 64;

        public float StartAngle { get => startAngle;
            set{
                if (startAngle == value) return;
                startAngle = value;
                SetVerticesDirty();
            }
        }
        public float Angle { get => angle;
            set {
                if (angle == value) return;
                angle = value;
                SetVerticesDirty();
            }
        }
        public int NumSegments { get => numSegments;
            set {
                if (numSegments == value) return;
                numSegments = value;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            FillMesh(vh);
        }

        protected void FillMesh(VertexHelper vh){
            vh.Clear();

            var rect = rectTransform.rect;
            var center = rect.center;
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
            //vh.AddTriangle(0, positions.Length-1, 1);
        }
    }
}