using System;
namespace FUI.Gears {
    public class Disposable : IDisposable {

        private readonly Action disposeAction;

        public Disposable(Action disposeAction) {
            this.disposeAction = disposeAction;
        }
        public void Dispose() {
            disposeAction();
        }



        public static Disposable<T> Create<T>(T value, Action<T> disposeAction) {
            return new Disposable<T>(value, disposeAction);
        }
        /*public static Disposable Create(Action disposeAction) {
            return new Disposable(disposeAction);
        }*/
    }

    public class Disposable<T> : IDisposable {
        public readonly T Value;
        private readonly Action<T> disposeAction;

        public Disposable(T value, Action<T> disposeAction) {
            Value = value;
            this.disposeAction = disposeAction;
        }

        public void Dispose() {
            disposeAction(Value);
        }


    }
}