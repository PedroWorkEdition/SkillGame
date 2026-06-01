using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public class OnEnableDisableEmitter : MonoBehaviour {

        [SerializeField] UltEvent onEnable, onDisable;

        private void OnEnable() => onEnable?.Invoke();
        private void OnDisable() => onDisable?.Invoke();

    }

}
