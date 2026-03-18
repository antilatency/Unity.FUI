#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

public static class MissingScript {
    private static readonly Regex ScriptGuidRegex = new Regex(
        @"m_Script:\s*\{fileID:\s*11500000,\s*guid:\s*([0-9a-fA-F]{32}),\s*type:\s*3\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );
    private static readonly HashSet<string> WatchReportedFindings = new HashSet<string>(StringComparer.Ordinal);
    private static bool _watchRunning;
    private static double _nextWatchScanTime;

    [MenuItem("Tools/MissingScript")]
    public static void Execute() {
        var report = new List<string>();

        ScanCurrentSelection(report);
        ScanOpenScenes(report);
        ScanPrefabs(report);
        ScanLoadedHiddenAndPreviewObjects(report);
        ScanLoadedUserMonoBehavioursWithoutMonoScript(report, false);
        ScanSerializedScriptReferences(report);

        if (report.Count == 0) {
            report.Add("No missing scripts or suspicious serialized script references found.");
        }

        Debug.Log(string.Join("\n", report));
    }

    [MenuItem("Tools/MissingScript Watch/Start")]
    public static void StartWatch() {
        if (_watchRunning) {
            return;
        }

        WatchReportedFindings.Clear();
        _watchRunning = true;
        _nextWatchScanTime = 0;
        EditorApplication.update += WatchUpdate;
        Debug.Log("MissingScript watch started.");
    }

    [MenuItem("Tools/MissingScript Watch/Start", true)]
    private static bool ValidateStartWatch() => !_watchRunning;

    [MenuItem("Tools/MissingScript Watch/Stop")]
    public static void StopWatch() {
        if (!_watchRunning) {
            return;
        }

        EditorApplication.update -= WatchUpdate;
        _watchRunning = false;
        Debug.Log("MissingScript watch stopped.");
    }

    [MenuItem("Tools/MissingScript Watch/Stop", true)]
    private static bool ValidateStopWatch() => _watchRunning;

    private static void WatchUpdate() {
        if (EditorApplication.timeSinceStartup < _nextWatchScanTime) {
            return;
        }

        _nextWatchScanTime = EditorApplication.timeSinceStartup + 0.5d;

        var findings = new List<string>();
        ScanLoadedUserMonoBehavioursWithoutMonoScript(findings, true);

        if (findings.Count == 0) {
            return;
        }

        Debug.Log("[MissingScript Watch]\n" + string.Join("\n", findings));
    }

    private static void ScanOpenScenes(List<string> report) {
        var findings = new List<string>();

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++) {
            var scene = SceneManager.GetSceneAt(sceneIndex);
            if (!scene.isLoaded) {
                continue;
            }

            var sceneLabel = string.IsNullOrEmpty(scene.path)
                ? $"<unsaved scene:{scene.name}>"
                : scene.path.Replace('\\', '/');

            foreach (var root in scene.GetRootGameObjects()) {
                ScanGameObjectForMissingComponents(root, $"scene:{sceneLabel}", findings);
            }
        }
        AppendSection(report, "Open Scenes", findings);
    }

    private static void ScanCurrentSelection(List<string> report) {
        var findings = new List<string>();
        var selectedObject = Selection.activeObject;

        if (selectedObject == null) {
            findings.Add("selection :: none");
            AppendSection(report, "Current Selection", findings);
            return;
        }

        var assetPath = AssetDatabase.GetAssetPath(selectedObject).Replace('\\', '/');
        findings.Add($"selection :: object:{selectedObject.name} :: type:{selectedObject.GetType().FullName} :: path:{(string.IsNullOrEmpty(assetPath) ? "<none>" : assetPath)} :: persistent:{EditorUtility.IsPersistent(selectedObject)} :: hideFlags:{selectedObject.hideFlags}");

        if (Selection.activeGameObject != null) {
            ScanGameObjectForMissingComponents(Selection.activeGameObject, "selection:activeGameObject", findings);
        }

        AppendSection(report, "Current Selection", findings);
    }

    private static void ScanPrefabs(List<string> report) {
        var findings = new List<string>();

        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" })) {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid).Replace('\\', '/');
            var root = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (root == null) {
                findings.Add($"prefab:{assetPath} :: failed to load prefab root");
                continue;
            }

            ScanGameObjectForMissingComponents(root, $"prefab:{assetPath}", findings);
        }

        AppendSection(report, "Prefabs", findings);
    }

    private static void ScanLoadedHiddenAndPreviewObjects(List<string> report) {
        var findings = new List<string>();
        var seenRoots = new HashSet<int>();

        foreach (var gameObject in Resources.FindObjectsOfTypeAll<GameObject>()) {
            if (gameObject == null) {
                continue;
            }

            if (gameObject.transform.parent != null) {
                continue;
            }

            if (!seenRoots.Add(gameObject.GetInstanceID())) {
                continue;
            }

            if (!IsHiddenOrPreviewObject(gameObject)) {
                continue;
            }

            ScanGameObjectForMissingComponents(gameObject, DescribeLoadedObjectOwner(gameObject), findings);
        }

        AppendSection(report, "Loaded Hidden Or Preview Objects", findings);
    }

    private static void ScanLoadedUserMonoBehavioursWithoutMonoScript(List<string> report, bool onlyNew) {
        var findings = new List<string>();

        foreach (var behaviour in Resources.FindObjectsOfTypeAll<MonoBehaviour>()) {
            if (behaviour == null) {
                continue;
            }

            var type = behaviour.GetType();
            if (!IsLikelyUserAssembly(type)) {
                continue;
            }

            var scriptAsset = MonoScript.FromMonoBehaviour(behaviour);
            var scriptPath = scriptAsset == null ? string.Empty : AssetDatabase.GetAssetPath(scriptAsset).Replace('\\', '/');
            var scriptClass = scriptAsset?.GetClass();

            string? issue = null;
            if (scriptAsset == null) {
                issue = "MonoScript asset is null";
            }
            else if (string.IsNullOrEmpty(scriptPath)) {
                issue = "MonoScript asset has no project path";
            }
            else if (scriptClass == null) {
                issue = "MonoScript.GetClass() returned null";
            }
            else if (scriptClass != type) {
                issue = $"MonoScript type mismatch :: script:{scriptClass.FullName}";
            }

            if (issue == null) {
                continue;
            }

            var finding = DescribeSuspiciousBehaviour(behaviour, type, scriptPath, issue);
            if (onlyNew && !WatchReportedFindings.Add(finding)) {
                continue;
            }

            findings.Add(finding);
        }

        AppendSection(report, "Loaded User MonoBehaviours Without MonoScript", findings);
    }

    private static void ScanGameObjectForMissingComponents(GameObject gameObject, string owner, List<string> findings) {
        var missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);
        var components = gameObject.GetComponents<Component>();
        List<int>? missingSlots = null;

        for (int i = 0; i < components.Length; i++) {
            if (components[i] != null) {
                continue;
            }

            if (missingSlots == null) {
                missingSlots = new List<int>();
            }

            missingSlots.Add(i);
        }

        if (missingScriptCount > 0 || missingSlots != null) {
            var slotText = missingSlots == null ? "[]" : $"[{string.Join(", ", missingSlots)}]";
            findings.Add($"{owner} :: {GetHierarchyPath(gameObject.transform)} :: missing script count {missingScriptCount} :: null component slots {slotText}");
        }

        foreach (Transform child in gameObject.transform) {
            ScanGameObjectForMissingComponents(child.gameObject, owner, findings);
        }
    }

    private static void ScanSerializedScriptReferences(List<string> report) {
        var findings = new List<string>();
        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        if (string.IsNullOrEmpty(projectRoot)) {
            findings.Add("project :: failed to determine project root");
            AppendSection(report, "Serialized Script References", findings);
            return;
        }

        foreach (var assetPath in EnumerateAssetPaths("t:Scene", "t:Prefab")) {
            var fullPath = Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(fullPath)) {
                findings.Add($"{assetPath} :: file does not exist on disk");
                continue;
            }

            string text;
            try {
                text = File.ReadAllText(fullPath);
            }
            catch (Exception exception) {
                findings.Add($"{assetPath} :: failed to read file :: {exception.Message}");
                continue;
            }

            var seenGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var matches = ScriptGuidRegex.Matches(text);
            foreach (Match match in matches) {
                var scriptGuid = match.Groups[1].Value;
                if (!seenGuids.Add(scriptGuid)) {
                    continue;
                }

                var scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuid).Replace('\\', '/');
                if (string.IsNullOrEmpty(scriptPath)) {
                    findings.Add($"{assetPath} :: missing script guid {scriptGuid}");
                    continue;
                }

                var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
                if (scriptAsset == null) {
                    findings.Add($"{assetPath} :: guid {scriptGuid} resolves to non-MonoScript asset :: {scriptPath}");
                    continue;
                }

                var type = scriptAsset.GetClass();
                if (type == null) {
                    findings.Add($"{assetPath} :: script type could not be loaded :: {scriptPath}");
                    continue;
                }

                if (!typeof(MonoBehaviour).IsAssignableFrom(type) && !typeof(ScriptableObject).IsAssignableFrom(type)) {
                    continue;
                }

                if (!IsSerializedTypePublic(type)) {
                    findings.Add($"{assetPath} :: non-public serialized type :: {type.FullName} :: {scriptPath}");
                }
            }
        }

        AppendSection(report, "Serialized Script References", findings);
    }

    private static IEnumerable<string> EnumerateAssetPaths(params string[] filters) {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var filter in filters) {
            foreach (var guid in AssetDatabase.FindAssets(filter, new[] { "Assets" })) {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid).Replace('\\', '/');
                if (seen.Add(assetPath)) {
                    yield return assetPath;
                }
            }
        }
    }

    private static bool IsLikelyUserAssembly(Type type) {
        var assemblyName = type.Assembly.GetName().Name ?? string.Empty;
        if (assemblyName.StartsWith("Unity", StringComparison.Ordinal)) {
            return false;
        }
        if (assemblyName.StartsWith("System", StringComparison.Ordinal)) {
            return false;
        }
        if (assemblyName.StartsWith("mscorlib", StringComparison.Ordinal)) {
            return false;
        }
        if (assemblyName.StartsWith("Mono.", StringComparison.Ordinal)) {
            return false;
        }
        if (assemblyName.StartsWith("netstandard", StringComparison.Ordinal)) {
            return false;
        }

        return true;
    }

    private static string DescribeSuspiciousBehaviour(MonoBehaviour behaviour, Type type, string scriptPath, string issue) {
        var gameObject = behaviour.gameObject;
        var owner = gameObject == null
            ? $"object:{behaviour.name}"
            : $"{DescribeLoadedObjectOwner(gameObject)} :: {GetHierarchyPath(gameObject.transform)}";

        var scriptPathText = string.IsNullOrEmpty(scriptPath) ? "<none>" : scriptPath;
        var assemblyName = type.Assembly.GetName().Name ?? string.Empty;

        return $"{owner} :: component:{type.FullName} :: assembly:{assemblyName} :: script:{scriptPathText} :: instanceID:{behaviour.GetInstanceID()} :: issue:{issue}";
    }

    private static bool IsSerializedTypePublic(Type type) {
        return type.IsPublic || type.IsNestedPublic;
    }

    private static bool IsHiddenOrPreviewObject(GameObject gameObject) {
        if (EditorUtility.IsPersistent(gameObject)) {
            return false;
        }

        if (gameObject.hideFlags != HideFlags.None) {
            return true;
        }

        var scene = gameObject.scene;
        if (!scene.IsValid()) {
            return true;
        }

        if (!scene.isLoaded) {
            return true;
        }

        if (string.IsNullOrEmpty(scene.path)) {
            return true;
        }

        return scene.name.IndexOf("Preview", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static string DescribeLoadedObjectOwner(GameObject gameObject) {
        var scene = gameObject.scene;
        var sceneLabel = !scene.IsValid()
            ? "<no-scene>"
            : string.IsNullOrEmpty(scene.path)
                ? $"<{scene.name}>"
                : scene.path.Replace('\\', '/');

        return $"loaded:{sceneLabel} :: hideFlags:{gameObject.hideFlags}";
    }

    private static string GetHierarchyPath(Transform transform) {
        var parts = new List<string>();
        Transform current = transform;

        while (current != null) {
            parts.Add(current.name);
            current = current.parent;
        }

        parts.Reverse();
        return string.Join("/", parts);
    }

    private static void AppendSection(List<string> report, string title, List<string> findings) {
        if (findings.Count == 0) {
            return;
        }

        report.Add($"[{title}]");
        report.AddRange(findings);
    }
}


#endif