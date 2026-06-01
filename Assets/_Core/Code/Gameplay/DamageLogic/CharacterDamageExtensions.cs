using SkillGame.Data;
using UnityEngine;

namespace SkillGame {
    public static class CharacterDamageExtensions {

        public static DamageContext DamageOther( this Character perpetrator, Character victim, Transform source, AttackData data ) =>
                                                 new( perpetrator, victim, source, perpetrator.GetDamageData( data ) );

        public static AttackDamageData GetDamageData( this Character perpetrator, AttackData data ) => new() {
            RawDamage = perpetrator.Weapon ? perpetrator.Stats.BaseDamage + perpetrator.Weapon.Damage : 0,
            Multiplier = perpetrator.Stats.AttackPower,
            UseKnockBack = data.UseKnockback,
            KnockBackStrength = data.KnockbackStrength
        };

    }

}
