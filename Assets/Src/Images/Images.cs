#nullable enable
using FUI.Modifiers;
using UnityEngine;
using static FUI.Basic;

namespace FUI {

    public static partial class Shortcuts {

        public static void Image_ToSRGB_IgnoreAlpha(Positioner positioner, Texture texture, Vector3? multiplier = null) {            
            var m = multiplier ?? Vector3.one;
            Image(positioner, texture, m, new Vector4(0, 0, 0, 1), true);

        }

        public static void Image(Positioner positioner, Texture texture, Vector4 multiplier, Vector4 increment, bool toSrgb) {
            var form = Form.Current;
            var element = Element(null
                , new AddComponent<RoundedRectangle>()
                , new SetCustomShader("FUI/Image"
                    , ("Texture", texture)
                    , ("TO_SRGB", toSrgb)
                    , ("Multiplier", multiplier)
                    , ("Increment", increment)
                )
            );
            positioner(element, form.CurrentBorders, () => new Vector2(texture.width, texture.height));
        }
    }
}