using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Gears
{
    public class FormNotifier : UIBehaviour
    {
        [SerializeField]
        private Form? FormToNotify;
        public void SetFormToNotify(Form? form)
        {
            FormToNotify = form;
        }

        public void NotifyForm()
        {
            FormToNotify?.MakeDirty(ExtraIterations);
        }

        public int ExtraIterations = 0;
    }
}
