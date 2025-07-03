using UnityEngine;

[ExecuteAlways]
public class CreateTestSceneAgent : MonoBehaviour {
    public string scriptFullName;

    public void Update() {
        // This is just a placeholder to ensure the script is compiled and can be used in the scene.
        // Find the script by its full name and add it to the GameObject.
        if (string.IsNullOrEmpty(scriptFullName)) {
            Debug.LogError("Script full name is not set.");
            return;
        }

        var scriptType = System.Type.GetType(scriptFullName);
        if (scriptType == null) {
            //Debug.Log($"Waiting for script type '{scriptFullName}' to be available.");
            return;
        }

        if (!typeof(MonoBehaviour).IsAssignableFrom(scriptType)) {
            Debug.LogError($"The script type '{scriptFullName}' is not a MonoBehaviour.");
        }
        else { 
            gameObject.AddComponent(scriptType);
        }
        DestroyImmediate(this);
    }

}
