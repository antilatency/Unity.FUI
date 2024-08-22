using TMPro;
using UnityEngine;

namespace FUI.Gears {
    public class DropdownState : MonoBehaviour {

        public TMP_Dropdown Target;

        public bool NewUserInput { get; protected set; }

        public int Value {
            set {
                Target.SetValueWithoutNotify(value);
            }
            get {
                NewUserInput = false;
                return Target.value;
            }
        }

        public void OnValueChanged(int value) {
            NewUserInput = true;
        }
    }
}