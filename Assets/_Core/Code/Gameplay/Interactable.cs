using UnityEngine;

namespace SkillGame {

    public abstract class Interactable : MonoBehaviour {

        [SerializeField] protected string interactionText;

        public abstract void Interact( CharacterInteraction source );

        private void OnTriggerEnter2D( Collider2D collision ) {
            if (!collision.TryGetComponent<Character>( out var character ) || !character.TryGetBehaviour<CharacterInteraction>( out var interaction )) return;
            interaction.RegisterInteractable( this );
        }

        private void OnTriggerExit2D( Collider2D collision ) {
            if (!collision.TryGetComponent<Character>( out var character ) || !character.TryGetBehaviour<CharacterInteraction>( out var interaction )) return;
            interaction.UnregisterInteractable( this );
        }

    }

}
