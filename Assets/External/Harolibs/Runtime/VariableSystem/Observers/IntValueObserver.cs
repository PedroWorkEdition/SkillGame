namespace HaroLibs {
    public class IntValueObserver : ValueObserverBase<int> {

        public void Add( int val ) => targetValue.Value += val;
        public void Add( ValueContainerBase<int> val ) => targetValue.Value += val;

    }

}
