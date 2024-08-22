using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Gears {
    public class PressedState : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        public virtual bool Pressed { get; set; }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            Pressed = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            Pressed = false;
        }

    }
}