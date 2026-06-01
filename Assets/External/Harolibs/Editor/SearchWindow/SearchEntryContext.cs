using System;

namespace HaroLibsEditor {
    /// <summary>
    /// The type T must have a empty constructor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchEntryContext<T> : SearchEntryContext where T : new() {
        public SearchEntryContext( int level, string name ) : base( level, name, typeof(T) ) { }
    }

    public class SearchEntryContext : ISearchEntryUtil {
        readonly object _obj;
        readonly int _level;
        readonly string _name;
        public int Level => _level;
        public string Name => _name;
        /// <param name="level">Starts at 1, 0 will be considered as 1</param>
        /// <param name="userObj">Return object, must be the same type as the initialized type</param>
        public SearchEntryContext( int level, string name, object userObj ) {
            _level = level;
            _obj = userObj;
            _name = name;
        }
        public object GetUserData() => _obj;
        public Type DataType => _obj.GetType();
    }
}
