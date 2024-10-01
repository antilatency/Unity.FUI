using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace FUI.Gears {
    public class DraggableObject : MonoBehaviour, /*IDragHandler,*/ IBeginDragHandler, IEndDragHandler {

        public static object CurrentlyDragging = null;

        public Func<object> DragFunc;

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            CurrentlyDragging = DragFunc?.Invoke();
        }

        /*void IDragHandler.OnDrag(PointerEventData eventData) {
            //throw new NotImplementedException();
        }*/

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            CurrentlyDragging = null;
        }

    }

}