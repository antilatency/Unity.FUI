using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Gears {
    public class Slider : UserInputState<Vector2>, IDragHandler, /*IBeginDragHandler,*/ IEndDragHandler, IPointerDownHandler/*, IPointerUpHandler*/ {
        public bool Moving { get; private set; } = false;

        private Vector2 GetLocalMousePosition(PointerEventData eventData) {

            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out var localPoint);

            return localPoint;
        }

        /*void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            Moving = true;
        }*/

        void IDragHandler.OnDrag(PointerEventData eventData) {
            SetValueFromEventData(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            Moving = false;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            eventData.useDragThreshold = false;
            Moving = true;
            SetValueFromEventData(eventData);
        }

        /*void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            Moving = false;
        }*/

        void SetValueFromEventData(PointerEventData eventData) {
            if (Moving) {
                var localPosition = GetLocalMousePosition(eventData);
                var size = ((RectTransform)transform).rect.size;
                var normalizedPosition = new Vector2(
                    size.x > 0 ? localPosition.x / size.x : 0,
                    size.y > 0 ? localPosition.y / size.y : 0)

                    + ((RectTransform)transform).pivot;

                UserInput(normalizedPosition);

            }
        }


    }


}