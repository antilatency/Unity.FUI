using FUI.Gears;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static UnityEngine.EventSystems.PointerEventData;
#nullable enable

namespace FUI.Modifiers {
    public class AddZoomPanViewport : Modifier {
        public Vector2 contentSize;
        public PointerEventUtils.PointerEventFilter? scrollFilter = null;
        public PointerEventUtils.PointerEventFilter? dragFilter = null;
        public AddZoomPanViewport(Vector2 contentSize, PointerEventUtils.PointerEventFilter? scrollFilter = null, PointerEventUtils.PointerEventFilter? dragFilter = null) {
            this.contentSize = contentSize;
            this.scrollFilter = scrollFilter;
            this.dragFilter = dragFilter;
        }
        public override void Create(GameObject gameObject) {
            gameObject.AddComponent<ZoomPanViewport>();
        }
        public override void Update(GameObject gameObject) {
            var component = gameObject.GetComponent<ZoomPanViewport>();
            component.ContentSize = contentSize;
            component.ScrollFilter = scrollFilter;
            component.DragFilter = dragFilter;
        }
    }
    /*public static partial class M {
        public static Modifier AddZoomPanViewport(Vector2 contentSize, PointerEventUtils.PointerEventFilter? scrollFilter = null, PointerEventUtils.PointerEventFilter? dragFilter = null ) =>
            new Modifier(
                "AddZoomPanViewport",
                x => x.AddComponent<Gears.ZoomPanViewport>(),
                x => {
                    var component = x.GetComponent<Gears.ZoomPanViewport>();
                    component.ContentSize = contentSize;
                    component.ScrollFilter = scrollFilter;
                    component.DragFilter = dragFilter;
                }
                );
    }*/

}

namespace FUI.Gears {

    public class ZoomPanViewport : FitInside, IDragHandler, IScrollHandler {

        

        public PointerEventUtils.PointerEventFilter? ScrollFilter = null;
        public PointerEventUtils.PointerEventFilter? DragFilter = null;
        //public InputButtonMask AllowedButtons = InputButtonMask.All;

        private int intScale;
        public int IntScale {
            get => intScale;
            set {
                if (intScale != value) {
                    intScale = value;
                    Scale = Mathf.Pow(1.1f, IntScale);
                }
            }
        }


        public void Move(Vector2 deltaXY) {
            if (ContentSize.x == 0 || ContentSize.y == 0 || FitScale==0 || Scale==0)
                return;

            ViewportCenterInContent -= deltaXY / ContentSize / (FitScale * Scale);
        }
       

        private Vector2 GetMousePositionInContent(PointerEventData eventData) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, eventData.position, eventData.enterEventCamera, out var localPoint);
            return localPoint / ContentSize;
        }


        void IDragHandler.OnDrag(PointerEventData eventData) {
            if (DragFilter != null && !DragFilter(gameObject, eventData)) {
                var parent = gameObject.transform.parent;
                ExecuteEvents.ExecuteHierarchy(
                    parent.gameObject,
                    eventData,
                    ExecuteEvents.dragHandler
                );
                return;
            }
            Move(eventData.delta);
            /*Draggable.HandleDragWithAllowedButtons(
                gameObject,
                eventData,
                AllowedButtons,
                (g,e) => {
                    Move(e.delta);
                }
            );  */          
        }

        void IScrollHandler.OnScroll(PointerEventData eventData) {
            if (ScrollFilter != null && !ScrollFilter(gameObject, eventData)) {
                var parent = gameObject.transform.parent;
                ExecuteEvents.ExecuteHierarchy(
                    parent.gameObject,
                    eventData,
                    ExecuteEvents.scrollHandler
                );
                return;
            }


            var cursorInContent = GetMousePositionInContent(eventData);
            var scale = Scale;
            IntScale += Mathf.RoundToInt(eventData.scrollDelta.y);
            var newScale = Scale;

            ViewportCenterInContent += (1 - scale/newScale) * (cursorInContent - ViewportCenterInContent);
        }
    }


}