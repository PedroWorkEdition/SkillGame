using System;
using UnityEngine;

namespace HaroLibs {
    public abstract class ContainerBase<T> : ScriptableObject {

        public abstract T Value { get; set; }

        internal event Action<T> OnValueChanged;

        public static implicit operator T( ContainerBase<T> val ) => val.Value;

        protected void ValueChanged( T val ) => OnValueChanged?.Invoke( val );

        public T GetValue() => Value;

    }

}
