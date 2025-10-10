using System;

using UnityEngine;

using static FUI.Shortcuts;

/*namespace FUI {
    public class StringEditDialog : Dialog<string> {

        public string Header = "Edit Value";
    

        public static void Open(string header, float width, string value, Action<string> returnAction) {
            var form = FormStack.Instance.Push<StringEditDialog>();
            form.Header = header;
            form.Value = value;
            form.SetReturn(new SerializableAction<string>(returnAction));
            (form.transform as RectTransform)!.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        protected override void Build() {

            using (WindowBackground(P.RigidFill)) {
                Label(Header);
                LabeledInputField("Edit Value", Value ?? "", x=> { Value = x; MakeDirty(); });
                Button("Ok", () => {
                    Return(Value ?? "");
                    Close();
                });
                Button("Cancel", () => {
                    Close();
                });
            }
            ShrinkContainer(false, true);
        }
    }
}*/
