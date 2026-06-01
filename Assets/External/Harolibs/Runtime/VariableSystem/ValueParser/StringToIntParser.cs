using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {
    public class StringToIntParser : ScriptableObject {

        [SerializeField] UnityEvent<int> parsedSuccess;

        public void TryParse( string str ) {
            if (!int.TryParse( str, out var val )) return;
            parsedSuccess?.Invoke( val );
        }

    }

}
