using System.Collections.Generic;

namespace SkillGame {

    public class CharacterInteraction : CharacterBehaviourBase {

        List<Interactable> _interactables;

        internal override void Initialize( Character source ) {
            base.Initialize( source );
            _interactables = new();
            source.ActionTriggered += AttemptInteraction;
        }

        private void AttemptInteraction( InputBuffer buffer ) {
            if (!Active || _interactables.Count == 0) return;
            buffer.Consume();
            _interactables[ 0 ].Interact( this );
        }

        public void RegisterInteractable( Interactable interactable ) => _interactables.Add( interactable );
        public void UnregisterInteractable( Interactable interactable ) => _interactables.Remove( interactable );
    }

}
