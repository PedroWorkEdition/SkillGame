using SkillGame.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SkillGame {

    public class WorldItem : Interactable {

        [SerializeField] AssetReferenceT<ItemData> targetItem;

        public override void Interact( Component source ) {
            
        }

    }

}
