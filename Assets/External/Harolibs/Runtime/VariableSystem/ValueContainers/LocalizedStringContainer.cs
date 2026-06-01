#if UNITY_LOCALIZATION
using UnityEngine;
using UnityEngine.Localization;

namespace HaroLibs {
    [CreateAssetMenu( fileName = nameof(LocalizedStringContainer), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof(LocalizedStringContainer) )]
    public class LocalizedStringContainer : ContainerBase<string> {

        
        [SerializeField] LocalizedString target;
        [SerializeField] string prefix, suffix;
        public override string Value { get => prefix + target.GetLocalizedString() + suffix; set { } }
        
    }

}
#endif
