using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    public class VEList : VisualElement {

        static bool _pointsFoldout;

        const int BOX_RADIUS = 10;

        SerializedProperty _target, _selectedProp;
        int _propertyIndex = -1, _persistentIndex = -1;
        int PropertyIndex {
            get => _propertyIndex; set {
                _propertyIndex = value;
                if (value >= 0)
                    _selectedProp = _target.GetArrayElementAtIndex( _propertyIndex );
            }
        }
        public SerializedProperty SelectedProperty => _selectedProp;

        string PointsFoldoutKey => _target.serializedObject.targetObject.GetType().ToString() + nameof( _pointsFoldout );

        string _label;
        readonly Func<SerializedProperty, VisualElement> _elementGen;
        readonly Action _onAdd, _onRemove;
        Action _forceUpdate;

        public VEList( SerializedProperty target, string label, Func<SerializedProperty, VisualElement> elementGen, Action onAdd = null, Action onRemove = null ) {
            name = nameof( VEList );
            _target = target;
            _label = label;
            _elementGen = elementGen;
            _onAdd = onAdd;
            _onRemove = onRemove;
            Add( ListElement( target ) );
        }

        VisualElement ListElement( SerializedProperty property ) {
            var container = new VisualElement();
            var box = new Box();
            var contentContainer = new VisualElement().AddRange( box, ButtonsContainer( InitializeContainer ) );
            box.style.borderBottomLeftRadius = box.style.borderTopRightRadius = box.style.borderTopLeftRadius = BOX_RADIUS;
            box.style.flexGrow = 1;
            box.style.minHeight = 25;
            box.style.paddingBottom = box.style.paddingTop = 10;
            box.style.paddingLeft = box.style.paddingRight = 5;
            void InitializeContainer() {
                box.Clear();
                for (int i = 0; i < property.arraySize; i++) {
                    int index = i;
                    box.Add( TargetElement( property.GetArrayElementAtIndex( i ), index ) );
                }
            }
            _forceUpdate = InitializeContainer;
            InitializeContainer();
            return container.AddRange( PointsHeader( contentContainer ) );
        }

        VisualElement ButtonsContainer( Action onClick ) {
            var container = new Box();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignSelf = Align.FlexEnd;
            container.style.paddingLeft = container.style.paddingRight = 2;
            container.style.paddingBottom = 4;
            container.style.borderBottomLeftRadius = container.style.borderBottomRightRadius = BOX_RADIUS;
            container.AddRange( VEUtil.CreateButton( "+", () => { AddPoint(); onClick?.Invoke(); } ), VEUtil.CreateButton( "-", () => { RemovePoint(); onClick?.Invoke(); } ) );
            return container;
        }

        VisualElement PointsHeader( VisualElement content ) {
            content.style.flexGrow = 1;
            var container = new VisualElement();
            container.style.flexGrow = 1;
            var fold = new Foldout() { value = _pointsFoldout };
            fold.Q<Toggle>().Add( new Label( _label) );
            fold.Q<Toggle>()[ 0 ].style.flexGrow = 0;
            fold.contentContainer.style.flexGrow = 1;
            fold.style.marginBottom = 10;
            fold.RegisterCallback<ChangeEvent<bool>>( ctx => EditorPrefs.SetBool( PointsFoldoutKey, _pointsFoldout = ctx.newValue ) );
            return container.AddRange( fold.AddRange( content ) );
        }

        VisualElement TargetElement( SerializedProperty property, int index ) {
            var container = new VisualElement();
            container.RegisterCallback<FocusInEvent, VisualElement>( ( _, el ) => SelectPoint( index, el ), container );
            container.RegisterCallback<FocusOutEvent, VisualElement>( ( _, el ) => SelectPoint( -1, el ), container );
            return container.AddRange( _elementGen.Invoke( property ) );
        }

        void AddPoint() { 
            if( _onAdd != null) {
                _onAdd();
                return;
            }
            _target.InsertArrayElementAtIndex( _target.arraySize );
            _target.serializedObject.ApplyModifiedProperties(); 
        }
        void RemovePoint() {
            if (_onRemove != null) {
                _onRemove();
                return;
            }
            _target.DeleteArrayElementAtIndex( _persistentIndex < 0 ? _target.arraySize - 1 : _persistentIndex );
            _persistentIndex = -1;
            _target.serializedObject.ApplyModifiedProperties();
        }

        public void ForceRepait() => _forceUpdate();

        void SelectPoint( int index, VisualElement target ) {
            PropertyIndex = index;
            bool cond = index >= 0;
            if (cond) _persistentIndex = index;
            target.style.backgroundColor = new( cond ? new Color32( 90, 150, 250, 100 ) : Color.clear );
            SceneView.RepaintAll();
        }
    }
}
