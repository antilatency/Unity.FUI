using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

#nullable enable


namespace FUI {
    public class FormStack : MonoBehaviour {
        public static FormStack? _instance;
        public static FormStack Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<FormStack>();
                }
                if (_instance == null) {
                    throw new Exception("No FormStack instance found in scene");
                }
                return _instance;
            }
        }



        public Form? Top {
            get {
                if (transform.childCount == 0) return null;
                Transform top = transform.GetChild(transform.childCount - 1);
                return top.GetComponent<Form>();
            }
        }

        static CanvasGroup GetOrCreateCanvasGroup(GameObject gameObject) {
            var canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            return canvasGroup;
        }
        public T Push<T>() where T : Form {        
            var form = (T)Push(typeof(T));
            return form;
        }

        public object Push(Type formType) {
            var top = Top;
            if (top != null) {
                top.enabled = false;
                var canvasGroup = GetOrCreateCanvasGroup(top.gameObject);
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            GameObject gameobject = new GameObject(formType.Name);
            gameobject.transform.SetParent(this.transform, false);
            object form = gameobject.AddComponent(formType);
            //stratch
            var rectTransform = gameobject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);

            return form;
        }
         

        public void Pop() {
            var top = Top;
            if (top != null) {
                DestroyImmediate(top.gameObject);
            }
            top = Top;
            if (top != null) {
                top.enabled = true;
                var canvasGroup = GetOrCreateCanvasGroup(top.gameObject);
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    } 
}
