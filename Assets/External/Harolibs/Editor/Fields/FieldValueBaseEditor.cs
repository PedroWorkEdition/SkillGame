using HaroLibs;
using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {

    [CustomPropertyDrawer( typeof( ValueField<> ), true )]
    public class FieldValueBaseEditor : PropertyDrawer {

        const string k_regexFilter = "<([^>]+)>";

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            var flagProp = property.FindPropertyRelative( "useContainer" );
            var rawProp = property.FindPropertyRelative( "rawValue" );
            var refProp = property.FindPropertyRelative( "container" );
            var rawField = rawProp.CreateField( prop => prop.Q<Label>().SetActive( false ) ).WithStyle( stl => stl.flexGrow = 1 );
            var refField = refProp.CreateField().WithStyle( stl => { stl.flexGrow = 1; stl.paddingRight = 5; stl.paddingLeft = 20; } );
            Button createContainerBtn = null;
            createContainerBtn = new Button( () => CreateContainer( rawProp, refProp, flagProp, UpdateFields ) ) { text = "Create" };
            UpdateFields();
            return new VisualElement() { style = { flexDirection = FlexDirection.Row } }
                                      .AddRange( new Label( property.displayName ) { 
                                                    style = { 
                                                        maxWidth = 85,
                                                        minWidth = 85,
                                                        overflow = Overflow.Hidden,
                                                        textOverflow = TextOverflow.Ellipsis,
                                                        unityTextAlign = TextAnchor.MiddleLeft,
                                                        flexGrow = 1
                                                    } 
                                                 }, 
                                                 VEUtil.CreateButton( "<>", () => {
                                                     flagProp.boolValue = !flagProp.boolValue;
                                                     flagProp.serializedObject.ApplyModifiedProperties();
                                                     UpdateFields();
                                                 } ), rawField, refField, createContainerBtn );
            void UpdateFields() {
                rawField.SetActive( !flagProp.boolValue );
                refField.SetActive( flagProp.boolValue );
                createContainerBtn.SetActive( !flagProp.boolValue );
            }
        }

        void CreateContainer( SerializedProperty rawProp, SerializedProperty refProp, SerializedProperty flagProp, Action updateFields ) {
            var match = Regex.Match( refProp.type, k_regexFilter );
            var name = match.Groups[ 1 ].Value[ 1.. ];
            string path = EditorUtility.SaveFilePanelInProject( $"Create Value Container", "New Value Container", "asset", "Chose a project folder to save the SO" );
            if (string.IsNullOrEmpty( path ))
                return;
            var obj = ContextMenuController.CreateSOInstance( refProp, name );
            ContextMenuController.CreateSOAsset( path, obj );
            refProp.objectReferenceValue = obj;
            flagProp.boolValue = true;
            refProp.serializedObject.ApplyModifiedProperties();
            new SerializedObject(obj).FindProperty( "_value" ).SetValue( rawProp.GetValue(), true );
            updateFields();
        }

    }
}
