using UnityEngine;
namespace FUI.Gears {
    public class UserInputState<T> : MonoBehaviour {

        private T _value;
        public bool NewUserInput { get; protected set; }

        public T Value {
            set {
                _value = value;
            }
            get {
                NewUserInput = false;
                return _value;
            }
        }

        public void UserInput(T value) {
            _value = value;
            NewUserInput = true;
        }
    }
}