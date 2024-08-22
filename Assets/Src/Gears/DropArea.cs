using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FUI.Gears {
    public class DropArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler {

        public Graphic ColorGraphic;
        public Graphic CatchGraphic;
        public Color NormalColor = Color.white;
        public Color ReadyToCatchColor = Color.green;
        public Color HoverColor = Color.blue;

        public Func<object, bool> AcceptPredicate = null;
        public Action<object> DropAction = null;

        private bool hovered = false;

        protected virtual void Update() {


            if (Draggable.CurrentlyDragging != null && AcceptPredicate(Draggable.CurrentlyDragging)) {
                CatchGraphic.enabled = true;
                if (hovered)
                    ColorGraphic.color = HoverColor;
                else
                    ColorGraphic.color = ReadyToCatchColor;

            } else {
                CatchGraphic.enabled = false;
                ColorGraphic.color = NormalColor;
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            hovered = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            hovered = false;
        }

        public void OnDrop(PointerEventData eventData) {
            if (AcceptPredicate(Draggable.CurrentlyDragging)) {
                DropAction(Draggable.CurrentlyDragging);
            }

        }
    }
}
