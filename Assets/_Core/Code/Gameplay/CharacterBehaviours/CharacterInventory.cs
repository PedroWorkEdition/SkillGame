using SkillGame.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SkillGame {

    public class CharacterInventory : CharacterBehaviourBase, IInventoryHolder {

        [SerializeField] Transform weaponParent;
        [SerializeField] AssetReferenceT<WeaponData> startingWeapon;
        [SerializeField] Inventory inventory;

        WeaponData _equipedWeapon;
        GameObject _weaponPrefab;

        void OnDestroy() {
            if (startingWeapon != null && startingWeapon.IsValid())
                startingWeapon.ReleaseAsset();
        }

        internal override void Initialize( Character source ) {
            base.Initialize( source );
            inventory.Initialize( this );
            if (startingWeapon.RuntimeKeyIsValid())
                startingWeapon.LoadAssetAsync().Completed += StartingWeaponOperationCompleted;
        }

        private void StartingWeaponOperationCompleted( AsyncOperationHandle<WeaponData> handle ) {
            if (handle.Status == AsyncOperationStatus.Failed) return;
            EquipWeapon( handle.Result );
        }

        public void EquipWeapon( WeaponData weapon ) {
            if (!weapon || weapon == _equipedWeapon) return;
            if (_weaponPrefab)
                Destroy( _weaponPrefab );
            _equipedWeapon = weapon;
            if (_equipedWeapon.WeaponPrefab)
                _weaponPrefab = Instantiate( _equipedWeapon.WeaponPrefab, weaponParent );
        }

        public bool AddItem( ItemData data, int amount = 1 ) => inventory.AddItem( data, amount );
        public (ItemData item, int count) RemoveItem( ItemData data, int amount = 1 ) => inventory.RemoveItem( data, amount );

    }

}
