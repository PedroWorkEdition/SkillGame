using System;
using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {

    [Flags]
    public enum TriggerFilter { 
        None = 0,
        Tag = 1,
        Layer = 2,
        All = 3
    }
    public class TriggerEventsEmmiter : MonoBehaviour {

        [SerializeField] TriggerFilter filter;
        [SerializeField, TagView] string targetTag;
        [SerializeField] LayerMask layerMask = int.MaxValue;
        [SerializeField] UnityEvent<GameObject> onTriggerEnter, onTriggerStay, onTriggerExit;

        private void OnTriggerEnter( Collider other ) => CallEvent( onTriggerEnter, other );
        private void OnTriggerStay( Collider other ) => CallEvent( onTriggerStay, other );
        private void OnTriggerExit( Collider other ) => CallEvent( onTriggerExit, other );

        void CallEvent( UnityEvent<GameObject> target, Collider other ) {
            if (( filter & TriggerFilter.Tag ) > 0 && !other.CompareTag( tag )) return;
            if (( filter & TriggerFilter.Layer ) > 0 && (layerMask.value & ( 1 << other.transform.gameObject.layer )) == 0) return;
            target?.Invoke( other.transform.gameObject );
        }
    }
}
