namespace HaroLibs {
    public class IntValueComparer : NumericalValueComparer<int> { 

        public void Add( int val ) => origin.Value += val;

    }
}
