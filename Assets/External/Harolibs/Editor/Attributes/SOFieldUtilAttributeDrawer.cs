using HaroLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {

    [CustomPropertyDrawer( typeof( SOFieldUtilAttribute ) )]
    public class SOFieldUtilAttributeDrawer : PropertyDrawer {

        const string k_assetExtensionName = "asset",
                     k_regexFilter = "<([^>]+)>",
                     k_scriptToken = "Script";

        readonly string r_prefsKey = $"{nameof( SOFieldUtilAttributeDrawer )}_FoldoutValue";

        Action<UnityEngine.Object> _onValueChanged;
        bool _isEdit, _removeLabel;
        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            var att = attribute as SOFieldUtilAttribute;
            _removeLabel = att.RemoveLabel;
            _isEdit = ( att.Display & SOFieldUtilAttribute.DisplayMode.Edit ) > 0;
            var controlContainer = new VisualElement() { style = { flexDirection = FlexDirection.Row, flexGrow = 1 } }
                                                      .AddRange( AddFieldControls( property, att.Display ) );
            if (_isEdit)
                return SOEdit( property, controlContainer );
            return controlContainer;
        }

        VisualElement AddFieldControls( SerializedProperty prop, SOFieldUtilAttribute.DisplayMode displayMode ) {
            var elements = new List<VisualElement>();
            if (( displayMode & SOFieldUtilAttribute.DisplayMode.Properties ) > 0) elements.Add( CreatePropertiesBtn( prop ) );
            if (( displayMode & SOFieldUtilAttribute.DisplayMode.Create ) > 0) elements.Add( CreateItemBtn( prop ) );
            return MainObjectField( prop, elements );
        }

        VisualElement MainObjectField( SerializedProperty prop, List<VisualElement> elements ) =>
            VEUtil.CreatePropField( prop, field => {
                var objField = field.Q<ObjectField>();
                if (_removeLabel)
                    objField.labelElement.SetActive( false );
                var contentElement = objField[ 1 ];
                if (_isEdit)
                    contentElement.Q<Image>().style.paddingRight = 4;
                foreach (var element in elements)
                    contentElement.Insert( 1, element );
                objField.RegisterValueChangedCallback( ctx => _onValueChanged?.Invoke( ctx.newValue ) );
            } ).WithStyle( stl => { stl.flexGrow = 1; stl.marginRight = 5; } );

        Button CreateItemBtn( SerializedProperty prop ) => new( () => TryCreateObject( prop ) ) {
            text = "+",
            style = {
                marginBottom = 0,
                marginTop = 0,
                paddingTop = 0,
                paddingBottom = 0,
                borderBottomWidth = 0,
                borderTopWidth = 0,
                paddingLeft = 6
            }
        };

        void TryCreateObject( SerializedProperty prop ) {
            var match = Regex.Match( prop.type, k_regexFilter );
            var name = match.Groups[ 1 ].Value[ 1.. ];
            var type = prop.GetSerializedType();
            var validTypes = AssemblyTypeDefinitions.GetAllTypes( type ).Where( t => !t.IsAbstract ).ToArray();
            if (validTypes.Length > 1) {
                var menu = new GenericMenu();
                foreach ( var validType in validTypes ) 
                    menu.AddItem( new UnityEngine.GUIContent( validType.Name ), false, () => CreateObject( prop, validType.Name) );
                menu.ShowAsContext();
                return;
            }
            // make so there is a list of inhereted objects if the target scriptable object cannot be instantiated
            CreateObject( prop, name );
        }

        void CreateObject( SerializedProperty prop, string name ) {
            string path = EditorUtility.SaveFilePanelInProject( $"Create {name}", name, k_assetExtensionName, "Chose a project folder to save the SO" );
            if (string.IsNullOrEmpty( path ))
                return;
            var obj = ContextMenuController.CreateSOInstance( prop, name );
            ContextMenuController.CreateSOAsset( path, obj );
            prop.objectReferenceValue = obj;
            prop.serializedObject.ApplyModifiedProperties();
        }

        Button CreatePropertiesBtn( SerializedProperty prop ) => new( () => OpenProperties( prop ) ) {
            text = "P",
            style = {
                marginBottom = 0,
                marginTop = 0,
                paddingTop = 0,
                paddingBottom = 0,
                borderBottomWidth = 0,
                borderTopWidth = 0,
                paddingLeft = 6,
            }
        };

        Foldout SOEdit( SerializedProperty prop, VisualElement controlsContainer ) {
            var foldout = new Foldout() { text = "Data", value = EditorPrefs.GetBool( r_prefsKey, false ) };
            var lbl = foldout.Q<Label>();
            lbl.SetActive( false );
            lbl.parent.style.flexShrink = 1;
            lbl.parent.Add( controlsContainer );
            foldout.RegisterValueChangedCallback( ctx => EditorPrefs.SetBool( r_prefsKey, false ) );
            _onValueChanged += obj => { foldout?.Clear(); foldout.Add( CreateObjectContainer( obj ) ); };
            _onValueChanged( prop.objectReferenceValue );
            foldout.RegisterCallback<GeometryChangedEvent>( UpdateFoldoutParent );
            return foldout;
            void UpdateFoldoutParent( GeometryChangedEvent ctx ) {
                foldout.parent.style.flexGrow = 1;
                foldout.UnregisterCallback<GeometryChangedEvent>( UpdateFoldoutParent );
            }
        }

        VisualElement CreateObjectContainer( UnityEngine.Object obj ) {
            var container = new VisualElement();
            if (!obj)
                return container;
            var iterator = new SerializedObject( obj ).GetIterator();
            iterator.NextVisible( true );
            do {
                if (iterator.displayName != k_scriptToken)
                    container.Add( VEUtil.CreatePropField( iterator ) );
            }
            while (iterator.NextVisible( false ));
            return container;
        }

        void OpenProperties( SerializedProperty prop ) {
            if (!prop.objectReferenceValue) return;
            EditorUtility.OpenPropertyEditor( prop.objectReferenceValue );
        }
    }
}
