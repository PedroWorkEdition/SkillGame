using HaroLibs;
using UnityEditor;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    [CustomPropertyDrawer( typeof(AutoGUIDAttribute) )]
    public class AutoGUIDAttributeDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            if (fieldInfo.FieldType != typeof( string ))
                return new Label( "Must be a string type" );
            if (string.IsNullOrEmpty( property.stringValue ))
                GenerateGUID( property );
            var field = VEUtil.CreatePropField( property );
            field.SetEnabled( false );
            return new VisualElement() { style = { flexDirection = FlexDirection.Row } }
                   .AddRange( field, new Button( () => GenerateGUID( property ) ) { text = "Regenerate" } );
        }

        void GenerateGUID( SerializedProperty property ) {
            property.stringValue = GUID.Generate().ToString();
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

    }
}
