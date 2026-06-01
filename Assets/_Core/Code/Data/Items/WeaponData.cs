using SkillGame.Utils;
using UnityEngine;

namespace SkillGame.Data {

    [CreateAssetMenu( fileName = nameof( WeaponData ), menuName = SkillGameGlobalsConstants.InventoryPath + nameof( WeaponData ) )]
    public class WeaponData : ItemData<IEquipableItem> {

        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public AttackData[] Attacks { get; private set; }

        public override bool Use( Inventory inventory ) {
            return false;
        }
    }
}