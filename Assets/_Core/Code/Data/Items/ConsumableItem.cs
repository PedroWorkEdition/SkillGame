using SkillGame.Utils;
using UnityEngine;

namespace SkillGame.Data {
    [CreateAssetMenu( fileName = nameof( ConsumableItem ), menuName = SkillGameGlobalsConstants.InventoryPath + nameof( ConsumableItem ) )]
    public class ConsumableItem : ItemData<IConsumableItem> {

        public override bool Use( Inventory inventory ) {
            for(int i = 0; i < Actions.Length; i++) 
                if (!Actions[i].Use( inventory )) 
                    return false;
            return true;
        }

    }

}
