using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public abstract class EventSignalListenerBase<T> : MonoBehaviour {

        [SerializeField] protected EventSignal<T> targetSignal;
        [SerializeField] protected UltEvent<T> targetEvent;

        private void Awake() => targetSignal.Register( targetEvent.Invoke );
        private void OnDestroy() => targetSignal.Unregister( targetEvent.Invoke );

    }
}
