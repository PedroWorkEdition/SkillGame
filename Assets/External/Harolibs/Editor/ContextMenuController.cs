using HaroLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HaroLibsEditor {

    public static class ContextMenuController {

        const string k_assetExtensionName = "asset",
                     k_classKeyWord = "class",
                     k_namespaceKeyWord = "namespace",
                     k_genericToken = "`";

        [MenuItem( "Assets/Create SO Asset", true )]
        public static bool CreateSOValidation() {
            if (Selection.objects.Length == 0) return false;
            var path = AssetDatabase.GetAssetPath( Selection.objects[ 0 ] );
            if (!path.EndsWith( ".cs" )) return false;
            path = path.Replace( "Assets", Application.dataPath );
            using var sr = new StreamReader( path );
            const string soKey = nameof( ScriptableObject );
            var line = sr.ReadLine();
            string nameSpace = string.Empty;
            while (line != null) {
                if (line.Contains( k_namespaceKeyWord ) )
                    nameSpace = line.Replace( k_namespaceKeyWord, string.Empty ).Replace( "{", string.Empty ).Trim();
                if (line.Contains( k_classKeyWord ) && ( line.Contains( soKey ) || IsInheretedClass( line, nameSpace ) ))
                    return true;
                line = sr.ReadLine();
            }
            return false;
        }

        static bool IsInheretedClass( string line, string currentNameSpace ) {
            var splitedLine = line.Split( ':' );
            if (splitedLine.Length < 2) return false;
            var className = splitedLine[ 1 ].Replace( "{", string.Empty ).Replace( "}", string.Empty ).Trim();
            var targetType = $"{currentNameSpace}.{className}".AsType() ?? DeepClassSearch( className );
            return targetType != null && typeof( ScriptableObject ).IsAssignableFrom( targetType );
        }

        static Type DeepClassSearch( string className ) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) 
                foreach (var assemblyType in assembly.GetTypes()) 
                    if ( assemblyType.Name == className )
                        return assemblyType;
            return null;
        }

        [MenuItem( "Assets/Create SO Asset", false )]
        public static void CreateSO() {
            string name = Selection.objects[ 0 ].name;
            string path = EditorUtility.SaveFilePanelInProject( $"Create {name}", name, k_assetExtensionName, "Chose a project folder to save the SO" );
            if (string.IsNullOrEmpty( path ))
                return;
            var obj = ScriptableObject.CreateInstance( name );
            obj.name = name;
            CreateSOAsset( path, obj );
        }

        [MenuItem( "Assets/Create Fixed SO" )]
        public static void CreateFixedSO() {
            var path = AssetDatabase.GetAssetPath( Selection.activeObject );
            var searchWindow = ScriptableObject.CreateInstance<SearchWindowUtil>();
            searchWindow.Initialize<Type>( new( t => CreateTargetFixedSO( path, t ) ) );
            AddUnsortedEntries( searchWindow );
            List<SearchGroupEntry> groups = new() { AddSignals(), AddValueContainers() };
            searchWindow.SetGroups( groups.ToArray() );
            var currentWindow = EditorWindow.focusedWindow;
            SearchWindow.Open( new SearchWindowContext( currentWindow.position.position ), searchWindow );
        }

        static void AddUnsortedEntries( SearchWindowUtil searchWindow ) {
            searchWindow.SetEntries( new SearchEntryContext( 1, "Scene Object", typeof( SceneObject ) ) );
        }

        static SearchGroupEntry AddSignals() {
            var types = AssemblyTypeDefinitions.GetAllTypes( typeof( EventSignal<> ) );
            types.Add( typeof( EmptyEventSignal ) );
            if (types.Count == 0) return null;
            return new SearchGroupEntry( "Event Signals", 1, types.Select( t => new SearchEntryContext( 2, t.Name, t ) ).ToArray() );
        }

        static SearchGroupEntry AddValueContainers() {
            var types = AssemblyTypeDefinitions.GetAllTypes( typeof( ValueContainerBase<> ) ).Where( type => !type.IsAbstract ).ToList();
            if (types.Count == 0) return null;
            return new SearchGroupEntry( "Value Containers", 1, types.Select( t => new SearchEntryContext( 2, t.Name, t ) ).ToArray() );
        }

        static void CreateTargetFixedSO( string path, Type type ) {
            var obj = ScriptableObject.CreateInstance( type );
            CreateSOAsset( $"{path}/{type.Name}.asset", obj );
        }

        // TODO: move to a utility script
        internal static void CreateSOAsset( string path, ScriptableObject obj ) { 
            AssetDatabase.CreateAsset( obj, path );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject( obj );
            Debug.Log( $"Created an SO of {obj.name} at {path}" );
        }

        internal static ScriptableObject CreateSOInstance( SerializedProperty prop, string name ) {
            if (!name.Contains( k_genericToken )) return ScriptableObject.CreateInstance( name );
            var type = prop.GetSerializedType();
            var validType = AssemblyTypeDefinitions.GetAllTypes( type ).FirstOrDefault( t => !t.IsAbstract );
            return validType != null ? ScriptableObject.CreateInstance( validType ) : null;
        }

    }

}
