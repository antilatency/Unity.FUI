using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Gears {
    public static class PointerEventUtils { 
        public delegate bool PointerEventFilter(GameObject target, PointerEventData eventData);
    }
}
