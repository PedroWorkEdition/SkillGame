using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using HaroLibs;
using System.Collections.Generic;

namespace HaroLibsEditor {

    [CustomPropertyDrawer( typeof( DropdownAttribute ) )]
    public class DropdownAttributeDrawer : PropertyDrawer {

        bool _useEmpty;

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) => 0;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            var att = attribute as DropdownAttribute;
            _useEmpty = att.UseEmpty;
            var items = GetItemsList( property, att );
            if (_useEmpty) items.Insert( 0, "Empty" );
            ValidatePropertyValue( property, items );
            var index = GetIndex( items, property );
            property.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            index = EditorGUILayout.Popup( label, index, items.ToArray() );
            if (EditorGUI.EndChangeCheck()) {
                // do a setter here for different types
                SetValue( items, property, index );
                property.serializedObject.ApplyModifiedProperties();
            }
        }
        
        List<string> GetItemsList( SerializedProperty property, DropdownAttribute att ) {
            var serObj = property.serializedObject;
            var (type, source) = AssemblyTypeDefinitions.GetTypeByPath( serObj.targetObject, property.propertyPath );
            var methodInfo = type.GetMethod( att.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            if (methodInfo?.Invoke( source, null ) is not string[] names) {
                Debug.LogError( "failed to get names listing from method", serObj.targetObject );
                return null; 
            }
            return names.ToList();
        }

        void ValidatePropertyValue( SerializedProperty property, List<string> items ) {
            if (property.propertyType != SerializedPropertyType.String || !string.IsNullOrEmpty( property.stringValue )) return;
            property.stringValue = items[ 0 ];
            property.serializedObject.ApplyModifiedProperties();
        }

        int GetIndex( List<string> items, SerializedProperty prop ) =>
                    prop.propertyType == SerializedPropertyType.String ? items.IndexOf( prop.stringValue ) : 
                    prop.propertyType == SerializedPropertyType.Integer ? prop.intValue + ( _useEmpty ? 1 : 0 ) : 0;
        
        void SetValue( List<string> items, SerializedProperty property, int index ) {
            if (property.propertyType == SerializedPropertyType.String) property.stringValue = items[ index ];
            if (property.propertyType == SerializedPropertyType.Integer) property.intValue = index - ( _useEmpty ? 1 : 0);
        }

    }
}
