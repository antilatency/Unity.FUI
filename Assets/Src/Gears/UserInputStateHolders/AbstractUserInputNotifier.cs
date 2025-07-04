using UnityEngine;
namespace FUI.Gears {
    public abstract class AbstractUserInputNotifier : MonoBehaviour {
        
        [SerializeField]
        private Form? FormToNotify;
        public void SetFormToNotify(Form? form) {
            FormToNotify = form;
        }

        public void NotifyForm() {
            FormToNotify?.MakeDirty(ExtraIteration);
        }

        public bool ExtraIteration = false;


    }
}