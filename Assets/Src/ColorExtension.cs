using UnityEngine;
namespace FUI {
    public static class ColorExtension {
        public static Color Alpha(this Color color, float alpha) {
            return new Color(
                color.r,
                color.g,
                color.b,
                alpha
            );
        }

        public static Color MultiplySaturationBrightness(this Color color, float saturationMultiplier, float brightnessMultiplier = 1) {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            s = Mathf.Clamp01(s * saturationMultiplier);
            v = Mathf.Clamp01(v * brightnessMultiplier);
            var result = Color.HSVToRGB(h, s, v);
            return result.Alpha(color.a);
        }



        public static Color Multiply(this Color color, float multiplier) {
            return new Color(
                color.r * multiplier,
                color.g * multiplier,
                color.b * multiplier,
                color.a // Keep alpha unchanged
            );
        }

        public static Color Normalize(this Color color) {
            float maxChannel = Mathf.Max(color.r, color.g, color.b);
            if (maxChannel == 0)
                return new Color(0, 0, 0, color.a); // Avoid division by zero
            return new Color(
                color.r / maxChannel,
                color.g / maxChannel,
                color.b / maxChannel,
                color.a // Keep alpha unchanged
            );
        }

        public static Color ContrastColor(this Color color, Color? light = null, Color? dark = null) {
            if (light == null) light = Color.white;
            if (dark == null) dark = Color.black;

            float luminance = (0.299f * color.r + 0.587f * color.g + 0.114f * color.b);
            return luminance > 0.5f ? dark.Value : light.Value;
        }
    }
}