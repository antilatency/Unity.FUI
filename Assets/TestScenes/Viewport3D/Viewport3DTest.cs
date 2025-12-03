using UnityEngine;
using FUI;
using FUI.Gears;
using static FUI.Shortcuts;
using static FUI.Basic;
using FUI.Modifiers;

#nullable enable



namespace FUI.Gears {

    
}




public class Viewport3DTest : Form {

    public Camera? Camera3D;
    private RenderTexture? RenderTexture;
    public OrbitCameraState CameraState = new OrbitCameraState() {
        Target = Vector3.zero,
        Distance = 5f,
        Yaw = 0.125f,
        Pitch = 0.1f
    };

    void CheckRenderTexture() {
        if (RenderTexture == null) {
            var descriptor = new RenderTextureDescriptor(256, 256) {
                graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB,
                depthBufferBits = 24,
                msaaSamples = 4,
                sRGB = false
            };
            RenderTexture = new RenderTexture(descriptor);
        }
        if (Camera3D != null) {
            Camera3D.targetTexture = RenderTexture;
            Camera3D.ResetAspect();
            CameraState.SetupCamera(Camera3D);
        }
    }

    protected override void Update() {
        CheckRenderTexture();
        base.Update();
    }

    protected override void Build() {
        

        using (WindowBackground()) {

            using (Panel(P.Left(80))) {
                Button("Button 1", () => { });
            }

            Viewport3D(P.Fill, RenderTexture!, CameraState);

        }
    }
}