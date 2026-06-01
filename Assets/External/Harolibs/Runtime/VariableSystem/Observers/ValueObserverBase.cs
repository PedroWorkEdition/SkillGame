using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public abstract class ValueObserverBase<T> : MonoBehaviour {

        [SerializeField] protected ValueField<T> targetValue;
        [SerializeField] protected UltEvent<T> onChanged;
        [SerializeField] protected bool manual, onEnable;

        private void OnEnable() {
            targetValue.Initialize();
            if (!manual)
                targetValue.OnValueChanged += Invoke;
            if (onEnable) Invoke();
        }

        private void OnDisable() {
            targetValue.Dispose();
            if (!manual)
                targetValue.OnValueChanged -= Invoke;
        }

        public void Invoke() => Invoke( targetValue );
        protected virtual void Invoke( T val ) => onChanged?.Invoke( val );
        public void SetValue( T val ) => targetValue.Value = val;
        public void SetValue( ValueContainerBase<T> val ) => targetValue.Value = val;
        public void SetValue( ValueObserverBase<T> val ) => targetValue.Value = val.targetValue;
        public T GetValue() => targetValue.Value;

    }

}
