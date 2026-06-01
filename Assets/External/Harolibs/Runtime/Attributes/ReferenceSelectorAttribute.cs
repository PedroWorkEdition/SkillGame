using UnityEngine;

namespace HaroLibs {
    public class ReferenceSelectorAttribute : PropertyAttribute {

        public bool UseFoldOut = false;

        public ReferenceSelectorAttribute( bool removeFoldOut = false ) => UseFoldOut = !removeFoldOut;

    }

}
