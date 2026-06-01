using UnityEngine;

namespace HaroLibs {
    public abstract class PrivateSingleton<T> : MonoBehaviour where T : class {

        static T _instance;

        protected static T Instance {
            get {
                
                return _instance;
            }
        }

        public static bool IsAvailable => Instance != null;

        protected virtual void Awake() => _instance = this as T;

        protected virtual void OnDestroy() {
            if (Instance == ( this as T ))
                _instance = null;
        }

    }

}
