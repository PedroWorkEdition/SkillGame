using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {
    public class StringToFloatParser : ScriptableObject {

        [SerializeField] float defaultValue;
        [SerializeField] UnityEvent<float> parsedSuccess;

        public void TryParse( string str ) {
            float val = float.TryParse( str, out var v ) ? v : defaultValue;
            parsedSuccess?.Invoke( val );
        }

    }

}
