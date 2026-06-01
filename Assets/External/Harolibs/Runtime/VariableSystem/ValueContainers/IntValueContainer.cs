using UnityEngine;

namespace HaroLibs {
    [CreateAssetMenu( fileName = nameof( IntValueContainer ), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof( IntValueContainer ) )]
    public class IntValueContainer : ValueContainerBase<int> { }
}
