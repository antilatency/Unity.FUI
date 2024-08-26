namespace FUI.Gears {

    public class InputFieldState : AbstractUserInput<string> {

        public TMPro.TMP_InputField Target;
        

        public override string Value {
            set {
                Target.SetTextWithoutNotify(value);
            }
            get {
                NewUserInput = false;
                return Target.text;
            }
        }

        public void OnValueChanged(string value) {
            NewUserInput = true;
            FormToNotify?.MakeDirty();
        }
    }
}