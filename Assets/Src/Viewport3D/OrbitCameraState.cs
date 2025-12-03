using UnityEngine;
using UnityEngine.EventSystems;


#nullable enable

namespace FUI {
    [System.Serializable]
    public class OrbitCameraState : CameraState {
        public float FOV = 45f;
        public Vector3 Target;
        public float Distance = 5f;
        public int IntScale = 0;
        public float Yaw; //in rotations around Y axis
        public float Pitch; //in rotations around X axis
        public float MinDistance = 0.1f;
        public float MaxDistance = 100f;
        public float PixelsPerRevolution = 500f;
        public float ZoomPower = 1.05f;
        public Quaternion Rotation => Quaternion.Euler(Pitch * 360f, Yaw * 360f, 0f);
        public float ZoomFactor => Mathf.Pow(ZoomPower, IntScale);
        public float DistanceScaled => Mathf.Clamp(Distance * ZoomFactor, MinDistance, MaxDistance);
        public Vector2 ViewportSize = Vector2.one * 256f;
        public override void SetViewportSize(Vector2 size) {
            ViewportSize = size;
        }
        public override void SetupCamera(Camera camera) {
            float d = DistanceScaled;
            var rotation = Rotation;
            camera.transform.position = Target - (rotation * Vector3.forward * d);
            camera.transform.rotation = rotation;
            camera.fieldOfView = FOV;
        }
        public override void HandleDrag(PointerEventData eventData) {
            var delta = eventData.delta;
            bool middleButton = eventData.button == PointerEventData.InputButton.Middle;
            if (middleButton) { // Pan
                var distance = DistanceScaled;
                var heightAtDistance = 2f * distance * Mathf.Tan(0.5f * FOV * Mathf.Deg2Rad);
                
                float panSpeed = heightAtDistance / ViewportSize.y; // world units per pixel
                var rotation = Rotation;
                var right = rotation * Vector3.right;
                var up = rotation * Vector3.up;
                Target -= (right * delta.x + up * delta.y) * panSpeed;
                return;
            }
            else { // Rotate
                float dragSpeed = 1f / PixelsPerRevolution; // rotations per pixel
                Yaw += delta.x * dragSpeed;
                Pitch -= delta.y * dragSpeed;
                Pitch = Mathf.Clamp(Pitch, -0.25f, 0.25f); // Limit pitch to avoid flipping
            }
        }
        public override void HandleScroll(PointerEventData eventData) {
            float delta = eventData.scrollDelta.y;
            IntScale -= Mathf.RoundToInt(delta);
            
        }
    }
}


