using HaroLibs;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    [CustomPropertyDrawer( typeof( ReferenceSelectorAttribute ) )]
    public class ReferenceSelectorAttributeEditor : PropertyDrawer {

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            var container = new VisualElement();
            var mem = property.GetMemberInfo();
            if (mem.GetCustomAttribute<SerializeReference>() == null) 
                return container.AddRange( new Label( $"Object is not a {nameof( SerializeReference )}" ) );
            var att = attribute as ReferenceSelectorAttribute;
            container.Add( new VEManagedPopup( mem.GetMemberType(), property, att.UseFoldOut ) );
            return container;
        }

    }

}
