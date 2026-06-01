using UltEvents;
using UnityEngine;

namespace SkillGame {

    public class DamageTrigger : MonoBehaviour {

        [SerializeField] Collider source;
        [SerializeField] UltEvent<Damageble, Transform> onHitDamageble;

        private void OnTriggerEnter( Collider other ) {
            if (!other.TryGetComponent<Damageble>( out var damageble )) return;
            onHitDamageble?.Invoke( damageble, source.transform );
        }

    }

}
