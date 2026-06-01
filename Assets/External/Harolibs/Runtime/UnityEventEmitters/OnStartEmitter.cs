using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {
    public class OnStartEmitter : MonoBehaviour {
        [SerializeField] UnityEvent onStart;
        private void Start() => onStart?.Invoke();
    }

}
