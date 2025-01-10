using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class CursorExperiment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public Sprite Sprite;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        var pixelPosition = hotSpot * new Vector2(cursorTexture.width, cursorTexture.height);

        Cursor.SetCursor(cursorTexture, pixelPosition, cursorMode);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
