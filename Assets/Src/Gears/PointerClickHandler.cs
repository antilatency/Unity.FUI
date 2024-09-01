using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Gears {
    public class PointerClickHandler : MonoBehaviour, IPointerClickHandler {

        public Action OnClick = null;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {            
            OnClick?.Invoke();
        }
    }
}
