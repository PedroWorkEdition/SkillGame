using HaroLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {

    [CustomPropertyDrawer( typeof( TimeViewAttribute ) )]
    public class TimeViewAttributeDrawer : PropertyDrawer {

        const string k_hourToken = "h", k_minuteToken = "m", k_secondsToken = "s", k_unityInputElementName = "unity-text-input";
        const uint k_limitValue = 86400;

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            if (property.propertyType != SerializedPropertyType.Integer)
                return new Label( $"{property.displayName} Must be a 'uint'" );
            var att = attribute as TimeViewAttribute;
            return CreateTimerField( property, att.TimeFlags );
        }

        VisualElement CreateTimerField( SerializedProperty targetProp, TimeAvailable timeAvailable ) {// GOD, this is horrible
            var lbl = new Label( targetProp.displayName ) { style = { minWidth = new StyleLength( Length.Percent( 30 ) ) } };
            var elements = new List<VisualElement>() { lbl };
            var fields = new List<TimeInputField>();
            var span = TimeSpan.FromSeconds( targetProp.uintValue );
            if (( timeAvailable & TimeAvailable.Hours ) > 0) {
                CreateFieldInContext( k_hourToken, span.Hours, 3600, span => ( uint )span.Hours );
                if (( timeAvailable ^ TimeAvailable.Hours ) > 0) 
                    elements.Add( SeparationLabel() ); 
            }
            if (( timeAvailable & TimeAvailable.Minutes ) > 0) {
                CreateFieldInContext( k_minuteToken, span.Minutes, 60, span => ( uint )span.Minutes );
                if (( timeAvailable & TimeAvailable.Seconds ) > 0)
                    elements.Add( SeparationLabel() );
            }
            if (( timeAvailable & TimeAvailable.Seconds ) > 0) 
                CreateFieldInContext( k_secondsToken, span.Seconds, 1, span => ( uint )span.Seconds );
            return new VisualElement() { style = { flexDirection = FlexDirection.Row, marginLeft = 3 } }.AddRange( elements.ToArray() );
            void UpdateValue() {
                targetProp.uintValue = Math.Min( (uint)fields.Sum( f => f.GetCurrentValue() ), k_limitValue - 1 );
                targetProp.serializedObject.ApplyModifiedProperties();
                var span = TimeSpan.FromSeconds( targetProp.uintValue );
                fields.ForEach( f => f.ApplyValue( span ) );
            }
            TimeInputField CreateFieldInContext( string label, int startingValue, uint multiplier, Func<TimeSpan, uint> setter ) {
                TimeInputField timeField = null;
                timeField = CreateField( label, ( uint )startingValue, () => timeField.value * multiplier, setter, _ => UpdateValue() );
                elements.Add( timeField );
                fields.Add( timeField );
                return timeField;
            }
        }

        TimeInputField CreateField( string label, uint startingValue, Func<uint> getter, Func<TimeSpan, uint> setter, EventCallback<ChangeEvent<uint>> callback ) {
            var field = new TimeInputField( label, getter, setter ) {
                style = { flexDirection = FlexDirection.RowReverse },
                value = startingValue,
                isDelayed = true
            };
            field.labelElement.style.minWidth = 0;
            var inputElement = field.Q<VisualElement>( k_unityInputElementName );
            inputElement.style.unityTextAlign = TextAnchor.MiddleCenter;
            inputElement.style.minWidth = 30;
            field.RegisterValueChangedCallback( callback );
            return field;
        }

        Label SeparationLabel() => new ( ":" ) { style = { marginLeft = 7 } };

        internal class TimeInputField : UnsignedIntegerField {

            readonly Func<uint> _getCurrentValue;
            readonly Func<TimeSpan, uint> _onTotalValueChanged;

            public TimeInputField( string label, Func<uint> getter, Func<TimeSpan, uint> setter ) : base( label ) =>
                (_getCurrentValue, _onTotalValueChanged) = (getter, setter);
            
            public uint GetCurrentValue() => _getCurrentValue();
            public void ApplyValue( TimeSpan span ) => SetValueWithoutNotify( _onTotalValueChanged( span ) );
            
        }

    }
}
