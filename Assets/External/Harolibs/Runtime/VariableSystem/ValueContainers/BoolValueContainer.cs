using UnityEngine;

namespace HaroLibs {
    [CreateAssetMenu( fileName = nameof( BoolValueContainer ), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof( BoolValueContainer ) )]
    public class BoolValueContainer : ValueContainerBase<bool> { 
        
        public void Toggle() => Value = !Value;

    }
}
