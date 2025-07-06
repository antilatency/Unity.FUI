using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable

namespace FUI.Gears {
    
    public interface IFormNotifier {
        void SetFormToNotify(Form? form, bool extraIteration);
    }

    public abstract class AbstractFormNotifier : MonoBehaviour, IFormNotifier {
        [HideInInspector]
        [SerializeField]
        protected Form? FormToNotify;
        [HideInInspector]
        [SerializeField]
        protected bool ExtraIteration = false;

        public void SetFormToNotify(Form? form, bool extraIteration) {
            FormToNotify = form;
            ExtraIteration = extraIteration;
        }

        protected void NotifyForm() {
            FormToNotify?.MakeDirty(ExtraIteration);
        }
        
    }
}
