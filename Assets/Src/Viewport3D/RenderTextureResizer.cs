using FUI.Gears;
using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable


namespace FUI.Modifiers {
    public class AddRenderTextureResizer : AddComponentConfigured<RenderTextureResizer> {
        public RenderTexture? RenderTexture;
        public AddRenderTextureResizer(RenderTexture? renderTexture = null) {
            RenderTexture = renderTexture;
        }
        public override void Configure(RenderTextureResizer component) {
            component.RenderTexture = RenderTexture;
        }
    }
}
namespace FUI.Gears {


    public class RenderTextureResizer : UIBehaviour {
        public RenderTexture? RenderTexture;

        protected override void OnRectTransformDimensionsChange() {
            var size = ((RectTransform)transform).rect.size;
            ResizeRenderTexture(Vector2Int.CeilToInt(size));
        }

        void ResizeRenderTexture(Vector2Int size) {
            if (RenderTexture == null) {
                return;
            }

            if (RenderTexture.width != size.x || RenderTexture.height != size.y) {
                RenderTexture.Release();
                RenderTexture.width = size.x;
                RenderTexture.height = size.y;
            }
        }
    }
}


