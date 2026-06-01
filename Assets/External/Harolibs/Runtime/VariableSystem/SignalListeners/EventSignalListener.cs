using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public class EventSignalListener : MonoBehaviour {

        [SerializeField] EmptyEventSignal targetSignal;
        [SerializeField] UltEvent targetEvent;

        private void Awake() => targetSignal.Register( targetEvent.Invoke );
        private void OnDestroy() => targetSignal.Unregister( targetEvent.Invoke );

    }
}
