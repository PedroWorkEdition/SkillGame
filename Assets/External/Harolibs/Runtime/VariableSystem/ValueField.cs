using System;
using UnityEngine;

namespace HaroLibs {

    [Serializable]
    public class ValueField<T> : IRawValueProvider<T> {

        [SerializeField] protected T rawValue;
        [SerializeField, SOFieldUtil(SOFieldUtilAttribute.DisplayMode.Create | SOFieldUtilAttribute.DisplayMode.Edit, true)] 
        protected ContainerBase<T> container;
        [SerializeField] bool useContainer;

        public event Action<T> OnValueChanged;

        public T Value { 
            get => useContainer ? ( container ? container.Value : default ) : rawValue;
            set {
                if (useContainer) 
                    container.Value = value;
                else {
                    rawValue = value;
                    OnValueChanged?.Invoke( value );
                }
            }
        }

        public void Initialize() {
            if (!useContainer || !container) return;
            container.OnValueChanged += OnChanged;
        }

        public void Dispose() {
            if (!useContainer || !container) return;
            container.OnValueChanged -= OnChanged;
        }
        void OnChanged( T val ) => OnValueChanged?.Invoke( val );

        public ValueField( T val ) => rawValue = val;
        public ValueField( ValueContainerBase<T> val ) => (useContainer, container) = (val, val);

        public static implicit operator ValueField<T>( T val ) => new ( val );
        public static implicit operator ValueField<T>( ValueContainerBase<T> val ) => new ( val );

        public static implicit operator T( ValueField<T> val ) => val != null ? val.Value : default;

    }
}
