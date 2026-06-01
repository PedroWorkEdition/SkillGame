using HaroLibs;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    [CustomPropertyDrawer( typeof( MinMaxAttribute ) )]
    public class MinMaxAttributeDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            if (property.propertyType != SerializedPropertyType.Vector2)
                return new Label( $"{property.displayName} Must be a vector 2" );
            if (attribute is not MinMaxAttribute att) return new Label( "Oops..." );
            var container = new VisualElement();
            var slider = new MinMaxSlider( property.vector2Value.x, property.vector2Value.y, att.Min, att.Max )
                                         { style = { flexGrow = 1, paddingLeft = 10, paddingRight = 10 } };
            slider.RegisterCallback<ChangeEvent<Vector2>>( ctx => property.SetValue( ctx.newValue, true ) );
            var minValue = GetValueField( property.FindPropertyRelative( "x" ), () => new Vector2( att.Min, property.vector2Value.y ), val => slider.minValue = val );
            var maxValue = GetValueField( property.FindPropertyRelative( "y" ), () => new Vector2( property.vector2Value.x, att.Max ), val => slider.maxValue = val );
            return container.AddRange( new Label( property.displayName ), 
                                       new VisualElement() { style = { flexDirection = FlexDirection.Row } }.AddRange( minValue, slider, maxValue ) );
        }

        FloatField GetValueField( SerializedProperty property, Func<Vector2> getter, Action<float> onChanged ) {
            var field = VEUtil.FloatField( property, false, getter, onChanged );
            field.style.minWidth = field.style.maxWidth = 50;
            field.maxLength = 4;
            field.Q<VisualElement>( name: "unity-text-input" )[0].style.unityTextAlign = TextAnchor.MiddleCenter;
            return field;
        }

    }
}
