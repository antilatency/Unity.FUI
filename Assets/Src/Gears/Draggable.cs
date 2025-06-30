using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;


namespace FUI.Gears {

    public class Draggable : MonoBehaviour, IDragHandler, IInitializePotentialDragHandler {


        public InputButtonMask AllowedButtons = InputButtonMask.All;

        public Action<GameObject, PointerEventData> DragAction;

        public void OnInitializePotentialDrag(PointerEventData eventData) {
            eventData.useDragThreshold = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            HandleDragWithAllowedButtons(
                gameObject,
                eventData,
                AllowedButtons,
                DragAction
            );
        }
        
        public static void HandleDragWithAllowedButtons(
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
        }

    }

}