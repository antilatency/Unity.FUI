using FUI.Gears;
using FUI.Modifiers;
using static FUI.Basic;
using static FUI.Shortcuts;
using UnityEngine;
using System;
#nullable enable

namespace FUI {
    public class MessageDialogYesNo : AbstractValueDialogGenericReturn<bool> {

        public string Message = "Are you sure?";
        public string YesText = "Yes";
        public string NoText = "No";
        protected override void Populate() {

            Padding(8);
            Label(Message);
            GapTop(8);
            using (Group(P.Up(Theme.LineHeight))) {
                Button(YesText, (g, e) => {
                    Return(true);
                    Close();
                }, P.Left(0, 0.5f));
                GapLeft(4);
                Button(NoText, (g, e) => {
                    Return(false);
                    Close();
                }, P.Fill);
            }
        }

        protected override Disposable<RectTransform> WindowElement() {
            return Group(P.Center(200)
            , new AddComponent<RoundedRectangle>()
            , new SetColor(Theme.PopupBackgroundColor)
            , new SetRectangleCorners(Theme.Radius)
            );
        }

        public void Configure(Action<bool> returnAction, string message, string yesText = "Yes", string noText = "No") {
            Message = message;
            YesText = yesText;
            NoText = noText;
            base.Configure(false, returnAction);
        }
    }


}
