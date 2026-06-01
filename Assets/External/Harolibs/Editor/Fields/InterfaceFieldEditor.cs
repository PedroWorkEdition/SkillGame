using HaroLibs;
using UnityEditor;
using UnityEngine.UIElements;

namespace HaroLibsEditor {

    [CustomPropertyDrawer( typeof( InterfaceField<> ) )]
    public class InterfaceFieldEditor : PropertyDrawer {

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            var lbl = new Label( property.displayName ) { style = { marginLeft = 5, flexGrow = .5f } };
            var boolProp = property.FindBackingField( "UseObjectReference" );
            var refField = property.FindPropertyRelative( "reference" ).CreateField( true ).WithStyle( stl => stl.flexGrow = 1 );
            var objField = property.FindPropertyRelative( "obj" ).CreateField( true ).WithStyle( stl => stl.flexGrow = .5f );
            var changeBtn = VEUtil.CreateButton( "<>", () => {
                UpdateFields( boolProp.boolValue = !boolProp.boolValue );
                property.serializedObject.ApplyModifiedProperties();
            } );
            UpdateFields( boolProp.boolValue );
            return new VisualElement() { style = { flexDirection = FlexDirection.Row } }
                                      .AddRange( lbl, changeBtn, refField, objField );
            void UpdateFields( bool val ) {
                refField.SetActive( !val );
                objField.SetActive( val );
            }
        }

    }

}
