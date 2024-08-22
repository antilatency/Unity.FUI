using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractHighlighter : MonoBehaviour {

    protected Color color {
        set {
            Graphic.color = value;
        }
    }

    public Graphic Graphic;

    protected virtual void OnEnable() {
        CheckComponent(ref Graphic);
    }

    protected void CheckComponent<T>(ref T component) where T: Component{
        if (component) return;
        component = GetComponent<T>();
        if (component) return;
        component = gameObject.AddComponent<T>();
    }

}