using System;
using UnityEngine;

namespace SkillGame.Data {

    [Serializable]
    public struct AttackData {

        public string State;
        public bool UseKnockback;
        public float KnockbackStrength;
        public float DamageMultiplier;
        [Range( 0, 1 ), Tooltip( "Normalized time from animation" )]
        public float ColliderEnableTime;
        [Range( 0, 1 )] public float ColliderDisableTime;
        [Range( 0, 1 )] public float ComboTriggerStart;
        [Range( 0, 1 )] public float ComboTriggerEnd;
        [Range( 0, 1 )] public float FinishedTime;
        public AnimationCurve SpeedMultiplier;
        public bool OverrideRange;
        public float CustomRange;
        public GameObject OverrideCollider;

        public readonly bool ColliderIsEnabled( float time ) => time >= ColliderEnableTime && time <= ColliderDisableTime;
        public readonly bool ComboIsEnabled( float time ) => time >= ComboTriggerStart && time <= ComboTriggerEnd;

    }

}
