using TMPro;
using UnityEngine;

namespace FUI.Gears {
    public class DropdownState : AbstractUserInput<int> {

        public TMP_Dropdown Target;

        public override int Value {
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
            FormToNotify?.MakeDirty();
        }
    }
}