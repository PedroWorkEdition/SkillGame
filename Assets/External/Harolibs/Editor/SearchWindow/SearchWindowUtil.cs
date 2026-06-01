using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    public class SearchWindowUtil : ScriptableObject, ISearchWindowProvider {

        IAnomActionProvider _onEntrySelected, _overrideAction;
        Texture2D _identationIcon;

        ISearchGroupUtil[] _groups;
        ISearchEntryUtil[] _entries;
        string _labelText = "Search";

        public void Initialize<T>( AnomAction<T> selectedEntry ) {
            _onEntrySelected = selectedEntry;
            _identationIcon = new Texture2D( 1, 1 );
            _identationIcon.SetPixel( 0, 0, Color.clear );
            _identationIcon.Apply();
        }

        public void SetEntries( params SearchEntryContext[] entries ) => _entries = Array.ConvertAll( entries, item => (ISearchEntryUtil)item );
        public void SetGroups( params SearchGroupEntry[] groups ) => _groups = Array.ConvertAll( groups, item => (ISearchGroupUtil)item );
        public bool Open( Vector2 pos, string label = null ) {
            _labelText = label ?? _labelText;
            return SearchWindow.Open( new SearchWindowContext( pos ), this );
        }

        public bool Open( VisualElement originController, EditorWindow window, string label = null ) =>
                    Open( originController.worldBound.position + window.position.position, label );

        public bool Open<T>( VisualElement originController, EditorWindow window, string label = null, AnomAction<T> overrideAction = null ) {
            _overrideAction = overrideAction;
            return Open( originController.worldBound.position + window.position.position, label );
        }

        public List<SearchTreeEntry> CreateSearchTree( SearchWindowContext context ) {
            var entries = new List<SearchTreeEntry> { new SearchTreeGroupEntry( new GUIContent( _labelText ), 0 ) };
            GenerateGroups( entries );
            GenerateEntries( entries );
            return entries;
        }

        void GenerateGroups( List<SearchTreeEntry> entries ) {
            if (_groups == null) return;
            foreach (var group in _groups) {
                entries.Add( new SearchTreeGroupEntry( new GUIContent( group.Name ), group.Level ) );
                AddGroupEntry( group, entries );
            }
        }

        void GenerateEntries( List<SearchTreeEntry> entries ) {
            if (_entries == null) return;
            foreach (var entry in _entries) 
                entries.Add( new SearchTreeEntry( new GUIContent( entry.Name, _identationIcon ) ) { level = entry.Level, userData = entry.GetUserData() } );
        }

        void AddGroupEntry( ISearchGroupUtil group, List<SearchTreeEntry> target ) {
            var entries = group.GetSearchEntries();
            foreach (var entry in entries ) 
                target.Add( new SearchTreeEntry( new GUIContent( entry.Name, _identationIcon ) ) { level = entry.Level, userData = entry.GetUserData() } );
        }

        public bool OnSelectEntry( SearchTreeEntry SearchTreeEntry, SearchWindowContext context ) {
            if (_overrideAction != null) {
                _overrideAction.Call( SearchTreeEntry.userData );
                _overrideAction = null;
            } else
                _onEntrySelected?.Call( SearchTreeEntry.userData );
            return true;
        }
    }
}
