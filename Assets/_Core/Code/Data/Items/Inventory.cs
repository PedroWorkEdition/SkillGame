using HaroLibs;
using System;
using UnityEngine;

namespace SkillGame.Data {

    public class Inventory {

        [field: SerializeField] public int Slots { get; private set; }

        InventorySlot[] _items;
        public IInventoryHolder Holder { get; private set; }

        public void Initialize( IInventoryHolder holder ) {
            Holder = holder;
            _items = new InventorySlot[ Slots ];
            for (int i = 0; i < Slots; i++)
                _items[ i ] = new InventorySlot( this );
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



    }

}
