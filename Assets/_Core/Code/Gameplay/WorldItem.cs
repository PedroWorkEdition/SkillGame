using SkillGame.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SkillGame {

    public class WorldItem : Interactable {// this only works for this project, as it is a 1 scene game
        // Ideally i want to have a global asset loader for the SOs

        [SerializeField] AssetReferenceT<ItemData> targetItem;
        [SerializeField, Min( 1 )] int amount = 1;
        [SerializeField] SpriteRenderer icon;

        ItemData _loadedData;

        private void OnValidate() {
#if UNITY_EDITOR
            if (!targetItem.RuntimeKeyIsValid()) return;
            icon.sprite = targetItem.editorAsset.Icon;
#endif
        }

        private void Awake() => targetItem.LoadAssetAsync().Completed += LoadData;

        private void OnDestroy() { 
            _loadedData = null;
            targetItem.ReleaseAsset();
        }

        public override void Interact( CharacterInteraction source ) {
            if (!source.Character.TryGetBehaviour<CharacterInventory>( out var inventory ) || !_loadedData) return;
            gameObject.SetActive( !inventory.AddItem( _loadedData, amount ) );
        }

        void LoadData( AsyncOperationHandle<ItemData> handler ) {
            if (handler.Status == AsyncOperationStatus.Failed) return;
            _loadedData = handler.Result;
            icon.sprite = _loadedData.Icon;
        }

        public override string GetInteractionText() => string.Format( interactionText, amount, _loadedData.ItemName );

    }

}
