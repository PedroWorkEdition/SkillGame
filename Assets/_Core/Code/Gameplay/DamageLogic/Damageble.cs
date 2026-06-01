using UltEvents;
using UnityEngine;

namespace SkillGame {

    public class Damageble : MonoBehaviour {

        [SerializeField] UltEvent onDamaged;

        protected Character character;
        internal Character Victim => character;

        public virtual void TakeDamage( DamageContext ctx ) => onDamaged?.Invoke();

    }

}
