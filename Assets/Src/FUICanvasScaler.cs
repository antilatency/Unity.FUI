using UnityEngine;
#nullable enable

namespace FUI {
    public class FUICanvasScaler : MonoBehaviour{
        private float _previousValue = -1;
        private void Update() {
            if (_previousValue != Screen.dpi) {
                _previousValue = Screen.dpi;
                GetComponent<Canvas>().scaleFactor = _previousValue / 96f;
            }
        }
    }
}
