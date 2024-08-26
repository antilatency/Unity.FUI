
using UnityEngine;

namespace FUI {

    [CreateAssetMenu(fileName = "ControlsLibrary", menuName = "FUI/ControlsLibrary")]
    public class PrefabLibrary : ScriptableObject {


        public GameObject InputField;
        public GameObject Dropdown;
        public GameObject Label;

        public GameObject FontAwesomeIcon;

        public GameObject ScrollRect;
    }
}