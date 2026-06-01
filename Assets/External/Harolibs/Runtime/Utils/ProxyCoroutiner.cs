using System.Collections;
using UnityEngine;

namespace HaroLibs {
    public class ProxyCoroutiner : MonoBehaviour {

        static ProxyCoroutiner _instance;
        static ProxyCoroutiner Instance { 
            get { 
                if (!_instance)
                    CreateInstance();
                return _instance; 
            } 
        }

        private void Awake() => _instance = this;
        
        static void CreateInstance() {
            var go = new GameObject( nameof( ProxyCoroutiner ) );
            go.AddComponent<ProxyCoroutiner>();
        }

        public static CoroutineData RunCoroutine( IEnumerator enumerator ) => Instance.StartCoroutine( enumerator );
        public static void StopTargetCoroutine( CoroutineData target ) => Instance.StopCoroutine( target.GetContent() );
    }

    public record CoroutineData {

        readonly Coroutine _target;

        public CoroutineData( Coroutine target ) => _target = target;

        internal Coroutine GetContent() => _target;

        public static implicit operator CoroutineData( Coroutine target ) => new ( target );

    }
}
