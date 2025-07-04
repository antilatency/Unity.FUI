using System;

using UnityEngine;
using UnityEngine.EventSystems;


namespace FUI.Gears {
    public class PointerEventReceiver : MonoBehaviour,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerMoveHandler,
        IDragHandler,
        IBeginDragHandler,
        IEndDragHandler {

        public Func<GameObject, PointerEventData, bool> Handler;

        public void HandleEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> callbackFunction) where T : IEventSystemHandler {
            var used = Handler?.Invoke(gameObject, eventData) ?? false;
            if (!used) {
                // If the handler did not use the event, we pass it to the parent
                var parent = gameObject.transform.parent;
                if (parent != null) {
                    ExecuteEvents.ExecuteHierarchy<T>(
                        parent.gameObject,
                        eventData,
                        callbackFunction
                    );
                }
            }
        }

        public static Vector2 GetPointerLocalPosition(GameObject gameObject, PointerEventData eventData) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)gameObject.transform,
                eventData.position,
                eventData.enterEventCamera ?? eventData.pressEventCamera,
                out var localPoint
            );
            return localPoint;
        }

        public static Vector2 GetPointerUV(GameObject gameObject, PointerEventData eventData) {
            var localPoint = GetPointerLocalPosition(gameObject, eventData);
            var rectTransform = (RectTransform)gameObject.transform;
            var uv = new Vector2(
                (localPoint.x - rectTransform.rect.x) / rectTransform.rect.width,
                (localPoint.y - rectTransform.rect.y) / rectTransform.rect.height
            );
            return uv;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            HandleEvent(eventData, ExecuteEvents.pointerClickHandler);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            HandleEvent(eventData, ExecuteEvents.pointerDownHandler);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            HandleEvent(eventData, ExecuteEvents.pointerUpHandler);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            HandleEvent(eventData, ExecuteEvents.pointerEnterHandler);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            HandleEvent(eventData, ExecuteEvents.pointerExitHandler);
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData) {
            HandleEvent(eventData, ExecuteEvents.pointerMoveHandler);
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            HandleEvent(eventData, ExecuteEvents.dragHandler);
        }
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            HandleEvent(eventData, ExecuteEvents.beginDragHandler);
        }
        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            HandleEvent(eventData, ExecuteEvents.endDragHandler);
        }
    }

}
