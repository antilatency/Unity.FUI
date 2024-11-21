using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Gears{
    public class PointerPressReleaseHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        public Action OnPress = null;
        public Action OnRelease = null;
        private bool _pressed;
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData){
            DoPressed();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData){
            DoReleased();
        }


        private void DoPressed(){
            _pressed = true;
            OnPress?.Invoke();
        }

        private void DoReleased(){
            _pressed = false;
            OnRelease?.Invoke();
        }

        private void OnApplicationFocus(bool hasFocus){
            if (!hasFocus && _pressed){
                DoReleased();
            }
        }
    }
}