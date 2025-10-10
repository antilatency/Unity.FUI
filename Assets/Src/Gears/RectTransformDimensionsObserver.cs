using System;

using UnityEngine;
using UnityEngine.EventSystems;
using FUI.Gears;
namespace FUI {
    
    namespace Gears {
        public class RectTransformDimensionsObserver : UIBehaviour {
            public Action<RectTransform> Handler;
            protected override void OnRectTransformDimensionsChange() {
                Handler?.Invoke((RectTransform)transform);
            }
        }
    }

    namespace Modifiers {
        public class AddRectTransformDimensionsObserver : Modifier {
            Action<RectTransform> _handler;
            public AddRectTransformDimensionsObserver(Action<RectTransform> handler) {
                _handler = handler;
            }
            public override void Create(GameObject gameObject) {
                var observer = gameObject.AddComponent<RectTransformDimensionsObserver>();
                observer.Handler = _handler;
            }
            public override void Update(GameObject gameObject) {
                var observer = gameObject.GetComponent<RectTransformDimensionsObserver>();
                observer.Handler = _handler;
            }
        }
    }



    /*public partial class M {
            public static Modifier AddRectTransformDimensionsObserver(Action<RectTransform> handler) =>
                new(
                    "AddRectTransformDimensionsObserver",
                    x => x.AddComponent<RectTransformDimensionsObserver>(),
                    x => {
                        var receiver = x.GetComponent<RectTransformDimensionsObserver>();
                        receiver.Handler = handler;
                    }
                );
        }*/
}