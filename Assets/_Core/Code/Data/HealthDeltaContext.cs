using UnityEngine;

namespace SkillGame.Data {

    public readonly struct HealthDeltaContext {

        public readonly int Delta;
        public readonly Transform Origin;
        public readonly bool Critical;

        public HealthDeltaContext( int delta, Transform origin, bool critical = false ) =>
            (Delta, Origin, Critical) = (delta, origin, critical);

    }

}
