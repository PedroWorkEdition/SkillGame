using SkillGame.Data;
using UnityEngine;

namespace SkillGame {

    public class WorldItem : Interactable {// this only works for this project, as it is a 1 scene game
        // Ideally i want to have a global asset loader for the SOs

        [SerializeField] ItemData targetItem;
        [SerializeField, Min( 1 )] int amount = 1;
        [SerializeField] SpriteRenderer icon;

        private void OnValidate() {
#if UNITY_EDITOR
            if (!targetItem) return;
            icon.sprite = targetItem.Icon;
#endif
        }

        private void Awake() => icon.sprite = targetItem.Icon;

        public override void Interact( CharacterInteraction source ) {
            if (!source.Character.TryGetBehaviour<CharacterInventory>( out var inventory ) || !targetItem) return;
            gameObject.SetActive( !inventory.AddItem( targetItem, amount ) );
        }

        public override string GetInteractionText() => string.Format( interactionText, amount, targetItem.ItemName );

    }

}
