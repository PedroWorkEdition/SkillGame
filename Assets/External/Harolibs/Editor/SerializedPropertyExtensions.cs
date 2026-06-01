using HaroLibs;
using HaroLibs.Cursed.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UObj = UnityEngine.Object;

namespace HaroLibsEditor {
    public static class SerializedPropertyExtensions {

        const string k_BackingField = "<{0}>k__BackingField";
        const string k_ArrayKey = "Array";
        const string k_regexFilter = "<([^>]+)>";

        //property.Array.data[0] if it goes too deep it dies, fix it later
        public static MemberInfo GetMemberInfo( this SerializedProperty property ) {
            Type currentType = property.serializedObject.targetObject.GetType();
            //int arrayIndex = -1;
            var targetName = property.propertyPath.Split( '.' )[ ^1 ];
            if (targetName.Contains( k_BackingField[ 6.. ] ))
                targetName = Regex.Match( targetName, k_regexFilter ).Groups[ 1 ].Value;
            if (targetName.Contains( "data[" )) {
                //arrayIndex = targetName[ ^1 ] - '0';
                targetName = property.propertyPath.Split( '.' )[ ^3 ];
            }
            bool skipNext = false;
            foreach (var curName in property.propertyPath.Split( '.' )) {
                var name = curName;
                bool isProp = false;
                if (name == k_ArrayKey || skipNext) {
                    skipNext = name == k_ArrayKey;
                    continue;
                }
                if (name.Contains( k_BackingField[ 6.. ] )) {
                    name = Regex.Match( name, k_regexFilter ).Groups[ 1 ].Value;
                    isProp = true;
                }
                //DEBUG_LogAllMembers( currentType );
                MemberInfo[] mem = isProp ? new MemberInfo[] { currentType.GetProperty( name ) } :
                                            currentType.GetMember( name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default );
                if (mem == null || mem.Length < 1) break;
                if (targetName == mem[ 0 ].Name)
                    return mem[ 0 ];
                currentType = mem[ 0 ].GetMemberType();
                if (currentType.IsArray)
                    currentType = currentType.GetElementType();
            }
            Debug.LogError( $"Failed to get {property.displayName} type, Path: {property.propertyPath}\nif it's from inheretance, it must be 'protected'" );
            return null;
        }

        static void DEBUG_LogAllMembers( Type type ) {
            var members = type.GetMembers();
            foreach (var member in members)
                Debug.Log( member.Name );
        }

        public static void SetArrayValue<T>( this SerializedProperty property, params T[] vals ) {
            if (!property.isArray) { Debug.Log( "The current property is not an array" ); return; }
            property.ClearArray();
            property.arraySize = vals.Length;
            property.serializedObject.ApplyModifiedProperties();
            for (int i = 0; i < vals.Length; i++)
                property.GetArrayElementAtIndex( i ).SetValue( vals[ i ] );
            property.serializedObject.ApplyModifiedProperties();
        }

        public static Type GetSerializedType( this SerializedProperty property ) => property.GetMemberInfo().GetMemberType();

        public static SerializedProperty FindBackProperty( this SerializedObject target, string propertyName ) =>
                                                            target.FindProperty( string.Format( k_BackingField, propertyName ) );

        public static SerializedProperty FindBackingField( this SerializedProperty property, string propertyName ) =>
                                                            property.FindPropertyRelative( string.Format( k_BackingField, propertyName ) );

        public static void SetValue( this SerializedProperty property, object val, bool applyChanges  = false ) {
            switch (property.propertyType) {
                case SerializedPropertyType.String: property.stringValue = ( string )val; break;
                case SerializedPropertyType.Integer: property.intValue = ( int )val; break;
                case SerializedPropertyType.Float: property.floatValue = ( float )val; break;
                case SerializedPropertyType.Boolean: property.boolValue = ( bool )val; break;
                case SerializedPropertyType.ObjectReference: property.objectReferenceValue = ( UObj )val; break;
                case SerializedPropertyType.ManagedReference: property.managedReferenceValue = val; break;
                case SerializedPropertyType.Vector2: property.vector2Value = ( Vector2 )val; break;
                case SerializedPropertyType.Vector3: property.vector3Value = ( Vector3 )val; break;
                case SerializedPropertyType.ExposedReference: property.exposedReferenceValue = ( UObj )val; break;
                default: break;
            }
            if(applyChanges)
                property.serializedObject.ApplyModifiedProperties();
        }

        public static object GetValue( this SerializedProperty property ) =>
            property.propertyType switch {
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Integer => property.intValue,
                SerializedPropertyType.Float => property.floatValue,
                SerializedPropertyType.Boolean => property.boolValue,
                SerializedPropertyType.ObjectReference => property.objectReferenceValue,
                SerializedPropertyType.ManagedReference => property.managedReferenceValue,
                SerializedPropertyType.Vector2 => property.vector2Value,
                SerializedPropertyType.Vector3 => property.vector3Value,
                SerializedPropertyType.ExposedReference => property.exposedReferenceValue,
                _ => default
            };


        public static List<SerializedProperty> ToPropList( this SerializedProperty property ) {
            var list = new List<SerializedProperty>();
            for (var i = 0; i < property.arraySize; i++)
                list.Add( property.GetArrayElementAtIndex( i ) );
            return list;
        }

        public static List<T> ToList<T>( this SerializedProperty property ) {
            if (!property.isArray) return null;
            List<T> vals = new();
            for (int i = 0; i < property.arraySize; i++)
                vals.Add( property.GetArrayElementAtIndex( i ).GetValue().AsGeneric<T>() );
            return vals;
        }

        public static T[] ToArray<T>( this SerializedProperty property ) => property.ToList<T>().ToArray();

        public static PropertyField CreateField( this SerializedProperty property, Action<PropertyField> onCreated = null ) =>
                                                 VEUtil.CreatePropField( property, onCreated );

        public static PropertyField CreateField( this SerializedProperty property, bool removeLabel ) =>
                                                 VEUtil.CreatePropField( property, field => {
                                                     if (!removeLabel) return;
                                                     field.Q<Label>()?.SetActive( false );
                                                 } );
    }

}
