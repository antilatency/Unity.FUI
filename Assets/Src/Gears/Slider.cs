using System;

using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable
namespace FUI {
    namespace Modifiers {
        public class AddSlider : Modifier {
            Action<Vector2>? _returnAction;
            public AddSlider(Action<Vector2>? returnAction = null) {
                _returnAction = returnAction;
            }
            private void SetReturnAction(Gears.Slider slider) {
                slider.ReturnAction = _returnAction;
            }
            public override void Create(GameObject gameObject) {
                var slider = gameObject.AddComponent<Gears.Slider>();
                SetReturnAction(slider);
            }
            public override void Update(GameObject gameObject) {
                var slider = gameObject.GetComponent<Gears.Slider>();
                SetReturnAction(slider);
            }
        }
    }

    namespace Gears {
        public class Slider : MonoBehaviour, IDragHandler, /*IBeginDragHandler,*/ IEndDragHandler, IPointerDownHandler/*, IPointerUpHandler*/ {

            public Vector2 NormalizedValue;
            public bool Moving { get; private set; } = false;
            public Action<Vector2>? ReturnAction = null;

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
                    var newNormalizedValue = new Vector2(
                        size.x > 0 ? localPosition.x / size.x : 0,
                        size.y > 0 ? localPosition.y / size.y : 0)

                        + ((RectTransform)transform).pivot;

                    newNormalizedValue.x = Mathf.Clamp01(newNormalizedValue.x);
                    newNormalizedValue.y = Mathf.Clamp01(newNormalizedValue.y);
                    if (newNormalizedValue != NormalizedValue) {
                        NormalizedValue = newNormalizedValue;
                        ReturnAction?.Invoke(NormalizedValue);
                    }

                    //throw new System.NotImplementedException("returnAction not implemented");

                    //UserInput(normalizedPosition);

                }
            }


        }

    }
}