using HaroLibs;
using SkillGame.Utils;
using System;
using UnityEngine;

namespace SkillGame.Data {

    [CreateAssetMenu( fileName = nameof( Inventory ), menuName = SkillGameGlobalsConstants.InventoryPath + nameof( Inventory ), order = 100 )]
    public class Inventory : ScriptableObject, IScriptableInitializer {

        [field: SerializeField] public int Size { get; private set; }

        InventorySlot[] _slots;
        public IInventoryHolder Holder { get; private set; }
        public bool IsInitialized { get; private set; }

        public void Initialize( IInventoryHolder holder = null) {
            if (holder == null || IsInitialized) return;
            Holder = holder;
            Initialize();
        }

        public void Initialize() {
            _slots = new InventorySlot[ Size ];
            for (int i = 0; i < Size; i++)
                _slots[ i ] = new InventorySlot( this );
            IsInitialized = true;
        }

        public bool AddItem( ItemData item, int amount = 1 ) {
            for (int i = 0; i < _slots.Length; i++) 
                if (_slots[ i ].AddOrSet( item, amount ))
                    return true;
            return false;
        }

        public (ItemData data, int amount) RemoveItem( ItemData item, int amount = 1 ) {
            for (int i = 0; i < _slots.Length; i++)
                if (_slots[ i ].Item == item)
                    return _slots[ i ].Remove( amount );
            return default;
        }

        public InventorySlot GetSlot( int index ) => _slots.IsInBounds( index ) ? _slots[ index ] : null;

        public void Dispose() { }

    }

    [Serializable]
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

        int _count;
        public int Count { 
            get => _count; 
            private set {
                _count = value;
                CountChanged?.Invoke( _count );
            }
        }

        public event Action<ItemData> ItemChanged;
        public event Action<int> CountChanged;

        internal InventorySlot( Inventory source ) => _source = source;

        public bool AddOrSet( ItemData data, int count = 1 ) {
            if (!Item) {
                Item = data;
                Count = count;
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
