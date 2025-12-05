using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable
namespace FUI.Gears {
    public static class PointerEventUtils {

        // A filter that returns true if the event should be handled by the current target,
        // or false to pass it to the parent.
        public delegate bool PointerEventFilter(GameObject target, PointerEventData eventData);

        // Returns true if the event should be handled by the current target,
        // or false if it WAS passed to the parent.
        public static bool HandleDragFilter(GameObject gameObject, PointerEventData eventData, PointerEventFilter? filter) {
            if (filter != null && !filter(gameObject, eventData)) {
                var parent = gameObject.transform.parent;
                ExecuteEvents.ExecuteHierarchy(
                    parent.gameObject,
                    eventData,
                    ExecuteEvents.dragHandler
                );
                return false;
            }
            return true;
        }

        public static bool HandleScrollFilter(GameObject gameObject, PointerEventData eventData, PointerEventFilter? filter) {
            if (filter != null && !filter(gameObject, eventData)) {
                var parent = gameObject.transform.parent;
                ExecuteEvents.ExecuteHierarchy(
                    parent.gameObject,
                    eventData,
                    ExecuteEvents.scrollHandler
                );
                return false;
            }
            return true;
        }


    }
}
