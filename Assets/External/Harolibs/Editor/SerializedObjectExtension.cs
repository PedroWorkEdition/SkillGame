using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace HaroLibsEditor {
    public static class SerializedObjectExtension {

        public static PropertyField CreatePropField( this SerializedObject serObj, string path, bool isBackingField = false ) {
            var prop = isBackingField ? serObj.FindBackProperty( path ) : serObj.FindProperty( path );
            return VEUtil.CreatePropField( prop );
        }

    }
}
