using FUI.Gears;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace FUI.Modifiers {
    public class AddCameraStateController : AddComponentConfigured<CameraStateController> {
        public CameraState? State;
        public AddCameraStateController(CameraState? state = null) {
            State = state;
        }
        public override void Configure(CameraStateController component) {
            component.State = State;
        }
    }
}

namespace FUI.Gears {
    public class CameraStateController : UIBehaviour, IScrollHandler, IDragHandler {
        public CameraState? State;
        public PointerEventUtils.PointerEventFilter? ScrollFilter = null;
        public PointerEventUtils.PointerEventFilter? DragFilter = null;

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if (!PointerEventUtils.HandleDragFilter(gameObject, eventData, DragFilter)) {
                return;
            }
            State?.HandleDrag(eventData);
        }
        void IScrollHandler.OnScroll(PointerEventData eventData) {
            if (!PointerEventUtils.HandleScrollFilter(gameObject, eventData, ScrollFilter)) {
                return;
            }
            State?.HandleScroll(eventData);
        }
        protected override void OnEnable() {
            var size = ((RectTransform)transform).rect.size;
            State?.SetViewportSize(size);
        }
        protected override void OnRectTransformDimensionsChange() {
            base.OnRectTransformDimensionsChange();
            var size = ((RectTransform)transform).rect.size;
            State?.SetViewportSize(size);
        }
    }
}


