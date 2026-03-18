
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System.Reflection;
using UnityEditor.Compilation;
using System.IO;

namespace FUI {

    public class EndNameEditActionCallStaticMethod : UnityEditor.ProjectWindowCallback.EndNameEditAction {
        
        public string ClassName;
        public string MethodName;
        public EndNameEditActionCallStaticMethod Init(string className, string methodName) {
            ClassName = className;
            MethodName = methodName;
            var method = GetMethod();
            if (method == null) {
                Debug.LogError($"Class {ClassName} does not have method static void {MethodName} (int instanceId, string pathName, string resourceFile).");
            }
            return this;
        }

        MethodInfo GetMethod() {
            var type = System.Type.GetType(ClassName);
            if (type == null) {
                Debug.LogError($"Class {ClassName} not found.");
                return null;
            }
            //find the method with signature void (int instanceId, string pathName, string resourceFile)
            var method = type.GetMethod(MethodName
            , BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
            , null
            , new System.Type[] { typeof(int), typeof(string), typeof(string) }
            , null);
            if (method == null) {
                Debug.LogError($"Method {MethodName} not found in class {ClassName}.");
                return null;
            }
            return method;
        }

        public override void Action(int instanceId, string pathName, string resourceFile) {
            var method = GetMethod();
            method?.Invoke(null, new object[] { instanceId, pathName, resourceFile });
            
        }
    }


    public static class MenuCommands {

        [MenuItem("Assets/Create/FUI/Scene", false, 0)]
        public static void BeginCreateScene() {
            var icon = EditorGUIUtility.FindTexture(
                "UnityLogo");
            BeginCreate(
                typeof(MenuCommands).FullName,
                nameof(CreateScene),
                "FUI Scene.unity",
                icon
            );
        }       

        public static void CreateScene(int instanceId, string pathName, string resourceFile) {

            //create a new scene with the name pathName
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                UnityEditor.SceneManagement.NewSceneMode.Single
            );
            var cameraGameObject = new GameObject("Camera");
            var camera = cameraGameObject.AddComponent<Camera>();   
            camera.backgroundColor = Color.black;
            camera.clearFlags = CameraClearFlags.SolidColor;

            var formStack = new GameObject("FormStack"
                , typeof(Canvas)
                , typeof(UnityEngine.UI.GraphicRaycaster)
                , typeof(FUI.FormStack)
                , typeof(FUICanvasScaler)

            );
            var canvas = formStack.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;

            var eventSystem = new GameObject("EventSystem"
                , typeof(UnityEngine.EventSystems.EventSystem)
                , typeof(UnityEngine.EventSystems.StandaloneInputModule)
            );

            var app = new GameObject("App");
            app.transform.SetParent(formStack.transform, false);
            var appRectTransform = app.AddComponent<RectTransform>();
            //stretch

            appRectTransform.anchorMin = new Vector2(0, 0);
            appRectTransform.anchorMax = new Vector2(1, 1);
            appRectTransform.sizeDelta = new Vector2(0, 0);

            //select app
            Selection.activeGameObject = app;

            //save the scene to pathName
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, pathName);            
        }

        [MenuItem("Assets/Create/FUI/Form", false, 1)]
        public static void BeginCreateForm() {
            var icon = EditorGUIUtility.FindTexture(
                "cs Script Icon");
            BeginCreate(
                typeof(MenuCommands).FullName,
                nameof(CreateForm),
                "NewForm.cs",
                icon
            );            
        }


    /*
using UnityEngine;
using FUI;
using static FUI.Shortcuts;
using static FUI.Basic;
using FUI.Gears;
using FUI.Modifiers;
using System;
#nullable enable

public class ThemeTest : Form {

*/
        public static void CreateForm(int instanceId, string pathName, string resourceFile) {
            var className = Path.GetFileNameWithoutExtension(pathName);
            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            bool hasNamespace = !string.IsNullOrEmpty(rootNamespace);
            var namespaceIndent = hasNamespace ? "    " : "";
            Debug.Log($"Root namespace: {rootNamespace}");

            string code = $@"
using UnityEngine;
using FUI;
using static FUI.Shortcuts;
using static FUI.Basic;
using FUI.Gears;
using FUI.Modifiers;
#nullable enable

{(hasNamespace ? $"namespace {rootNamespace} {{" : "")}
{namespaceIndent}public class {className} : Form {{
{namespaceIndent}    protected override void Build() {{
{namespaceIndent}        using (WindowBackground()) {{
{namespaceIndent}            Padding(8);
{namespaceIndent}            Label(""Hello, World!"");
{namespaceIndent}        }}
{namespaceIndent}    }}
{namespaceIndent}}}
{(hasNamespace ? "}" : "")}
";
            System.IO.File.WriteAllText(pathName, code);
            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            Selection.activeObject = asset;

            
        }



        private static void BeginCreate(string callbackClassName,
            string callbackMethodName,
            string defaultName,
            Texture2D icon) {
            var callback = ScriptableObject.CreateInstance<EndNameEditActionCallStaticMethod>()
                .Init(callbackClassName, callbackMethodName);
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                callback,
                defaultName,
                icon,
                null
            );
        }
    }
}

#endif