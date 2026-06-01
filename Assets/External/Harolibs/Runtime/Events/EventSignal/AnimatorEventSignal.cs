using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {
    public class AnimatorEventSignal : MonoBehaviour {

        [SerializeField] UnityEvent[] evts;

        public void CallEvent( int index ) {
            if (index < 0 && index >= evts.Length) return;
            //DLogger.Log( $"index: {index}, size: {evts.Length}" );
            evts[index].Invoke();
        }

    }
}
