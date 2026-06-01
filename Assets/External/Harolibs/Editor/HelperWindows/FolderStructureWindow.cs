using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace HaroLibsEditor {
    public class FolderStructureWindow : EditorWindow {

        static FolderData _root;
        VisualElement _scrollView;


        [MenuItem( "HaroLibs/Create Folder Structure" )]
        public static void ShowFolderStructureWindow() {
            var window = GetWindow<FolderStructureWindow>();
            window.titleContent = new( "Folder Structure" );
            window.minSize = new( 400, 600 );
        }

        private void OnEnable() {
            CreateBaseStructure();
            rootVisualElement.AddRange( _scrollView = CreateVisualStructure(), new Button( CreateActualFolders ) { text = "Create Folders" } );
        }

        VisualElement CreateVisualStructure() {
            // TODO: add/remove folder button, 'create folders' button
            return new ScrollView( ScrollViewMode.Vertical ).AddRange( FolderElement( _root ) ); 
        }

        VisualElement FolderElement( FolderData data, string[] parent = null ) {
            parent ??= new string[0];
            var container = new Foldout() { value = parent.Length < 2 };
            container.style.backgroundColor = new( new Color32( 100, 100, 100, 255 ) );
            container.SetBorder( 1, 10, Color.black );
            TextField nameField = new() { value = data.Name };
            data.Data ??= new();
            var tgl = container.Q<Toggle>();
            tgl.Add( nameField );
            tgl[ 0 ].style.flexGrow = 0;
            tgl[ 0 ].style.display = data.Data != null && data.Data.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            nameField.style.flexGrow = 1;
            nameField.style.marginRight = 10;
            nameField.Q<TextElement>().style.paddingLeft = 3;
            ArrayUtility.Add( ref parent, data.Name );
            tgl.AddRange( 
                new Button( () => AddButton( parent[1..] ) ) { text = "Add Folder" }.WithStyle( st => { st.flexGrow = .25f; st.marginRight = 10; } ),
                parent.Length > 1 ? new Button( () => RemoveButton( parent[1..] ) ) { text = "Delete" }.WithStyle( st => { st.flexGrow = .25f; st.marginRight = 10; } ) : new() );
            return container.AddRange( data.Data.Select( d => FolderElement( d, parent ) ).ToArray() );
        }

        static void CreateBaseStructure() {
            _root = new( "_Core", new() {
                new ( "Art", new() {
                    new( "Sprites" ),
                    new( "Textures" ),
                } ),
                new ( "Audio", new() {
                    new ( "Music" ),
                    new ( "SFX" ),
                } ),
                new ( "Code", new() {
                    new ( "Editor" ),
                    new ( "Data" ),
                    new ( "UI" ),
                    new ( "Gameplay" ),
                    new ( "Utils" ),
                } ),
                new ( "Data" ),
                new ( "Models" ),
                new ( "Prefabs", new() {
                    new( "Entities" ),
                    new( "Objects" ),
                    new( "UI" ),
                } ),
                new ( "Presets" ),
                new ( "Scene", new() {
                    new( "Assets" ),
                    new( "Levels" ),
                } ),
                new ( "Settings" ),
            } );
        }

        void AddButton( string[] path ) {
            var current = GetFolder( path );
            current.Add( $"{current.Name} new folder" );
            UpdateList();
        }

        void RemoveButton( string[] path ) {
            var current = GetFolder( path[..^1] );
            current.Remove( path[ ^1 ] );
            UpdateList();
        }

        void UpdateList() {
            _scrollView.contentContainer.Clear();
            _scrollView.AddRange( FolderElement( _root ) );
        }

        FolderData GetFolder( string[] path ) {
            var current = _root;
            for (int i = 0; i < path.Length; i++) 
                current = current[ path[ i ] ];
            return current;
        }

        void CreateActualFolders() {
            var paths = new List<string>();
            GetAvailablePath( _root, null, paths );
            foreach ( string path in paths ) CreateFolder( path );
        }

        void GetAvailablePath( FolderData current, string currentPath = null, List<string> paths = null ) {
            paths ??= new();
            currentPath ??= "";
            currentPath += $"/{current.Name}";
            if( current.Data == null || current.Data.Count < 1) {
                paths.Add( currentPath );
                return;
            }
            foreach ( var folder in current.Data )
                GetAvailablePath( folder, currentPath, paths );
        }

        void CreateFolder( string path ) {
            var fullPath = $"{Application.dataPath}{path}";
            Directory.CreateDirectory( fullPath );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        class FolderData {

            public string Name;
            public List<FolderData> Data;

            public FolderData( string name, List<FolderData> data = null ) => (Name, Data) = (name, data);

            public void Add( string name ) {
                Data ??= new();
                Data.Add( new( name ) );
            }

            public void Remove( string name ) {
                if (Data == null) return;
                Data.Remove( this[ name ] );
            }

            public FolderData this[ string name ] => Data.FirstOrDefault( x => x.Name == name );

        }

    }
}
