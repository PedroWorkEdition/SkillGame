using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public class OnAwakeEmitter : MonoBehaviour {

        [SerializeField] UltEvent onAwake;
        private void Awake() => onAwake?.Invoke();
    }

}
