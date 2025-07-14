using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;


namespace FUI.Gears {

    public class Draggable : MonoBehaviour, IDragHandler, IInitializePotentialDragHandler {


        public PointerEventUtils.PointerEventFilter EventFilter = null;

        public Action<GameObject, PointerEventData> DragAction;

        public void OnInitializePotentialDrag(PointerEventData eventData) {
            eventData.useDragThreshold = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if (EventFilter != null && !EventFilter(gameObject, eventData)) {
                var parent = gameObject.transform.parent;
                ExecuteEvents.ExecuteHierarchy(
                    parent.gameObject,
                    eventData,
                    ExecuteEvents.dragHandler
                );
                return;
            }
            DragAction(gameObject, eventData);
        }

        /*public static void HandleDragWithAllowedButtons(
            GameObject gameObject,
            PointerEventData eventData,
            InputButtonMask allowedButtons,
            Action<GameObject, PointerEventData> dragAction
        ) {
            if (((int)allowedButtons & (1 << (int)eventData.button)) != 0) {
                dragAction?.Invoke(gameObject, eventData);
            }
            else {
                var parent = gameObject.transform.parent;
                ExecuteEvents.ExecuteHierarchy(
                    parent.gameObject,
                    eventData,
                    ExecuteEvents.dragHandler
                );
            }
        }*/

    }

}