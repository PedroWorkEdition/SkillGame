using SkillGame.Data;
using SkillGame.Utils;
using System;
using UnityEngine;

namespace SkillGame {

    public class CharacterInventory : CharacterBehaviourBase, IInventoryHolder, ISaveDataProvider<InventoryData> {

        [SerializeField] Transform weaponParent;
        [SerializeField] WeaponData startingWeapon;
        [SerializeField] Inventory inventory;

        WeaponData _equipedWeapon;
        GameObject _weaponPrefab;

        public string ID => inventory.ID;

        internal override void Initialize( Character source ) {
            base.Initialize( source );
            inventory.Initialize( this );
            PersistentSystem.RegisterData( this );
            PersistentSystem.ApplyLoadedData( ID );
            if (startingWeapon)
                EquipWeapon( startingWeapon );
        }

        public void EquipWeapon( WeaponData weapon ) {
            if (!weapon || weapon == _equipedWeapon) return;
            if (_weaponPrefab)
                Destroy( _weaponPrefab );
            _equipedWeapon = weapon;
            Character.Weapon = _equipedWeapon;
            if (_equipedWeapon.WeaponPrefab)
                _weaponPrefab = Instantiate( _equipedWeapon.WeaponPrefab, weaponParent );
        }

        public bool AddItem( ItemData data, int amount = 1 ) => inventory.AddItem( data, amount );
        public (ItemData item, int count) RemoveItem( ItemData data, int amount = 1 ) => inventory.RemoveItem( data, amount );

        public InventoryData GetSaveData() => new( inventory );

        public void LoadData( object data ) {
            inventory.Clear();
            var loadedData = ( InventoryData )data;
            for (int i = 0; i < loadedData.Slots.Length; i++)
                inventory.GetSlot( i ).Set( ItemDatabase.LoadItem( loadedData.Slots[ i ].ItemID ), loadedData.Slots[ i ].Count );
        }

    }

    [Serializable]
    public class InventoryData : IWritebleData {

        public SlotData[] Slots;

        public InventoryData( Inventory source ) {
            Slots = new SlotData[ source.Size ];
            for (int i = 0; i < Slots.Length; i++)
                Slots[ i ] = new( source.GetSlot( i ) );
        }

    }

    [Serializable]
    public struct SlotData {

        public string ItemID;
        public int Count;

        public SlotData( InventorySlot slot ) => (ItemID, Count) = (slot.Item ? slot.Item.ID : string.Empty, slot.Count);

    }

}
