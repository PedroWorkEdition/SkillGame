using SkillGame.Data;
using UnityEngine;

namespace SkillGame {

    public class WorldItem : Interactable {

        [SerializeField] ItemData targetItem;
        [SerializeField, Min( 1 )] int amount = 1;
        [SerializeField] SpriteRenderer icon;

        private void OnValidate() {
#if UNITY_EDITOR
            if (!targetItem) return;
            icon.sprite = targetItem.Icon;
#endif
        }

        private void Awake() => icon.sprite = targetItem ? targetItem.Icon : null;

        public void SetData( ItemData data, int amount ) {
            targetItem = data;
            this.amount = amount;
            icon.sprite = targetItem.Icon;
        }

        public override void Interact( CharacterInteraction source ) {
            if (!source.Character.TryGetBehaviour<CharacterInventory>( out var inventory ) || !targetItem) return;
            gameObject.SetActive( !inventory.AddItem( targetItem, amount ) );
        }

        public override string GetInteractionText() => targetItem ? string.Format( interactionText, amount, targetItem.ItemName ) : string.Empty;

    }

}
