using FUI.Gears;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static UnityEngine.EventSystems.PointerEventData;
#nullable enable

namespace FUI {
    public static partial class M {
        public static Modifier AddZoomPanViewport(Vector2 contentSize, InputButtonMask allowedButtons = InputButtonMask.All) =>
            new Modifier(
                "AddZoomPanViewport",
                x => x.AddComponent<Gears.ZoomPanViewport>(),
                x => {
                    var component = x.GetComponent<Gears.ZoomPanViewport>();
                    component.ContentSize = contentSize;
                    component.AllowedButtons = allowedButtons;
                }
                );
    }
}

namespace FUI.Gears {

    public class ZoomPanViewport : FitInside, IDragHandler, IScrollHandler {

        public InputButtonMask AllowedButtons = InputButtonMask.All;

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
            Draggable.HandleDragWithAllowedButtons(
                gameObject,
                eventData,
                AllowedButtons,
                (g,e) => {
                    Move(e.delta);
                }
            );            
        }

        void IScrollHandler.OnScroll(PointerEventData eventData) {
            var cursorInContent = GetMousePositionInContent(eventData);
            var scale = Scale;
            IntScale += Mathf.RoundToInt(eventData.scrollDelta.y);
            var newScale = Scale;

            ViewportCenterInContent += (1 - scale/newScale) * (cursorInContent - ViewportCenterInContent);
        }
    }


}