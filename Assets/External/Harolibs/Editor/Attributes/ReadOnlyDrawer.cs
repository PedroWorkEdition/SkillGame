using HaroLibs;
using UnityEditor;
using UnityEngine;

namespace HaroLibsEditor {
    [CustomPropertyDrawer( typeof( ReadOnlyAttribute ) )]
    public class ReadOnlyDrawer : PropertyDrawer {

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) => EditorGUI.GetPropertyHeight( property );

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            GUI.enabled = false;
            EditorGUI.PropertyField( position, property, label, true );
            GUI.enabled = true;
        }
    }
}
