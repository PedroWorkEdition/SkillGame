using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using HaroLibs;

namespace HaroLibsEditor {
    [CustomPropertyDrawer( typeof( TagViewAttribute ) )]
    public class TagViewDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            var field = new TagField( property.displayName );
            field.BindProperty( property );
            return field;
        }

    }
}
