using System;

using UnityEngine;

namespace FUI {

    public interface IThemeListener {
        Theme Theme { get; }
        void ThemeChanged();
    }

    [CreateAssetMenu(fileName = "Theme", menuName = "FUI/Theme")]
    public class Theme : ScriptableObject {

        /*private static Theme _instance;
        public static Theme Instance {
            get {
                if (_instance == null) {
                    _instance = Resources.Load<Theme>("FUI.DefaultTheme");
                }
                return _instance;
            }
        }*/
        private static Theme _default;
        public static Theme Default {
            get {
                if (_default == null) {
                    _default = Resources.Load<Theme>("FUI.DefaultTheme");
                    if (_default == null)
                        throw new Exception("Default theme (FUI.DefaultTheme) not found in resources.");
                }
                return _default;
            }
        }

        private static void BroadcastToScene(string methodName, object parameter = null) {
            foreach (GameObject gameobject in GameObject.FindObjectsOfType<GameObject>()) {
                gameobject.SendMessage(methodName, parameter, SendMessageOptions.DontRequireReceiver);
            }
        }

        public void OnValidate() {
            #if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return; // Skip runtime updates in edit mode
            #endif
            BroadcastToScene("ThemeChanged", this);

        }

        public static Color Gray(float value) {
            return new Color(value, value, value, 1f);
        }

        public Color WindowBackgroundColor = new Color32(0x30, 0x30, 0x30, 255);
        public Color PopupBackgroundColor = new Color32(0x35, 0x35, 0x35, 255);
        
        public Color PrimaryColor = new Color32(0xAC, 0xC4, 0x35, 255);

        public Color newButtonColor = Gray(0.2f);

        [Header("Label")]
        public Color LabelColor = Color.white;
        public Color LabelColorHovered = Gray(0.9f);
        public Color LabelColorPressed = Gray(0.8f);
        public Color LabelColorDisabled = Gray(0.5f);
        public Color LabelColorSelected = new Color32(0xAC, 0xC4, 0x35, 255);

        [Header("Input Field")]
        public Color InputColor = Gray(0.04f);
        public Color InputHoveredColor = Gray(0);
        public Color InputOutlineColor = Gray(0.4f);
        public Color InputSelectionColor = new Color32(0x40, 0x4D, 0x00, 255);
        /*
        backgroundColor.MultiplySaturationBrightness(1.1f, 1.1f),
                                backgroundColor.MultiplySaturationBrightness(1.3f, 1.3f)
        */
        [Header("Button")]
        public Color ButtonColor = Gray(0.2f);
        public Color ButtonHoveredColor = Gray(0.3f);
        public Color ButtonPressedColor = Gray(0.4f);
        public Color ButtonTextColor = Color.white;

        public Color DisabledButtonColor = Gray(0.1f);
        public Color DisabledButtonTextColor = Gray(0.5f);

        [Header("Selected Item")]
        public Color SelectedButtonColor = new Color32(0xAC, 0xC4, 0x35, 255);
        public Color SelectedButtonHoveredColor = new Color32(0xCC, 0xE4, 0x55, 255);
        public Color SelectedButtonPressedColor = new Color32(0xEC, 0xF4, 0x75, 255);
        public Color SelectedButtonTextColor = Color.black;

        public float Radius = 4;


        [Header("Hover")]
        public float HoverBrightness = 1.1f;
        public float HoverSaturation = 1.1f;
        public Color HoverColor(Color baseColor) {
            return baseColor.MultiplySaturationBrightness(HoverSaturation, HoverBrightness);
        }

        [Header("Pressed")]
        public float PressedBrightness = 1.3f;
        public float PressedSaturation = 1.3f;
        public Color PressedColor(Color baseColor) {
            return baseColor.MultiplySaturationBrightness(PressedSaturation, PressedBrightness);
        }



        public Color TransparentButtonColor = Color.white.Alpha(0);
        public Color TransparentButtonColorHovered = Color.white.Alpha(0.1f);
        public Color TransparentButtonColorPressed = Color.white.Alpha(0.2f);


        public Color PanelBackgroundColor = Color.white.Alpha(0.1f);
        public float LineHeight = 20;
        public float DefaultGap = 4;
        public float OutlineThickness = 1;
        public float ButtonHorizontalPadding = 4;
        
    }
}