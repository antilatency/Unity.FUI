using System.Collections.Generic;

namespace FUI.Gears {
    public class UserInputState<T> : AbstractUserInput<T> where T : struct, System.IEquatable<T> {

        protected T _value;

        public override T Value {
            set {
                _value = value;
            }
            get {
                NewUserInput = false;
                return _value;
            }
        }

        protected virtual bool Equals(T a, T b) {
            return a.Equals(b);
        }

        public void UserInput(T value) {
            if (Equals(_value, value)) {
                return; // No change in value
            }
            _value = value;
            NewUserInput = true;
            NotifyForm();
        }
    }
}