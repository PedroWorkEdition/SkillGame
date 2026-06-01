using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public class BoolValueObserver : ValueObserverBase<bool> {

        [SerializeField] bool invert;
        [SerializeField] UltEvent onTrue, onFalse;

        protected override void Invoke( bool val ) {
            if (invert) val = !val;
            base.Invoke( val );
            if (val) onTrue?.Invoke();
            else onFalse?.Invoke();
        }

        public void Toogle() => targetValue.Value = !targetValue;

    }

}
