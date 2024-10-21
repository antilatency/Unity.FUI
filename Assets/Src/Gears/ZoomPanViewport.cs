using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable

namespace FUI {
    public static partial class M {
        public static Modifier AddZoomPanViewport(Vector2 contentSize) =>
            new Modifier(
                "AddZoomPanViewport",
                x => x.AddComponent<Gears.ZoomPanViewport>(),
                x => {
                    var component = x.GetComponent<Gears.ZoomPanViewport>();
                    component.ContentSize = contentSize;
                }
                );
    }
}

namespace FUI.Gears {
    public class FitPixelSize : BaseAspectFit {
        public override float? CalcFitScale(Vector2 viewportSize, Vector2 contentSize) {
            return 1f;
        }
    }

    public class ZoomPanViewport : FitInside, IDragHandler, IEndDragHandler,  IScrollHandler {
        public bool Moving { get; private set; } = false;

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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, eventData.position, eventData.pressEventCamera, out var localPoint);

            return localPoint / ContentSize;
        }


        void IDragHandler.OnDrag(PointerEventData eventData) {
            Move(eventData.delta);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            Moving = false;
        }


        void IScrollHandler.OnScroll(PointerEventData eventData) {
            var a = GetMousePositionInContent(eventData);
            IntScale += Mathf.RoundToInt(eventData.scrollDelta.y);
            var b = GetMousePositionInContent(eventData);
            ViewportCenterInContent += a - b;
        }
    }


}