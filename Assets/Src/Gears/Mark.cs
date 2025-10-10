using UnityEngine;
#nullable enable
namespace FUI.Gears {
    public class Mark : MonoBehaviour {
        public string Identifier;
        /*private object? TemporaryState = null;
        private bool HasTemporaryState = false;
        public bool TryGetTemporaryState<T>(out T? value) where T {
            if (HasTemporaryState) {
                var state = TemporaryState;
                TemporaryState = null;
                HasTemporaryState = false;

                if (state is T casted) {
                    value = casted;
                    return true;
                }
            }
            value = default;
            return false;
        }
        public void SetTemporaryState(object state) {
            TemporaryState = state;
            HasTemporaryState = true;
        }*/
    }
}