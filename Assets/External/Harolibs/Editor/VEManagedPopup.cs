using HaroLibs;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HaroLibsEditor {

    public class VEManagedPopup<T> : VEManagedPopup {

        public VEManagedPopup( SerializedProperty prop, bool fold = false ) : base( typeof( T ), prop, fold ) { }

    }

    public class VEManagedPopup : VisualElement {

        const string k_fouldOutKey = "VEManagedPopup_{0}_Foldout";

        Dictionary<string, Type> _dataMap;

        VisualElement _dataContainer;
        readonly Type _targetType;
        readonly SerializedProperty _dataProp;
        readonly bool _fold;

        public VEManagedPopup( Type type, SerializedProperty prop, bool fold = false ) {
            _dataProp = prop;
            _targetType = type.IsArray ? type.GetElementType() : type;
            _fold = fold;
            Initialize();
            PopupDataField();
        }

        void Initialize() {
            _dataMap = new();
            var types = AssemblyTypeDefinitions.GetAllTypes( _targetType );
            foreach (var type in types) {
                var name = GetTypeName( type );
                _dataMap.Add( name, type );
            }
        }

        void PopupDataField() {
            var types = new List<string> { "None" };
            types.AddRange( _dataMap.Keys );
            Add( PopupElement( types ) );
            _dataContainer = new();
            _dataContainer.Add( DataField() );
            Add( _dataContainer );
        }

        VisualElement PopupElement( List<string> types ) {
            var container = new VisualElement();
            var popup = new PopupField<string>( types, 0 ) { label = _dataProp.displayName, value = GetTypeName( _dataProp.managedReferenceValue?.GetType() ) ?? "None" };
            popup.RegisterCallback<ChangeEvent<string>>( ctx => SetDataValue( ctx.newValue ) );
            container.style.flexDirection = FlexDirection.Row;
            popup.style.flexGrow = 1;
            container.Add( popup );
            container.SetActive( true );
            return container;
        }

        VisualElement DataField() {
            VisualElement container = new();
            /*
            var mem = _dataProp.managedReferenceValue?.GetType().GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
            if (mem == null) return container;
            foreach (var m in mem) {
                var propField = new PropertyField();// must bind manually if we want to update the ui (funny '-')
                var prop = _dataProp.FindPropertyRelative( m.Name );
                if (prop != null)
                    propField.BindProperty( prop );
                else
                    continue;
                VEUtil.CreatePropField( _dataProp.FindPropertyRelative( m.Name ) );
                container.Add( propField );
            }
            */
            if (_dataProp.managedReferenceValue == null) return container;
            container.Add( VEUtil.CreatePropField( _dataProp, !_fold ) );
            return container;
        }

        VisualElement FoldOutContainer( VisualElement container, string label = null ) =>
            new VEFoldout( container, k_fouldOutKey, label );
            /*
        {
            label ??= "Container";
            var foldout = new Foldout {
                text = label,
                value = EditorPrefs.GetBool( FOLDOUT_KEY, true )
            };
            foldout.RegisterCallback<ChangeEvent<bool>>( ctx => EditorPrefs.SetBool( string.Format( FOLDOUT_KEY, foldout.text ), ctx.newValue ) );
            foldout.Add( container );
            return foldout;
        }
            */
        void SetDataValue( string type ) {
            _dataContainer?.Clear();
            _dataProp.managedReferenceValue = _dataMap.ContainsKey( type ) ? Activator.CreateInstance( _dataMap[ type ] ) : null;
            _dataProp.serializedObject.ApplyModifiedProperties();
            _dataContainer.SetActive( _dataProp.managedReferenceValue != null );
            if (_dataProp.managedReferenceValue == null) return;
            _dataContainer?.Add( DataField() );
        }

        string GetTypeName( Type t ) => t != null && t.Name.Contains( '+' ) ? t.Name.Split( '+' )[ ^1 ] : t?.Name;

    }

    public class VEFoldout : Foldout {

        public VEFoldout( VisualElement content, string key, string label = null ) {
            label ??= "Container";
            text = label;
            value = EditorPrefs.GetBool( string.Format( key, text ), true );
            Add( content );
            RegisterCallback<ChangeEvent<bool>>( ctx => EditorPrefs.SetBool( string.Format( key, text ), ctx.newValue ) );
        }

        public VEFoldout( string key, string label = null, params VisualElement[] elements ) {
            text = label;
            value = EditorPrefs.GetBool( key, true );
            this.AddRange( elements );
            RegisterCallback<ChangeEvent<bool>>( ctx => EditorPrefs.SetBool( string.Format( key, text ), ctx.newValue ) );
        }

    }
}
