using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {

    public static partial class VEUtil {

        public class MinMaxContainer : VisualElement {

            const string k_PrefsKey = "{0}_{1}_MinMaxContainer", k_MinPath = "Min", k_MaxPath = "Max";

            public readonly MinMaxSlider ValueSlider;

            public MinMaxContainer( SerializedProperty minProp, SerializedProperty maxProp, int? min = null, int? max = null ) {
                Vector2Int mm = new( min ?? 0, max ?? 100 );
                ValueSlider = new MinMaxSlider( GetNumericValue( minProp, 0 ), GetNumericValue( maxProp, 100 ),
                            EditorPrefs.GetFloat( GetPrefsPath( minProp, k_MinPath ), mm.x ), EditorPrefs.GetFloat( GetPrefsPath( maxProp, k_MaxPath ), mm.y ) );
                ValueSlider.RegisterCallback<ChangeEvent<Vector2>>( ctx => {
                    SetNumericValue( minProp, ctx.newValue.x );
                    SetNumericValue( maxProp, ctx.newValue.y );
                    minProp.serializedObject.ApplyModifiedProperties();
                } );
                ValueSlider.style.paddingLeft = ValueSlider.style.paddingRight = 10;
                ValueSlider.style.flexGrow = .8f;
                var upperContainer = new VisualElement().WithStyle( stl => stl.flexDirection = FlexDirection.Row )
                                                        .AddRange( GetLimitField( minProp, mm.x, k_MinPath, val => ValueSlider.lowLimit = val ),
                                                                   ValueSlider,
                                                                   GetLimitField( maxProp, mm.y, k_MaxPath, val => ValueSlider.highLimit = val ) );
                var lowerContainer = new VisualElement().WithStyle( stl => stl.flexDirection = FlexDirection.Row )
                                                        .AddRange( GetNumericElement( minProp, val => ValueSlider.minValue = val ),
                                                                   GetNumericElement( maxProp, val => ValueSlider.maxValue = val ) );
                this.AddRange( upperContainer, lowerContainer );
            }

            FloatField GetLimitField( SerializedProperty targetProp, float defaultValue, string loadPath, Action<float> onChanged ) {
                var field = new FloatField() { value = EditorPrefs.GetFloat( GetPrefsPath( targetProp, loadPath ), defaultValue ) };
                field.RegisterCallback<ChangeEvent<float>>( ctx => {
                    onChanged?.Invoke( ctx.newValue );
                    EditorPrefs.SetFloat( GetPrefsPath( targetProp, loadPath ), ctx.newValue );
                } );
                field.style.flexGrow = .1f;
                return field;
            }

            string GetPrefsPath( SerializedProperty prop, string loadPath ) => string.Format( k_PrefsKey, prop.propertyPath, loadPath );

            [Obsolete("it was just annoying rly")]
            static (byte[] min, byte[] max) GetMinMaxValue( SerializedProperty reference ) => reference.numericType switch {
                SerializedPropertyNumericType.Int8 => new( BitConverter.GetBytes( sbyte.MinValue ), BitConverter.GetBytes( sbyte.MaxValue ) ),
                SerializedPropertyNumericType.UInt8 => new( BitConverter.GetBytes( byte.MinValue ), BitConverter.GetBytes( byte.MaxValue ) ),
                SerializedPropertyNumericType.Int16 => new( BitConverter.GetBytes( short.MinValue ), BitConverter.GetBytes( short.MaxValue ) ),
                SerializedPropertyNumericType.UInt16 => new( BitConverter.GetBytes( ushort.MinValue ), BitConverter.GetBytes( ushort.MaxValue ) ),
                SerializedPropertyNumericType.Int32 => new( BitConverter.GetBytes( int.MinValue ), BitConverter.GetBytes( int.MaxValue ) ),
                SerializedPropertyNumericType.UInt32 => new( BitConverter.GetBytes( uint.MinValue ), BitConverter.GetBytes( uint.MaxValue ) ),
                SerializedPropertyNumericType.Int64 => new( BitConverter.GetBytes( long.MinValue ), BitConverter.GetBytes( long.MaxValue ) ),
                SerializedPropertyNumericType.UInt64 => new( BitConverter.GetBytes( ulong.MinValue ), BitConverter.GetBytes( ulong.MaxValue ) ),
                SerializedPropertyNumericType.Float => new( BitConverter.GetBytes( float.MinValue ), BitConverter.GetBytes( float.MaxValue ) ),
                SerializedPropertyNumericType.Double => new( BitConverter.GetBytes( byte.MinValue ), BitConverter.GetBytes( byte.MaxValue ) ),
                _ => (null, null)
            };

            static float GetNumericValue( SerializedProperty target, float defaultValue ) {
                switch (target.numericType) {
                    case SerializedPropertyNumericType.UInt8:
                    case SerializedPropertyNumericType.UInt16:
                    case SerializedPropertyNumericType.UInt32:
                        return target.uintValue;
                    case SerializedPropertyNumericType.Int8:
                    case SerializedPropertyNumericType.Int16:
                    case SerializedPropertyNumericType.Int32:
                        return target.intValue;
                    case SerializedPropertyNumericType.Int64: return target.longValue;
                    case SerializedPropertyNumericType.UInt64: return target.ulongValue;
                    case SerializedPropertyNumericType.Float: return target.floatValue;
                    case SerializedPropertyNumericType.Double: return ( float )target.doubleValue;
                    default: return defaultValue;
                }
            }

            static void SetNumericValue( SerializedProperty target, float val ) {
                switch (target.numericType) {
                    case SerializedPropertyNumericType.UInt8:
                    case SerializedPropertyNumericType.UInt16:
                    case SerializedPropertyNumericType.UInt32:
                        target.uintValue = ( uint )Mathf.RoundToInt( val ); break;
                    case SerializedPropertyNumericType.Int8:
                    case SerializedPropertyNumericType.Int16:
                    case SerializedPropertyNumericType.Int32:
                        target.intValue = Mathf.RoundToInt( val ); break;
                    case SerializedPropertyNumericType.Int64: target.longValue = Mathf.RoundToInt( val ); break;
                    case SerializedPropertyNumericType.UInt64: target.ulongValue = ( ulong )Mathf.RoundToInt( val ); break;
                    case SerializedPropertyNumericType.Float: target.floatValue = val; break;
                    case SerializedPropertyNumericType.Double: target.doubleValue = val; break;
                    default: break;
                }
            }

            static VisualElement GetNumericElement( SerializedProperty prop, Action<float> onChanged ) {
                var field = CreatePropField( prop );
                field.RegisterValueChangeCallback( ctx => onChanged?.Invoke( GetNumericValue( ctx.changedProperty, 0 ) ) );
                return field.WithStyle( stl => stl.flexGrow = 1 );
                /*
                BindableElement target = prop.numericType switch {
                    SerializedPropertyNumericType.Int8 or SerializedPropertyNumericType.Int16 or SerializedPropertyNumericType.Int32 => new IntegerField(),
                    SerializedPropertyNumericType.UInt8 or SerializedPropertyNumericType.UInt16 or SerializedPropertyNumericType.UInt32 => new UnsignedIntegerField(),
                    SerializedPropertyNumericType.Int64 => new LongField(),
                    SerializedPropertyNumericType.UInt64 => new UnsignedLongField(),
                    SerializedPropertyNumericType.Float => new FloatField(),
                    SerializedPropertyNumericType.Double => new DoubleField(),
                    _ => null
                };
                
                //RegisterOnChanged( target, prop, onChanged );
                return target;
                */
            }

            static void RegisterOnChanged( BindableElement element, SerializedProperty prop, Action<float> onChanged ) {
                
                switch( element ) {
                    case IntegerField integerField:
                        integerField.RegisterCallback<ChangeEvent<int>>( ctx => {

                            onChanged?.Invoke( ctx.newValue );
                        } );
                        break;
                }
            }
        }
    }
}
