using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public abstract class ValueStorageBase<T> : MonoBehaviour {

        [SerializeField] UltEvent<T> onValueRegistered, onValueEmit;

        T _value;

        protected T Value { 
            get => _value; 
            set {
                onValueRegistered.Invoke( value );
                _value = value;
            }
        }

        public void SetValue( T val ) => Value = val;
        public void SetValue( ValueContainerBase<T> val ) => Value = val;
        public void SetValue( ValueObserverBase<T> val ) => Value = val.GetValue();
        public void EmmitValue() => onValueEmit?.Invoke( Value );

    }
}
