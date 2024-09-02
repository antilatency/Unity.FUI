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

    public class ZoomPanViewport : UIBehaviour, IDragHandler, IEndDragHandler,  IScrollHandler {
        public bool Moving { get; private set; } = false;

        [SerializeField]
        private Vector2 contentSize;
        public Vector2 ContentSize {
            get => contentSize;
            set {
                if (contentSize != value) {
                    contentSize = value;
                    if (content != null) {
                        content.sizeDelta = contentSize;
                        UpdateFitScale();
                        
                    }
                }
            }
        }


        public RectTransform? content;

        public RectTransform Content {
            get {
                if (!content) {
                    var go = new GameObject("Content");
                    go.transform.SetParent(transform, false);
                    content = go.AddComponent<RectTransform>();                    
                    content.anchoredPosition = Vector2.zero;
                    content.anchorMin = Vector2.one * 0.5f;
                    content.anchorMax = Vector2.one * 0.5f;
                    content.sizeDelta = contentSize;
                    ApplyContentScale();
                    ApplyContentPosition();
                }
                return content!;
            }
        }

        
        public float FitScale = 1;
        void UpdateFitScale() {
            if (ContentSize.x == 0 || ContentSize.y == 0)
                return;
            var scale2d = ViewportSize / ContentSize;
            FitScale = Mathf.Min(scale2d.x, scale2d.y);
            ApplyContentScale();
        }

        private Vector2 viewportSize;
        public Vector2 ViewportSize {
            get => viewportSize;
            set {
                if (viewportSize != value) {
                    viewportSize = value;
                    UpdateFitScale();
                    
                }
            }
        }
        void ReacquireViewportSize() { 
            ViewportSize = ((RectTransform)transform).rect.size;
        }

        private int intScale;
        public float Scale => Mathf.Pow(1.1f, IntScale);

        public int IntScale { get => intScale;
            set {
                if (intScale != value) {
                    intScale = value;
                    ApplyContentScale();
                }
            }
        }        

        void ApplyContentScale() {
            if (content == null) return;
            content.localScale = Vector3.one * FitScale * Scale;
            ApplyContentPosition();
        }


        protected override void OnEnable() {
            ReacquireViewportSize();
        }

        protected override void OnRectTransformDimensionsChange() {
            ReacquireViewportSize();
        }


        void ApplyContentPosition() {
            if (content == null) return;
            content.localPosition = -viewportCenterInContent * contentSize * (FitScale*Scale);
        }
        [SerializeField]
        private Vector2 viewportCenterInContent;
        public Vector2 ViewportCenterInContent {
            get => viewportCenterInContent;
            set {
                if (viewportCenterInContent != value) {
                    viewportCenterInContent = value;
                    ApplyContentPosition();
                }
            }
        
        }

        public void Move(Vector2 deltaXY) {
            ViewportCenterInContent -= deltaXY / ContentSize / (FitScale * Scale);
        }
       

        private Vector2 GetMousePositionInContent(PointerEventData eventData) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position, eventData.pressEventCamera, out var localPoint);

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