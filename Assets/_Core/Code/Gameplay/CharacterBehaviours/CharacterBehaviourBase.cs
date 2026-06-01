using UnityEngine;

namespace SkillGame {

    public abstract class CharacterBehaviourBase : MonoBehaviour {

        [field: SerializeField] public bool Active { get; internal set; }
        protected Character character;

        public Character Character => character;

        internal virtual void Initialize( Character source ) => character = source;
        internal virtual void PostInitilize() { }

    }

}
