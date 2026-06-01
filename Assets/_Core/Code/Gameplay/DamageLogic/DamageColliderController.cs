using SkillGame.Data;
using System;
using UnityEngine;

namespace SkillGame {

    public class DamageColliderController : MonoBehaviour {

        [SerializeField] Collider targetCollider;
        [SerializeField] bool customAttackData;
        [SerializeField] int damage;
        [SerializeField] bool useKnockback;
        [SerializeField] float knockbackStrength;

        Character _perpetrator;
        AttackData? _data;

        internal void Initilize( Character character ) => _perpetrator = character;

        public void DamagebleHit( Damageble damageble, Transform source ) {
            if (!customAttackData && !_data.HasValue) return;
            if (customAttackData) _data = new AttackData() { DamageMultiplier = 1 };
            DamageContext ctx = _perpetrator ? _perpetrator.DamageOther( damageble.Victim, source, _data.Value ) : CreateCustomContext( damageble, source );
            var dmg = Mathf.RoundToInt( ctx.Data.Damage * _data.Value.DamageMultiplier );
            ctx.DamageDealt = ctx.Victim ? Mathf.Max( dmg - ctx.Victim.Stats.Defense, 1 ) : dmg;
            damageble.TakeDamage( ctx );
            ctx.Data.OnHit?.Invoke( ctx );
            _data = null;
        }

        DamageContext CreateCustomContext( Damageble damageble, Transform source ) =>
            new ( _perpetrator, damageble.Victim, source, new AttackDamageData() {
                RawDamage = damage,
                Multiplier = 1,
                UseKnockBack = useKnockback,
                KnockBackStrength = knockbackStrength
            } );

        internal void SetColliderEnable( bool enable ) => targetCollider.enabled = enable;
        internal void SetColliderDirection( Vector3 dir ) => targetCollider.transform.localPosition = dir;
        internal void SetAttackData( AttackData data ) => _data = data;

    }

}
