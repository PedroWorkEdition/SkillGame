using System;
using UnityEngine;

namespace SkillGame.Data {

    [Serializable]
    public class Inventory {

        [field: SerializeField] public int Size { get; private set; }

        InventorySlot[] _slots;
        public IInventoryHolder Holder { get; private set; }

        public void Initialize( IInventoryHolder holder ) {
            Holder = holder;
            _slots = new InventorySlot[ Size ];
            for (int i = 0; i < Size; i++)
                _slots[ i ] = new InventorySlot( this );
        }

        public bool AddItem( ItemData item ) {
            for (int i = 0; i < _slots.Length; i++) 
                if (_slots[ i ].AddOrSet( item ))
                    return true;
            return false;
        }

        public (ItemData data, int amount) RemoveItem( ItemData item, int amount = 1 ) {
            for (int i = 0; i < _slots.Length; i++)
                if (_slots[ i ].Item == item)
                    return _slots[ i ].Remove( amount );
            return default;
        }

    }

    public class InventorySlot {

        readonly Inventory _source;

        ItemData _item;
        public ItemData Item { 
            get => _item; 
            private set {
                _item = value;
                ItemChanged?.Invoke( _item );
            }
        }

        public int Count { get; private set; }

        public event Action<ItemData> ItemChanged;

        internal InventorySlot( Inventory source ) => _source = source;

        public bool AddOrSet( ItemData data, int count = 1 ) {
            if (!Item) {
                Item = data;
                return true;
            }
            if (Item == data && !Item.Stackable) return false;
            if (Count >= Item.MaxStack) return false;
            Count = Mathf.Max( Count + count, Item.MaxStack );
            return true;
        }

        public (ItemData data, int amount) Remove( int count = 1 ) {
            if (!Item || count < 1) return default;
            if (Count < 1) {
                Item = null;
                return default;
            }
            Count -= count;
            var item = Item;
            if (Count < 0) {
                Item = null;
                count += Count;
                Count = 0;
            }
            return (item, count);
        }

        public bool ValidateUse() => true;

        public bool Use() {
            if (!Item || !Item.Use( _source )) return false;
            if (Item.ConsumedWhenUsed) Remove();
            return true;
        }

    }

    public interface IInventoryHolder {

        void EquipWeapon( WeaponData weapon );

    }

}
