using UnityEngine;

namespace HaroLibs {
    public abstract class PrivateLazySingleton<T> : MonoBehaviour where T : Component {

        static T _instance;

        protected static T Instance {
            get {
                if (!_instance) {
                    var go = new GameObject( typeof( T ).Name );
                    return go.AddComponent<T>();
                }
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
