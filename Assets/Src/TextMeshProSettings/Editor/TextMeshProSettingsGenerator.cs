using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FUI {
    public static class TextMeshProSettingsGenerator {

        const string SettingsPath = "Assets/TextMeshPro/Resources/TMP Settings.asset";
        [UnityEditor.MenuItem("FUI/Generate TextMeshPro Settings")]
        public static void Generate() {
            var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<TMPro.TMP_Settings>(SettingsPath);
            if (settings == null) {
                //create new settings
                settings = ScriptableObject.CreateInstance<TMPro.TMP_Settings>();
                UnityEditor.AssetDatabase.CreateAsset(settings, SettingsPath);
            }
            //settings.enableWordWrapping = true;
        }
    }
}
