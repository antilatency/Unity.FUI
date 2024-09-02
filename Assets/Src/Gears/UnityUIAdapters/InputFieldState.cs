namespace FUI.Gears {

    public class InputFieldState : AbstractUserInput<string> {

        public FUI.FUI_InputField Target;
        

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
            NotifyForm();
        }

        public void OnEndEdit(string value) {
            NotifyForm();
        }
    }
}