using FUI.Gears;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FUI {

    [CreateAssetMenu(fileName = "ControlsLibrary", menuName = "FUI/ControlsLibrary")]
    public class PrefabLibrary : ScriptableObject {


        public InputFieldState InputField;
        public TMP_Dropdown Dropdown;
        public TMP_Text Label;

        public RectTransform Empty;
        public RoundedRectangle Rectangle;


        public TMP_Text FontAwesomeIconBrands;
        public TMP_Text FontAwesomeIconRegular;
        public TMP_Text FontAwesomeIconSolid;

        public ScrollRect ScrollRect;
    }
}