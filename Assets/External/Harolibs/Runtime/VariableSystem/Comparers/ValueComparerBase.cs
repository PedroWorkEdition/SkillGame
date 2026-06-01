using System;
using UltEvents;
using UnityEngine;

namespace HaroLibs {

    public enum CompareType { Equal, NotEqual, GreaterThan, LowerThan, GreaterOrEqualThan, LowerOrEqualThan }

    public abstract class ValueComparerBase<T> : MonoBehaviour where T : IComparable {

        [SerializeField] protected ValueField<T> origin, compare;

        [SerializeField] UltEvent then, @else;

        public void Check() => ( Validate() ? then : @else )?.Invoke();
        public void Check( T val ) {
            SetValue( val );
            Check();
        }

        public void Check( ContainerBase<T> val ) {
            SetValue( val );
            Check();
        }

        protected virtual bool Validate() => origin.Value.CompareTo( compare.Value ) == 0;

        public void SetValue( T val ) => origin.Value = val;
        public void SetValue( ContainerBase<T> val ) => origin.Value = val.Value;

    }
}
