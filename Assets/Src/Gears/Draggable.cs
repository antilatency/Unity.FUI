using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;


namespace FUI.Gears {
    public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IInitializePotentialDragHandler {


        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int X;
            public int Y;

            public POINT(int x, int y) {
                this.X = x;
                this.Y = y;
            }
        }


        public Action<GameObject, PointerEventData> DragAction;

        public void OnInitializePotentialDrag(PointerEventData eventData) {
            eventData.useDragThreshold = false;
        }



        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            //Cursor.lockState = CursorLockMode.Locked;
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            DragAction?.Invoke(gameObject,eventData);
            //SetCursorPos(0, 0);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            //Cursor.lockState = CursorLockMode.None;
        }

    }

}