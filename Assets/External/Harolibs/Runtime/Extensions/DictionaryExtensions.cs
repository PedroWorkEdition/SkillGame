using System.Collections.Generic;

namespace HaroLibsEditor {
	
	public static class DictionaryExtensions {

        public static void Add<K,V>( this Dictionary<K,V> target, KeyValuePair<K,V> val ) => target.Add( val.Key, val.Value );

    }
	
}