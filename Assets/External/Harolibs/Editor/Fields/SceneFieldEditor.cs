using HaroLibs;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    [CustomPropertyDrawer( typeof( SceneField ) )]
    public class SceneFieldEditor : PropertyDrawer {

        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            var container = new VisualElement();
            var sceneProp = property.FindPropertyRelative( "sceneName" );
            var guidProp = property.FindPropertyRelative( "sceneGUID" );
            SceneAsset loadedScene = LoadCurrentValue( AssetDatabase.FindAssets( "t:scene" ), sceneProp, guidProp.stringValue );
            if (loadedScene)
                UpdateSerializedData( sceneProp, guidProp, loadedScene );
            container.Add( CreateField( sceneProp, guidProp, loadedScene ) );
            return container;
        }

        void UpdateSerializedData( SerializedProperty sceneProp, SerializedProperty guidProp, SceneAsset loadedScene ) {
            if (!string.IsNullOrEmpty( guidProp.stringValue )) return;
            guidProp.stringValue = AssetDatabase.GUIDFromAssetPath( AssetDatabase.GetAssetPath( loadedScene ) ).ToString();
            guidProp.serializedObject.ApplyModifiedProperties();
            if (sceneProp.stringValue == loadedScene.name) return;
            sceneProp.stringValue = loadedScene.name;
            sceneProp.serializedObject.ApplyModifiedProperties();
        }

        SceneAsset LoadCurrentValue( string[] scenesGUID, SerializedProperty property, string guid ) {
            foreach (var scene in scenesGUID) {
                var target = AssetDatabase.LoadAssetAtPath<SceneAsset>( AssetDatabase.GUIDToAssetPath( scene ) );
                if (target.name == property.stringValue || scene == guid)
                    return target;
            }
            return null;
        }

        ObjectField CreateField( SerializedProperty sceneProp, SerializedProperty guidProp, SceneAsset loadedScene ) {
            var field = new ObjectField( "Scene" ) {
                objectType = typeof( SceneAsset ),
                value = loadedScene
            };
            field.labelElement.style.width = 137;
            field.RegisterCallback<ChangeEvent<Object>>( ctx => {
                sceneProp.stringValue = ctx.newValue ? ctx.newValue.name : null;
                guidProp.stringValue = ctx.newValue ? AssetDatabase.GUIDFromAssetPath( AssetDatabase.GetAssetPath( ctx.newValue ) ).ToString() : null;
                sceneProp.serializedObject.ApplyModifiedProperties();
            } );
            return field;
        }
    }
}
