using HaroLibs;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace HaroLibsEditor {

    [CustomPropertyDrawer( typeof( SideButtonAttribute ) )]
    public class SideButtonAttributeDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            var att = attribute as SideButtonAttribute;
            var serObj = property.serializedObject;
            var (type, source) = AssemblyTypeDefinitions.GetTypeByPath( serObj.targetObject, property.propertyPath );
            var methodInfo = type.GetMethod( att.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            if (methodInfo == null)
                return property.CreateField();
            var btn = new Button( () => methodInfo.Invoke( source, null ) ) { text = att.MethodName };
            return new VisualElement() { style = { flexDirection = FlexDirection.Row } }
                                      .AddRange( property.CreateField( field => field.style.flexGrow = 1 ), btn );
        }

    }
}
