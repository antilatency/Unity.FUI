using System;

#nullable enable
namespace FUI {

    public static class ValueFormExtensions { 
        public static ValueForm<T> Execute<F,T>(this F form, T value, Action<T> returnAction) where T : IEquatable<T> where F : ValueForm<T> {
            form._returnAction = returnAction;
            if (!form.initialized || !form.Value.Equals(value) || form.Dirty) {
                form.Value = value;
                form.initialized = true;
                form.Rebuild();
            }
            return form;
        }
    }

    public abstract class ValueForm<T> : Form where T : IEquatable<T> {
        public bool initialized = false;
        public T Value;
        public Action<T> _returnAction = null!;

        protected void AssignAndReturn<F>(ref F field, F fieldValue) {
            var oldValue = Value;
            field = fieldValue;
            if (!oldValue.Equals(Value)) {
                MakeDirty();
                _returnAction(Value);
            }
        }
    }

}
