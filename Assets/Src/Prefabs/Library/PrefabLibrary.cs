
using UnityEngine;

namespace FUI {

    [CreateAssetMenu(fileName = "ControlsLibrary", menuName = "FUI/ControlsLibrary")]
    public class PrefabLibrary : ScriptableObject {


        public GameObject InputField;
        //public GameObject Dropdown;
        //public GameObject Label;

        //public GameObject FontAwesomeIcon;

        //public GameObject ScrollRect;


        private static PrefabLibrary _instance = null!;
        public static PrefabLibrary Instance {
            get {
                if (_instance == null)
                    _instance = Resources.Load<PrefabLibrary>("FUI.PrefabLibrary");
                return _instance;
            }
        }
    }
}