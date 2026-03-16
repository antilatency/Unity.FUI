
using UnityEngine;

namespace FUI {

    public class PrefabLibrary : ScriptableObject {


        public GameObject InputField;


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