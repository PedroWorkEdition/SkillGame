using System.Linq;
using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public class AnimationEvents : MonoBehaviour {

        [SerializeField] AnimationEvent[] events;

        public void CallEvent( string evtName ) {
            var current = events.FirstOrDefault( evt => evt.Name == evtName );
            if (string.IsNullOrEmpty( current.Name )) { Debug.Log( $"Animation event: '{evtName}' was not registered" ); return; }
            current.Event?.Invoke();
        }

        [System.Serializable]
        public struct AnimationEvent {
            public string Name;
            public UltEvent Event;
        }

    }
}
