using UnityEngine;
using UnityEngine.UI;

namespace FUI {
    internal class SimpleRoundedRectangle : AbstractRoundedRectangle{
        public float Radius = 10f;


        protected override void OnPopulateMesh(VertexHelper vh){
            FillMesh(new float[]{ Radius, Radius, Radius, Radius }, vh);
        }
    }
}