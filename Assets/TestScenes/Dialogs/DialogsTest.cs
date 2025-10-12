#nullable enable
using FUI;
using FUI.Gears;
using static FUI.Shortcuts;
using static FUI.Basic;

public class DialogsTest : Form {

    public InspectorButton _RebuildCS;
    public void RebuildCS() {
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }

    public string Value = "";

    protected override void Update() {
        base.Update();
    }

    protected override void Build() {


        using (WindowBackground()) {
            
            /*Button("Show Dialog", () => {
                StringEditDialog.Open("Edit Value", 300, Value, AssignValue);
                //FormStack.Instance?.Push<StringEditDialog>();
            });
            GapTop(4);*/

            var localVariable = Value;

            /*Button("Show Dialog Lambda", () => {
                StringEditDialog.Open("Edit Value", 300, Value, x => {
                    Value = x;
                    MakeDirty();
                });
                //FormStack.Instance?.Push<StringEditDialog>();
            });*/
            GapTop(4);
            Label($"Value: {Value}");
            
        }
    }

    void AssignValue(string newValue) {
        Value = newValue;
        MakeDirty();
    }


}