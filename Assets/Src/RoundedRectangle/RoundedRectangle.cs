using UnityEngine;
using UnityEngine.UI;

namespace FUI{

    [RequireComponent(typeof(CanvasRenderer))]
    public class RoundedRectangle : AbstractRoundedRectangle{

        public float TopLeft;
        public float TopRight;
        public float BottomLeft;
        public float BottomRight;

        protected override void OnPopulateMesh(VertexHelper vh){
            FillMesh(new float[]{ TopRight, TopLeft, BottomLeft, BottomRight }, vh);
        }

        public void SetAllCorners(float radius) {
            TopLeft = radius;
            TopRight = radius;
            BottomLeft = radius;
            BottomRight = radius;
        }
    }
}
