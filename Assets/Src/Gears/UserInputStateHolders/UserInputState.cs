namespace FUI.Gears {
    public class UserInputState<T> : AbstractUserInput<T> {

        private T _value;

        public override T Value {
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
            FormToNotify?.MakeDirty();
        }
    }
}