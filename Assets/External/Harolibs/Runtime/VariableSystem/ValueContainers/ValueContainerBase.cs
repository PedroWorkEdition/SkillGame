using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public abstract class ValueContainerBase<T> : ContainerBase<T> {

        [SerializeField] protected T _value;
        [SerializeField] protected UltEvent<T> onChanged;

        public override T Value { get => _value; set { _value = value; ValueChanged( _value ); onChanged?.Invoke( _value ); } }

        public void SetValue( T val ) => Value = val;
        public void SetValue( ValueContainerBase<T> other ) => Value = other.Value;

    }
}
