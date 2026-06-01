using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HaroLibs {
    [CreateAssetMenu( fileName = nameof( InputStringContainer ), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof( InputStringContainer ) )]
    public class InputStringContainer : ContainerBase<string> {

        [SerializeField] InputActionReference input;
        [SerializeField] string prefix, suffix;
        public override string Value {
            get => prefix + input.action.controls.First().displayName + suffix;
            set { } }

    }

}
