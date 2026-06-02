using UnityEngine;

namespace SkillGame {
    public abstract class Interactable : MonoBehaviour { 
        
        [field: SerializeField] public string InteractionText { get; set; }

        public abstract void Interact( Component source );

        private void OnTriggerEnter2D( Collider2D collision ) {
            if (!collision.TryGetComponent<CharacterInteraction>( out var interaction )) return;
            interaction.RegisterInteractable( this );
        }

        private void OnTriggerExit2D( Collider2D collision ) {
            if (!collision.TryGetComponent<CharacterInteraction>( out var interaction )) return;
            interaction.UnregisterInteractable( this );
        }

    }

}
