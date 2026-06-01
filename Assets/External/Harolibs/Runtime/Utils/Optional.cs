using UnityEngine;

namespace HaroLibs {
    public class Optional<T> where T : class {// WIP

        readonly T _value;

        public Optional( T val ) => _value = val;

        public T GetValue( T defaultValue ) => _value ?? defaultValue;

        public static implicit operator T( Optional<T> optional ) => optional._value;

    }
}
