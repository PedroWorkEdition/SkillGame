using HaroLibs;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace HaroLibsEditor {

    [CustomEditor( typeof( ValueObserverBase<> ), true )]
    public class ValueObserverBaseEditor : Editor {

        public override VisualElement CreateInspectorGUI() =>
            new VisualElement().AddRange(
                VEUtil.CreatePropField( serializedObject.FindProperty( "targetValue" ) ),
                CreateBox().WithStyle( stl => stl.flexDirection = FlexDirection.Row ).AddRange(
                    CreateBoolField( serializedObject.FindProperty( "manual" ) ),
                    CreateBoolField( serializedObject.FindProperty( "onEnable" ) )
                ),
                EventContainer()
            );

        protected virtual VisualElement EventContainer() =>
                          CreateBox().AddRange( serializedObject.CreatePropField( "onChanged" ) );

        protected virtual Box CreateBox() => new() {
            style = {
                marginBottom = 5,
                marginTop = 5,
                paddingBottom = 5,
                paddingTop = 5,
            }
        };

        protected BaseBoolField CreateBoolField( SerializedProperty prop ) {
            var field = new Toggle( prop.displayName ) { style = { flexGrow = 1 } };
            field.BindProperty( prop );
            return field;
        }

    }
}
