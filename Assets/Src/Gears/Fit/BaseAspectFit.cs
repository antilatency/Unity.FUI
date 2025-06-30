using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable

namespace FUI.Gears {
    public abstract class BaseAspectFit : UIBehaviour {

        bool _dirty = true;
        protected void MarkDirty() {
            if (!_dirty) {
                _dirty = true;
            }
        }

        private void SetValue<T>(ref T field, T value) {
            if (!EqualityComparer<T>.Default.Equals(field, value)) {
                field = value;
                MarkDirty();
            }
        }

        [SerializeField]
        private Vector2 _contentSize;
        public Vector2 ContentSize {
            get => _contentSize;
            set => SetValue(ref _contentSize, value);
        }

        //private Vector2 _viewportSize;
        public Vector2 ViewportSize => ((RectTransform)transform).rect.size;

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
                    MarkDirty();
                }
                return _content!;
            }
        }

        protected virtual float FitScale { 
            get {
                if (ContentSize.x == 0 || ContentSize.y == 0)
                    return 1f;
                var scale2d = ViewportSize / ContentSize;
                return Mathf.Min(scale2d.x, scale2d.y);
            }
        }

        [SerializeField]
        private Vector2 _viewportCenterInContent;
        public Vector2 ViewportCenterInContent {
            get => _viewportCenterInContent;
            set => SetValue(ref _viewportCenterInContent, value);
        }

        private float _scale = 1;
        public float Scale {
            get => _scale;
            set => SetValue(ref _scale, value);
        }

        protected override void OnEnable() {
            MarkDirty();
        }

        protected override void OnRectTransformDimensionsChange() {
            MarkDirty();
        }


        

        void Update() {
            if (!_dirty) return;            
            

            var content = Content;

            content.sizeDelta = _contentSize;
            content.localScale = Vector3.one * FitScale * _scale;
            content.localPosition = -_viewportCenterInContent * _contentSize * (FitScale * _scale);

            _dirty = false;

            //Debug.Log($"ViewportSize: {ViewportSize}, ContentSize: {ContentSize}, FitScale: {FitScale}, Scale: {Scale}, ContentPosition: {_content?.localPosition}");
        }

        
    }


}