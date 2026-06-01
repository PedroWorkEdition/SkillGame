using HaroLibs;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HaroLibsEditor {

    [CustomPropertyDrawer( typeof( MaskItemAttribute ) )]
    public class MaskItemAttributeDrawer : PropertyDrawer {

        const char k_separator = ',';

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) => 0;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            var items = GetItemsList( property );
            var index = GetIndex( property.stringValue, items );
            property.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            if (items.Count == 0) {
                EditorGUILayout.LabelField( $"{property.displayName} (List is empty)" );
                return;
            }
            index = EditorGUILayout.MaskField( label, index, items.ToArray() );
            if (EditorGUI.EndChangeCheck()) {
                var selected = new List<string>();
                for (int i = 0; i < items.Count; i++)
                    if (( 1 << i & index ) > 0)
                        selected.Add( items[ i ] );
                property.stringValue = selected.Count > 0 ? string.Join( k_separator, selected ) : string.Empty;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        List<string> GetItemsList( SerializedProperty property ) {
            var att = attribute as DropdownAttribute;
            var serObj = property.serializedObject;
            var (type, source) = AssemblyTypeDefinitions.GetTypeByPath( serObj.targetObject, property.propertyPath );
            var methodInfo = type.GetMethod( att.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            if (methodInfo?.Invoke( source, null ) is not string[] names) {
                Debug.LogError( "failed to get names listing from method", serObj.targetObject );
                return null;
            }
            return names.ToList();
        }

        int GetIndex( string source, List<string> items ) {
            int index = 0;
            if (string.IsNullOrEmpty( source ))
                return index;
            var splited = source.Split( k_separator );
            foreach (var split in splited)
                index |= 1 << items.IndexOf( split );
            return index;
        }

    }

}
