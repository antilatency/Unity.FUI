using System;
using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable
namespace FUI.Gears{
    public class PointerPressReleaseHandlerEx : MonoBehaviour, IPointerDownHandler, IPointerUpHandler{
        
        public Action<GameObject, PointerEventData>? OnPress;
        public Action<GameObject,PointerEventData?>? OnRelease;
        private bool _pressed;
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData){
            DoPressed(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData){
            DoReleased(eventData);
        }
        
        private void DoPressed(PointerEventData eventData){
            _pressed = true;
            OnPress?.Invoke(gameObject, eventData);
        }

        private void DoReleased(PointerEventData eventData){
            _pressed = false;
            OnRelease?.Invoke(gameObject, eventData);
        }

        private void OnApplicationFocus(bool hasFocus){
            if (!hasFocus && _pressed){
                DoReleased(null);
            }
        }
    }
}