using UnityEngine;
using FUI;
using FUI.Gears;
using static FUI.Shortcuts;
using System;
#nullable enable
/*
[Serializable]
public struct LinearPosition {
    public Vector2 B;
    public Vector2 K;    

    public LinearPosition(Vector2 b, Vector2 k) {
        K = k;
        B = b;
    }

    public static LinearPosition operator +(LinearPosition a, LinearPosition b) {

        return new LinearPosition(a.B + a.K * b.B, a.K * b.K);
    }

    public Vector2 Get(Vector2 windowSize) {
        return new Vector2(K.x * windowSize.x + B.x, K.y * windowSize.y + B.y);
    }

    public static LinearPosition MinFromRectTransform(RectTransform transform) {


        var current = new LinearPosition(transform.offsetMin, transform.anchorMin);
        if (transform.parent == null || !(transform.parent is RectTransform parent))
            return current;

        var parentLinear = MinFromRectTransform(parent);

        return current + parentLinear;
    }
}*/



[ExecuteAlways]
public class LinearPositionTest : MonoBehaviour {


    public RectTransform rectTransform;

    public RectTransform PositionDisplay;

    //public LinearPosition linearPosition;
    void Update() {
        /*if (rectTransform != null) {
            linearPosition = LinearPosition.MinFromRectTransform(rectTransform);
        }
        var canvas = GetComponent<Canvas>();
        var canvasSize = canvas.GetComponent<RectTransform>().rect.size;
        var canvasLinearPosition = LinearPosition.MinFromRectTransform(canvas.GetComponent<RectTransform>());
        Debug.Log($"Canvas K: {canvasLinearPosition.K} B: {canvasLinearPosition.B}");

        PositionDisplay.anchoredPosition = linearPosition.Get(canvasSize);*/
    }
}