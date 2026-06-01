using UnityEngine;

namespace HaroLibs {
    [CreateAssetMenu( fileName = nameof( CurveValueContainer ), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof( CurveValueContainer ) )]
    public class CurveValueContainer : ValueContainerBase<AnimationCurve> { }
}
