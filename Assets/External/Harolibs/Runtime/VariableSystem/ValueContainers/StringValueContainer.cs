using UnityEngine;

namespace HaroLibs {
    [CreateAssetMenu( fileName = nameof( StringValueContainer ), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof( StringValueContainer ) )]
    public class StringValueContainer : ValueContainerBase<string> { }
}
