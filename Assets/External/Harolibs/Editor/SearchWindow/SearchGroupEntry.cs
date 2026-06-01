using System;

namespace HaroLibsEditor {
    public class SearchGroupEntry : ISearchGroupUtil {
        readonly string _name;
        public string Name => _name;

        readonly int _level;
        public int Level => _level;

        public SearchEntryContext[] Entries;
        public SearchGroupEntry( string name, int level, params SearchEntryContext[] entries ) =>
                               (_name, _level, Entries) = (name, level, entries);
        
        public ISearchEntryUtil[] GetSearchEntries() => Array.ConvertAll( Entries, item => (ISearchEntryUtil)item );
    }
}
