using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {
    public class UnityEventProxy : MonoBehaviour {

        [SerializeField] UnityEvent targetEvent;

        public void Raise() => targetEvent?.Invoke();

    }
}
