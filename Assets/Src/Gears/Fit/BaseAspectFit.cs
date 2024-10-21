using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Gears {
    public abstract class BaseAspectFit : UIBehaviour {
        [SerializeField]
        private Vector2 _contentSize;
        public Vector2 ContentSize {
            get => _contentSize;
            set {
                if (_contentSize != value) {
                    _contentSize = value;
                    if (_content != null) {
                        _content.sizeDelta = _contentSize;
                        UpdateFitScale();

                    }
                }
            }
        }

        private Vector2 _viewportSize;
        public Vector2 ViewportSize {
            get => _viewportSize;
            protected set {
                if (_viewportSize != value) {
                    _viewportSize = value;
                    UpdateFitScale();

                }
            }
        }

        public RectTransform? _content;

        public RectTransform Content {
            get {
                if (!_content) {
                    var go = new GameObject("Content");
                    go.transform.SetParent(transform, false);
                    _content = go.AddComponent<RectTransform>();
                    _content.anchoredPosition = Vector2.zero;
                    _content.anchorMin = Vector2.one * 0.5f;
                    _content.anchorMax = Vector2.one * 0.5f;
                    _content.sizeDelta = _contentSize;
                    ApplyContentScaleAndPosition();
                }
                return _content!;
            }
        }

        protected float FitScale = 1;

        public abstract float? CalcFitScale(Vector2 viewportSize, Vector2 contentSize);


        void UpdateFitScale() {
            FitScale = CalcFitScale(ViewportSize, ContentSize) ?? FitScale;            
            ApplyContentScaleAndPosition();
        }

        void ReacquireViewportSize() {
            ViewportSize = ((RectTransform)transform).rect.size;
        }

        
        private float _scale = 1;
        public float Scale {
            get => _scale;
            set {
                if (_scale == value) return;
                _scale = value;
                ApplyContentScaleAndPosition();
            }
        }


        

        void ApplyContentScaleAndPosition() {
            if (_content == null) return;
            _content.localScale = Vector3.one * FitScale * Scale;
            ApplyContentPosition();
        }


        protected override void OnEnable() {
            ReacquireViewportSize();
        }

        protected override void OnRectTransformDimensionsChange() {
            ReacquireViewportSize();
        }


        void ApplyContentPosition() {
            if (_content == null) return;
            _content.localPosition = -viewportCenterInContent * _contentSize * (FitScale * Scale);
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

        
    }


}