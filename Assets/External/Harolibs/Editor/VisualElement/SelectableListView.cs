using HaroLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    public class SelectableListView : VisualElement {

        readonly SerializedProperty _targetArray;
        readonly Action<SerializedProperty> _onSelected;
        readonly Func<VisualElement[]> _filterBounds;

        ListElement _selectedElement;
        ListContext _context;

        internal SelectableListView( SerializedProperty arrayProp, Action<SerializedProperty> onSelection, Func<VisualElement[]> filterBounds ) {
            if (!arrayProp.isArray) {
                Add( new Label( "Source must be an array" ) );
                return;
            }
            _filterBounds = filterBounds;
            _onSelected = onSelection;
            _targetArray = arrayProp;
            _context = new() { TargetArrayProp = arrayProp };
        }

        #region Builder

        public SelectableListView WithIconPropertyGetter( Func<SerializedProperty, Sprite> iconProp ) {
            _context.IconPropertyGetter = iconProp;
            return this;
        }

        public SelectableListView WithRawLabel( params string[] labels ) {
            _context.RawLabels = labels;
            return this;
        }

        public SelectableListView WithContextLabel( Func<SerializedProperty, string[]> contextLbls ) {
            _context.ContextLabels = contextLbls;
            return this;
        }

        public SelectableListView WithUpdatableLabel( Func<SerializedProperty, string>[] updatableLbls ) {
            _context.UpdatableLabels = updatableLbls;
            return this;
        }

        public SelectableListView WithLabelPropertyGetter( Func<SerializedProperty, SerializedProperty[]> lblProp ) {
            _context.LabelPropertyGetters = lblProp;
            return this;
        }

        public SelectableListView WithLabelSeparators( params string[] lblSeparators ) {
            _context.LabelSeparators = lblSeparators;
            return this;
        }

        public SelectableListView WithLabelContainers( params KeyValuePair<string, string>?[] containers ) {
            _context.LabelContainers = containers;
            return this;
        }

        public SelectableListView WithLabelStyles( Action<IStyle>[] lblStyles ) {
            _context.LabelStyles = lblStyles;
            return this;
        }

        public SelectableListView WithPropertyGetter( Func<SerializedProperty, SerializedProperty> getProp ) {
            _context.InternalPropertyGetter = getProp;
            return this;
        }

        public SelectableListView Build() {
            for (int i = 0; i < _context.TargetArrayProp.arraySize; i++)
                Add( CreateElement( _context.RawLabels, _context.UpdatableLabels, _context.ContextLabels, _context.LabelPropertyGetters, _context.LabelSeparators, _context.LabelContainers, _context.LabelStyles, _context.IconPropertyGetter,
                                    _context.InternalPropertyGetter?.Invoke( _context.TargetArrayProp.GetArrayElementAtIndex( i ) ) ?? _context.TargetArrayProp.GetArrayElementAtIndex( i ), i ) );
            _context = null;
            return this;
        }

        #endregion

        void OnSelected( ListElement element ) {
            _selectedElement?.Deselect();
            _selectedElement = element;
            _selectedElement?.Select();
            _onSelected?.Invoke( _selectedElement?.Property );
            UpdateLabels();
        }

        public void DeleteSelected() {
            _targetArray.DeleteArrayElementAtIndex( _selectedElement.Index ); 
            _targetArray.serializedObject.ApplyModifiedProperties();
            OnSelected( null );
            UpdateLabels();
        }

        public void RegisterIconField( PropertyField field ) {
            field.RegisterValueChangeCallback( ctx => {
                if(ctx.changedProperty.objectReferenceValue == null) {
                    _selectedElement.SetIconSprite( null );
                    return;
                }
                if(ctx.changedProperty.objectReferenceValue is not Sprite spr) {
                    Debug.LogWarning( "Icon field is not sprite field" );
                    return;
                }
                _selectedElement.SetIconSprite( spr );
            } );
        }
        
        public void BoundsCheck( Vector2 mousePos ) {
            if (!worldBound.Contains( mousePos ) && ( _filterBounds?.Invoke().Any( ve => !ve?.worldBound.Contains( mousePos ) ?? false ) ?? false ) )
                OnSelected( null );
        }

        public void UpdateLabels() {
            for (int i = 0; i < childCount; i++) 
                (this[i] as ListElement).UpdateLabels();
        }

        // this is a crime
        ListElement CreateElement( string[] lblRaw, Func<SerializedProperty, string>[] updatableLabels, Func<SerializedProperty, string[]> lblCtx, Func<SerializedProperty, SerializedProperty[]> lblProp,
                                   string[] separators, KeyValuePair<string, string>?[] containers, Action<IStyle>[] lblStyles, 
                                   Func<SerializedProperty, Sprite> iconProp, SerializedProperty prop, int index ) =>
                                   new( lblRaw != null ? lblRaw.Concat( lblCtx?.Invoke( prop ) ).ToArray() : lblCtx?.Invoke( prop ), updatableLabels, lblProp?.Invoke( prop ), separators, containers, 
                                        lblStyles, iconProp( prop ), prop, OnSelected, index );

        class ListContext {
            public SerializedProperty TargetArrayProp;
            public string[] RawLabels, LabelSeparators;
            public KeyValuePair<string, string>?[] LabelContainers;
            public Func<SerializedProperty, string[]> ContextLabels;
            public Func<SerializedProperty, string>[] UpdatableLabels;
            public Func<SerializedProperty, SerializedProperty[]> LabelPropertyGetters;
            public Action<IStyle>[] LabelStyles;
            public Func<SerializedProperty, Sprite> IconPropertyGetter;
            public Func<SerializedProperty, SerializedProperty> InternalPropertyGetter;
        }

        public class ListElement : VisualElement {

            public SerializedProperty Property => _prop;
            public int Index => _index;

            readonly Color32 k_selectedColor = new( 0x2C, 0x5D, 0x87, 255 );
            readonly Label[] _labels;
            readonly Image _icon;
            readonly int _index;
            readonly Dictionary<Label, Func<SerializedProperty, string>> _updatableLabelsMap;

            SerializedProperty _prop;
            Color? _startingColor;

            // i will prob go to jail for this
            internal ListElement( string[] rawLbls, Func<SerializedProperty, string>[] updatableLabels, SerializedProperty[] lblProp, string[] separators, KeyValuePair<string, string>?[] containers, Action<IStyle>[] lblStyle, 
                                  Sprite icon, SerializedProperty property, Action<ListElement> onSelected, int index ) {
                _updatableLabelsMap = new();
                _index = index;
                Initialize( property, onSelected );
                _labels = GetPropertyLabels( rawLbls, updatableLabels, lblProp, lblStyle, separators, containers );
                _icon = new() { sprite = icon };
                _icon.style.minHeight = _icon.style.minWidth = _icon.style.maxHeight = _icon.style.maxWidth = 20;
                _icon.style.alignSelf = Align.Center;
                this.AddRange( _icon ).AddRange( _labels );
            }

            internal void SetIconSprite( Sprite spr ) => _icon.sprite = spr;

            Label[] GetPropertyLabels( string[] raw, Func<SerializedProperty, string>[] updatableLabels, SerializedProperty[] lblProp, Action<IStyle>[] lblStyle, string[] separators, KeyValuePair<string, string>?[] containers ) {
                var lbls = new List<Label>();
                int modifierIndex = 0;
                raw ??= Array.Empty<string>();
                lblProp ??= Array.Empty<SerializedProperty>();
                updatableLabels ??= Array.Empty<Func<SerializedProperty, string>>();
                for (int i = 0; i < raw.Length; i++) {
                    int index = i;
                    lbls.AddRange( GetCurrentLabel( () => new Label( raw[ index ] ).WithStyle( TextStyle )
                    , ref modifierIndex, lblStyle, separators, containers ) );
                }
                for (int i = 0; i < lblProp.Length; i++) {
                    int index = i;
                    lbls.AddRange( GetCurrentLabel( () => {
                        var currentLabel = new Label();
                        currentLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                        currentLabel.BindProperty( lblProp[ index ] );
                        return currentLabel;
                    }, ref modifierIndex, lblStyle, separators, containers ) );
                }
                for (int i = 0; i < updatableLabels.Length; i++) {
                    int index = i;
                    var currentLbls = GetCurrentLabel( () => { 
                        var lbl = new Label( updatableLabels[ index ].Invoke( _prop ) ).WithStyle( TextStyle );
                        _updatableLabelsMap.Add( lbl, updatableLabels[ index ] );
                        return lbl;
                    }
                    , ref modifierIndex, lblStyle, separators, containers );
                    lbls.AddRange( currentLbls );
                }
                return lbls.ToArray();
            }

            Label[] GetCurrentLabel( Func<Label> lbl, ref int index, Action<IStyle>[] lblStyle, string[] separators, KeyValuePair<string, string>?[] containers ) {
                var lbls = new List<Label>();
                bool containerValidation = ValidateContainers( containers, index );
                if (containerValidation) lbls.Add( new Label( containers[ index ]?.Key ).WithStyle( TextStyle ) );
                var currentLabel = lbl();
                if (index < lblStyle.Length) lblStyle[ index ]?.Invoke( currentLabel.style ); 
                lbls.Add( currentLabel );
                if (containerValidation) lbls.Add( new Label( containers[ index ]?.Value ).WithStyle( TextStyle ) );
                if (ValidateSeparators( separators, index )) return lbls.ToArray();
                if (!string.IsNullOrEmpty( separators[index] ))
                    lbls.Add( new Label( separators[ index ] ).WithStyle( TextStyle ) );
                index++;
                return lbls.ToArray();
            }

            void TextStyle( IStyle stl ) => stl.unityTextAlign = TextAnchor.MiddleCenter;
            bool ValidateContainers( KeyValuePair<string, string>?[] containers, int index ) => 
                                        containers != null && index < containers.Length && containers[ index ] != null;
            bool ValidateSeparators( string[] separators, int index ) => separators == null || separators.Length < 1 || index >= separators.Length;

            void Initialize( SerializedProperty property, Action<ListElement> onSelected ) {
                _prop = property;
                RegisterCallback<ClickEvent>( ctx => {
                    if (ctx.button != 0) return;
                    onSelected?.Invoke( this );
                } );
                style.flexDirection = FlexDirection.Row;
                style.minHeight = 30;
                style.paddingBottom = style.paddingTop = 2;
                style.overflow = Overflow.Hidden;
            }

            public void Select() {
                _startingColor ??= style.backgroundColor.value;
                style.backgroundColor = new( k_selectedColor );
            }

            public void Deselect() => style.backgroundColor = _startingColor ?? Color.clear;

            public void SetProperty( SerializedProperty prop ) => _prop = prop;

            public void UpdateLabels() {
                foreach (var label in _updatableLabelsMap) 
                    label.Key.text = label.Value.Invoke( _prop );
            }

        }

    }
}
