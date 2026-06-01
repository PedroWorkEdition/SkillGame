using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {
    public sealed class Trigger2DEvents : MonoBehaviour {

        [SerializeField] LayerMask _mask;
        [SerializeField] UnityEvent<Collider2D> onTriggerEnter, onTriggerStay, onTriggerExit;

        private void OnTriggerEnter2D( Collider2D collision ) {
            if (!CheckLayer( collision ))
                return;
            onTriggerEnter?.Invoke( collision );
        }

        private void OnTriggerStay2D( Collider2D collision ) {
            if (!CheckLayer( collision )) 
                return;
            onTriggerStay?.Invoke( collision );
        }
        private void OnTriggerExit2D( Collider2D collision ) {
            if (!CheckLayer( collision )) 
                return;
            onTriggerExit?.Invoke( collision );
        }

        private bool CheckLayer( Collider2D other ) => other.gameObject.IsInLayerMask( _mask );
    }
}
