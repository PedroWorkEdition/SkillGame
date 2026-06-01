using System;
using UnityEngine;

namespace SkillGame {
    public class AttackDamageData {

        public int RawDamage;
        public float Multiplier;
        public bool UseKnockBack;
        public float KnockBackStrength;
        public Action<DamageContext> OnHit;

        internal int Damage => Mathf.RoundToInt( RawDamage * Multiplier );

    }

}
