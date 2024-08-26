namespace FUI.Gears {

    public abstract class AbstractUserInput<T> : AbstractUserInputNotifier {
        public bool NewUserInput { get; protected set; }
        public abstract T Value { get; set; }

        
    }
}