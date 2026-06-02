using System.Collections.Generic;
using UltEvents;
using UnityEngine;

namespace SkillGame {

    public class CharacterInteraction : CharacterBehaviourBase {

        [SerializeField] UltEvent<string> onInteractableChanged;

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

        void UpdateInteraction() =>
            onInteractableChanged?.Invoke( _interactables.Count == 0 ? string.Empty : _interactables[0].GetInteractionText() );

        public void RegisterInteractable( Interactable interactable ) { 
            _interactables.Add( interactable );
            UpdateInteraction();
        }

        public void UnregisterInteractable( Interactable interactable ) { 
            _interactables.Remove( interactable );
            UpdateInteraction();
        }
    }

}
