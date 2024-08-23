using UnityEngine;

namespace FUI {
    [CreateAssetMenu(fileName = "Theme", menuName = "FUI/Theme")]
    public class Theme : ScriptableObject {

        private static Theme _instance;
        public static Theme Instance {
            get {
                if (_instance == null) {
                    _instance = Resources.Load<Theme>("FUI.DefaultTheme");
                }
                return _instance;
            }
        }



        public Color WindowBackgroundColor = new Color32(0x30, 0x30, 0x30, 255);
        public Color LabelColor = Color.white;
        public Color PrimaryColor = new Color32(0xAC, 0xC4, 0x35, 255);

        public Color InputColor = Color.black.Alpha(0.25f);
        public Color InputColorHovered = Color.black.Alpha(0.5f);
        public Color InputColorEditing = Color.black;

        public Color ButtonColor = Color.white.Alpha(0.1f);
        public Color ButtonColorHovered = Color.white.Alpha(0.2f);
        public Color ButtonColorPressed = Color.white.Alpha(0.3f);

        public Color TransparentButtonColor = Color.white.Alpha(0);
        public Color TransparentButtonColorHovered = Color.white.Alpha(0.1f);
        public Color TransparentButtonColorPressed = Color.white.Alpha(0.2f);


        public Color PanelBackgroundColor = Color.white.Alpha(0.1f);
        public float LineHeight = 20;
        public float DefaultGap = 4;
    }
}