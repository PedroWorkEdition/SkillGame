using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {
    public class UnityEventEmmiter : MonoBehaviour {

        [SerializeField] UnityEvent emmiter;

        public void Emmit() => emmiter?.Invoke();

    }
}
