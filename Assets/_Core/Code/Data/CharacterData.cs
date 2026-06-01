using SkillGame.Utils;
using System;
using UnityEngine;

namespace SkillGame.Data {

    [CreateAssetMenu( fileName = nameof( CharacterData ), menuName = SkillGameGlobalsConstants.SOPath + nameof( CharacterData ) )]
    public class CharacterData : ScriptableObject {

        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public WeaponData Weapon { get; private set; }
        [field: SerializeField] public CharacterStatusData Data { get; private set; }

    }

    [Serializable]
    public struct CharacterStatusData {

        [field: SerializeField] public int HP { get; private set; }
        [field: SerializeField] public int MaxCombo { get; private set; }
        [field: SerializeField] public int Defense { get; private set; }
        [field: SerializeField] public int BaseDamage { get; private set; }
        [field: SerializeField, Range( 1, 3 )] public float AttackPower { get; private set; }
        [field: SerializeField, Range( 1, 3 )] public float AttackSpeed { get; private set; } 
        [field: SerializeField, Range( 1, 4 )] public float MoveSpeedMultiplier { get; private set; }
        [field: SerializeField, Range( 1, 5 )] public float Range { get; private set; }

        public static CharacterStatusData operator +( CharacterStatusData a, CharacterStatusData b ) {
            a.HP += b.HP;
            a.MaxCombo += b.MaxCombo;
            a.Defense += b.Defense;
            a.BaseDamage += b.BaseDamage;
            a.AttackPower += b.AttackPower;
            a.AttackSpeed += b.AttackSpeed;
            a.MoveSpeedMultiplier += b.MoveSpeedMultiplier;
            a.Range += b.Range;
            return a;
        }

        public static CharacterStatusData Default( int? hp = null, int? maxCombo = null, int? baseDamage = null ) => new() {
            HP = hp ?? 0,
            MaxCombo = maxCombo ?? 1,
            Defense = 0,
            BaseDamage = baseDamage ?? 0,
            AttackPower = 1,
            AttackSpeed = 1,
            MoveSpeedMultiplier = 1,
            Range = 1
        };

        internal static CharacterStatusData CreateRaw( int? hp = null, int? maxCombo = null, int? defense = null, int? baseDamage = null ) => new() {
            HP = hp ?? 0,
            MaxCombo = maxCombo ?? 0,
            Defense = defense ?? 0,
            BaseDamage = baseDamage ?? 0,
            AttackPower = 0,
            AttackSpeed = 0,
            MoveSpeedMultiplier = 0,
            Range = 0
        };

        internal static CharacterStatusData CreateMultiplier( float? power = null, float? attackSpd = null, float? moveSpd = null, float? range = null ) => new() {
            HP = 0,
            MaxCombo = 0,
            Defense = 0,
            BaseDamage = 0,
            AttackPower = power ?? 0,
            AttackSpeed = attackSpd ?? 0,
            MoveSpeedMultiplier = moveSpd ?? 0,
            Range = range ?? 0
        };

    }

}
