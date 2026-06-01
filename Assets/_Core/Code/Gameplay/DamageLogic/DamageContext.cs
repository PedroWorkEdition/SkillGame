using UnityEngine;

namespace SkillGame {
    public struct DamageContext {

        public readonly Character Perpetrator;
        public readonly Character Victim;
        public readonly Transform Source;
        public readonly AttackDamageData Data;
        public int DamageDealt;

        public DamageContext( Character perpetrator, Character victim, Transform source, AttackDamageData data ) =>
            (Perpetrator, Victim, Source, Data, DamageDealt) = (perpetrator, victim, source, data, 0);

    }

}
