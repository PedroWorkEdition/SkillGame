using HaroLibs;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {

    [CustomPropertyDrawer(typeof(InterfaceTypeAttribute))]
    public class InterfaceTypeAttributeDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) => GetField( property );

        VisualElement GetField( SerializedProperty prop ) {
            var targetType = GetTargetType( prop );
            if (targetType == null)
                return new Label( "Failed to get field" );
            var propType = prop.GetSerializedType();
            if (!targetType.IsAssignableFrom( propType )) {
                if (propType.IsArray) propType = propType.GetElementType();
                if (propType.IsGenericType) propType = propType.GenericTypeArguments[0];
            }
            var field = new ObjectField { objectType = propType, label = prop.displayName, value = prop.objectReferenceValue };
            var emptyLabel = new Label( $"{targetType.Name} ({propType.Name})" );
            var normalLabel = ( Label )field[ 1 ][ 0 ][ 1 ];
            emptyLabel.AddClassRange( "unity-object-field-display__label", "unity-object-field-display__label--value-null" );
            field[ 1 ][ 0 ].Add( emptyLabel );
            emptyLabel.SetActive( field.value == null );
            normalLabel.SetActive( field.value != null );
            field.RegisterCallback<ChangeEvent<Object>>( ctx => {
                if (ctx.newValue == null) {
                    emptyLabel.SetActive( true );
                    normalLabel.SetActive( false );
                    prop.objectReferenceValue = null;
                    prop.serializedObject.ApplyModifiedProperties();
                    return; 
                }
                var val = ctx.newValue;
                if (!targetType.IsAssignableFrom( val.GetType() )) {
                    if (propType == typeof( MonoBehaviour ) &&
                        val is MonoBehaviour mb &&
                        mb.TryGetComponent( targetType, out var target ))
                        val = target;
                    else {
                        field.value = ctx.previousValue;
                        return;
                    }
                }
                emptyLabel.SetActive( false );
                normalLabel.SetActive( true );
                prop.objectReferenceValue = val;
                prop.serializedObject.ApplyModifiedProperties();
            } );
            return field;
        }
            
        System.Type GetTargetType( SerializedProperty prop ) {
            var att = attribute as InterfaceTypeAttribute; //prop.GetMemberInfo()?.GetCustomAttribute<InterfaceTypeAttribute>();
            if (att.TargetType != null)
                return att.TargetType;
            var (type, source) = AssemblyTypeDefinitions.GetTypeByPath( prop.serializedObject.targetObject, prop.propertyPath );
            var methodInfo = type.GetMethod( att.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            if (methodInfo?.Invoke( source, null ) is not System.Type retType) {
                Debug.LogError( $"failed to get type from method '{att.MethodName}' of '{prop.serializedObject.targetObject.GetType()}'", 
                                prop.serializedObject.targetObject );
                return null;
            }
            return retType;
        }

    }

}
