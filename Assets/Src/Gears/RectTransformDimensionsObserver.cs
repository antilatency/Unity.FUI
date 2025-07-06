using System;

using UnityEngine;
using UnityEngine.EventSystems;
using FUI.Gears;

namespace FUI.Gears {
    public class RectTransformDimensionsObserver : UIBehaviour {

        public Action<RectTransform> Handler;

        protected override void OnRectTransformDimensionsChange() {
            Handler?.Invoke((RectTransform)transform);
        }
    } 
}

namespace FUI {
    public partial class M {
        public static Modifier AddRectTransformDimensionsObserver(Action<RectTransform> handler) =>
            new(
                "AddRectTransformDimensionsObserver",
                x => x.AddComponent<RectTransformDimensionsObserver>(),
                x => {
                    var receiver = x.GetComponent<RectTransformDimensionsObserver>();
                    receiver.Handler = handler;
                }
            );
    }
}