using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Gears {
    public class PointerClickHandlerEx : MonoBehaviour, IPointerClickHandler {

        public Action<GameObject, PointerEventData> OnClick = null;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            OnClick?.Invoke(gameObject, eventData);
        }
    }
}