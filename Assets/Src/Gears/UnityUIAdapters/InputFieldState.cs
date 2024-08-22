using UnityEngine;
namespace FUI.Gears {
    public class InputFieldState : MonoBehaviour {

        public TMPro.TMP_InputField Target;
        public bool NewUserInput { get; protected set; }

        public string Value {
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
        }
    }
}