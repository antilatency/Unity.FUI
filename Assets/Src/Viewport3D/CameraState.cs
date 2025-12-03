using UnityEngine;
using UnityEngine.EventSystems;


#nullable enable

namespace FUI {
    public abstract class CameraState {
        public abstract void SetupCamera(Camera camera);
        public abstract void HandleDrag(PointerEventData eventData);
        public abstract void HandleScroll(PointerEventData eventData);
        public abstract void SetViewportSize(Vector2 size);
    }
}


