using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
namespace FUI.Gears {
    public class HoveredState : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler {

        public virtual bool Hovered { get; set; }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            Hovered = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            Hovered = false;
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData) {
            Hovered = !Enumerable.Range(0, 3).Any(x => Input.GetMouseButton(x)) && eventData.hovered.Contains(this.gameObject);
        }

    }
}