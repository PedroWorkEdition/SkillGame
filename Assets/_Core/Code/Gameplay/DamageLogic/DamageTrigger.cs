using UltEvents;
using UnityEngine;

namespace SkillGame {

    public class DamageTrigger : MonoBehaviour {

        [SerializeField] Collider2D source;
        [SerializeField] UltEvent<Damageble, Transform> onHitDamageble;

        private void OnTriggerEnter2D( Collider2D other ) {
            if (!other.TryGetComponent<Damageble>( out var damageble )) return;
            onHitDamageble?.Invoke( damageble, source.transform );
        }

    }

}
