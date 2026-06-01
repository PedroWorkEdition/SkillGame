using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections;
using HaroLibs;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace HaroLibsEditor {

    public class VESliderInt : SliderInt {

        readonly IntegerField _intField;

        public VESliderInt( int startValue ,string label, int start = 0, int end = 10, SliderDirection direction = SliderDirection.Horizontal, int pageSize = 0 ) :
            base( label, start, end, direction, pageSize ) {
            _intField = new IntegerField();
            value = _intField.value = startValue;
            _intField.labelElement.style.display = DisplayStyle.None;
            _intField.style.marginRight = 0;
            _intField.style.marginLeft = 12;
            _intField.style.minWidth = 64;
            Add( _intField );
            RegisterCallback<ChangeEvent<int>>( ctx => _intField.value = ctx.newValue );
            _intField.RegisterCallback<ChangeEvent<int>>( ctx => value = Mathf.Clamp( ctx.newValue, start, end ));
        }

    }

    public static partial class VEUtil {

        public static VisualElement Space( Vector2Int space ) => new SpaceElement( space );

        public static IntegerField IntField( SerializedProperty property, int min = int.MinValue, int max = int.MaxValue ) {
            var field = new IntegerField { label = property.displayName };
            field.BindProperty( property );
            field.RegisterCallback<ChangeEvent<int>>( ctx => { property.intValue = Mathf.Clamp( ctx.newValue, min, max ); property.serializedObject.ApplyModifiedProperties(); } );
            return field;
        }

        public static FloatField FloatField( SerializedProperty property, float min = float.MinValue, float max = float.MaxValue ) {
            var field = new FloatField { label = property.displayName };
            field.BindProperty( property );
            field.RegisterCallback<ChangeEvent<float>>( ctx => { property.floatValue = Mathf.Clamp( ctx.newValue, min, max ); property.serializedObject.ApplyModifiedProperties(); } );
            return field;
        }

        public static FloatField FloatField( SerializedProperty property, bool useLabel, Func<Vector2> minMax, Action<float> onChanged = null ) {
            var field = new FloatField { label = property.displayName };
            field.BindProperty( property );
            field.labelElement.SetActive( useLabel );
            field.RegisterCallback<ChangeEvent<float>>( ctx => { 
                var bounds = minMax?.Invoke() ?? new Vector2( float.MinValue, float.MaxValue );
                var val = Mathf.Clamp( ctx.newValue, bounds.x, bounds.y );
                property.SetValue( val, true );
                onChanged?.Invoke( val );
            } );
            return field;
        }

        public static VisualElement Field<T> ( SerializedProperty property, string label = null, Action<T> onChanged = null ) {
            var element = Field( property, label );
            element.RegisterCallback<ChangeEvent<T>>( ctx => onChanged?.Invoke( ctx.newValue ) );
            return element;
        }

        public static VisualElement Field( SerializedProperty property, string label = null ) {
            if (property == null) return null;
            string displayName = label ?? property.displayName;
            VisualElement element = property.propertyType switch {
                SerializedPropertyType.Boolean => new Toggle( displayName ),
                SerializedPropertyType.String => new TextField( displayName ),
                SerializedPropertyType.Vector2 => new Vector2Field( displayName ),
                SerializedPropertyType.Vector2Int => new Vector2IntField( displayName ),
                SerializedPropertyType.Vector3 => new Vector3Field( displayName ),
                SerializedPropertyType.Integer => new IntegerField( displayName ),
                SerializedPropertyType.Float => new FloatField( displayName ),
                SerializedPropertyType.ObjectReference => new ObjectField( displayName ),
                SerializedPropertyType.Enum => new EnumField( displayName ),
                _ => new()
            };
            if (element is ObjectField objField)
                objField.objectType = GetType( property );
            if( element is BindableElement bindable )
                bindable.BindProperty( property );
            return element;
        }

        public static Button CreateButton( string label, Action onClick = null ) {
            var btn = new Button { text = label };
            btn.clicked += onClick;
            return btn;
        }

        public static T AddRange<T>( this T target, params VisualElement[] elements ) where T : VisualElement {
            if( elements == null ) return null;
            for ( var i = 0; i < elements.Length; i++ ) 
                target.Add( elements[ i ] );
            return target;
        }

        public static T WithStyle<T>( this T target, Action<IStyle> action ) where T : VisualElement { 
            action?.Invoke( target.style ); 
            return target; 
        }

        public static T EditLabel<T>( this T target, Action<Label> callback ) where T : BindableElement {
            if ( target == null || callback == null) {
                Debug.LogError( $"Could not edit label! (Element: {target}, callback is null? {callback == null})" );
                return target;
            }
            if (target.GetType().FindMembers( MemberTypes.All, BindingFlags.Instance | BindingFlags.Public, null, null )
                                .FirstOrDefault( mem => mem.Name == "labelElement" ).GetMemberValue( target ) is not Label label) {
                Debug.LogError( $"Target does not have a label element" );
                return target;
            }
            callback.Invoke( label );
            return target;
        }

        public static Type GetType( SerializedProperty property ) {
            //field.Array.data[0].Target <-- deal with array in SerializedProperty.path
            string[] path = property.propertyPath.Split('.');
            Type targetType = property.serializedObject.targetObject.GetType();
            FieldInfo currentField = null;
            for (int i = 0; i < path.Length; i++) {
                if (path[i] == "Array") {
                    var list = ( IList )currentField.GetValue( property.serializedObject.targetObject );
                    int index = int.Parse( path[ i + 1 ].Replace( "data[", "" ).Replace( "]", "" ) );
                    targetType = list[index].GetType();
                    i++;
                    continue;
                }
                currentField = targetType.GetField( path[ i ], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
                targetType = currentField.FieldType;
            }
            return targetType;
        }

        public static SerializedProperty AddElementToArray( this SerializedProperty target ) {
            if (!target.isArray) return null;
            target.InsertArrayElementAtIndex( target.arraySize );
            target.serializedObject.ApplyModifiedProperties();
            return target.GetArrayElementAtIndex( target.arraySize - 1 );
        }

        public static SerializedProperty AddElementToArray<T>( this SerializedProperty target, T obj ) {
            if (!target.isArray) return null;
            target.InsertArrayElementAtIndex( target.arraySize );
            target.serializedObject.ApplyModifiedProperties();
            target.GetArrayElementAtIndex( target.arraySize - 1 ).SetValue( obj );
            target.serializedObject.ApplyModifiedProperties();
            return target.GetArrayElementAtIndex( target.arraySize - 1 );
        }

        public static SerializedProperty AddElementToArray<T>( this SerializedProperty target, T obj, Action<SerializedProperty, T> setter ) {
            if (!target.isArray) return null;
            target.InsertArrayElementAtIndex( target.arraySize );
            target.serializedObject.ApplyModifiedProperties();
            setter?.Invoke( target.GetArrayElementAtIndex( target.arraySize - 1 ), obj );
            target.serializedObject.ApplyModifiedProperties();
            return target.GetArrayElementAtIndex( target.arraySize - 1 );
        }

        public static PropertyField CreatePropField( SerializedProperty prop, string lbl, bool removeFold ) 
            => CreatePropField( prop, lbl, removeFold ? RemoveFoldout : null );
        public static PropertyField CreatePropField( SerializedProperty prop, bool removeFold ) 
            => CreatePropField( prop, prop.displayName, removeFold ? RemoveFoldout : null );

        public static PropertyField CreatePropField( SerializedProperty prop, Action<PropertyField> onCreated = null ) =>
            CreatePropField( prop, null, onCreated );

        public static PropertyField CreatePropField( SerializedProperty prop, string lbl, Action<PropertyField> onCreated = null ) {
            var field = new PropertyField( prop, lbl ?? prop.displayName );
            field.BindProperty( prop );
            if (onCreated != null) 
                field.RegisterCallbackOnce<GeometryChangedEvent>( ctx => WaitForPropertyFieldToLoad( ctx.target as PropertyField, onCreated ).Forget() );
            return field;
        }

        public static void RemoveFoldout( PropertyField field ) {// removes foldout from property fields, must be called after geometry changed
            var foldout = field.Q<Foldout>();
            if (foldout == null) return;
            foldout.value = true;
            var content = foldout.contentContainer;
            content.RemoveFromHierarchy();
            foldout.parent.Add( content );
            foldout.style.display = DisplayStyle.None;
        }

        public static MinMaxContainer CreateMinMaxSlider( SerializedProperty minProp, SerializedProperty maxProp, int? min = null, int? max = null ) { 
            if( minProp.numericType == SerializedPropertyNumericType.Unknown || minProp.numericType == SerializedPropertyNumericType.Unknown) {
                Debug.LogError( "The serialized property is not a number, please use a number to create the MinMaxSlider" );
                return null;
            }
            return new( minProp, maxProp, min, max ); 
        }

        public static SelectableListView CreateSelectableListView( SerializedProperty arrayProp, Action<SerializedProperty> onSelect, Func<VisualElement[]> bounds = null )
                                                                                            => new( arrayProp, onSelect, bounds );

        internal class SpaceElement : VisualElement {
            public SpaceElement( Vector2Int space ) {
                style.minWidth = space.x;
                style.minHeight = space.y;
            }
        }

        async static UniTask WaitForPropertyFieldToLoad( PropertyField prop, Action<PropertyField> actOnField ) {
            while(prop.childCount == 0) 
                await UniTask.Yield();
            actOnField( prop );
        }

    }
}
