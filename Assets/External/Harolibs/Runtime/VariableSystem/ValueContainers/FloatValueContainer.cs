using UnityEngine;

namespace HaroLibs {
    [CreateAssetMenu( fileName = nameof( FloatValueContainer ), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof( FloatValueContainer ) )]
    public class FloatValueContainer : ValueContainerBase<float> {

        public void Add( float val ) => Value += val;
        public void Add( int val ) => Value += val;
        public void Add( ValueContainerBase<float> val ) => Value += val;
        public void Add( ValueContainerBase<int> val ) => Value += val;

        public void SetValue( int val ) => Value = val;
        public void SetValue( ValueContainerBase<int> val ) => Value = val;


    }
}
