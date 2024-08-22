using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FUI{
    public abstract class AbstractRoundedRectangle : MaskableGraphic{
        public int Segments = 4;

        protected abstract override void OnPopulateMesh(VertexHelper vh);

        protected void FillMesh(float[] radiusCorners, VertexHelper vh){
            vh.Clear();

            var rect = rectTransform.rect;
            var positions = GenerateRoundedRectanglePoints(rect, radiusCorners, Segments);

            foreach (var p in positions){
                var uv = (p - rect.position) / rect.size;

                vh.AddVert(new UIVertex(){
                    position = p,
                    color = color,
                    uv0 = uv,
                });
            }

            for (int i = 0; i < positions.Length - 2; i++){
                vh.AddTriangle(0, i + 1, i + 2);
            }
        }

        Vector2[] GenerateRoundedRectanglePoints(Rect rect, float[] radiusCorners, int segments){

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