using HaroLibs;
using SkillGame.Utils;
using System;
using UltEvents;
using UnityEngine;

namespace SkillGame.Data {

    [CreateAssetMenu( fileName = nameof( Inventory ), menuName = SkillGameGlobalsConstants.InventoryPath + nameof( Inventory ), order = 100 )]
    public class Inventory : ScriptableObject, IScriptableInitializer {

        [field: SerializeField, AutoGUID] public string ID { get; private set; }
        [field: SerializeField] public int Size { get; private set; }
        [SerializeField] UltEvent<ItemData> onItemAdded, onItemRemoved;
        [SerializeField] UltEvent onClear;

        [NonSerialized] InventorySlot[] _slots;
        public IInventoryHolder Holder { get; private set; }
        [field: NonSerialized] public bool IsInitialized { get; private set; } = false;

        public void Initialize( IInventoryHolder holder ) {
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
                if (_slots[ i ].AddOrSet( item, amount )) {
                    onItemAdded?.Invoke( item );
                    return true;
                }
            return false;
        }

        public (ItemData data, int amount) RemoveItem( InventorySlot slot, int amount = 1 ) {
            var result = slot.Remove( amount );
            onItemRemoved?.Invoke( result.data );
            return result;
        }

        public (ItemData data, int amount) RemoveItem( ItemData item, int amount = 1 ) {
            for (int i = 0; i < _slots.Length; i++)
                if (_slots[ i ].Item == item) {
                    var result = _slots[ i ].Remove( amount );
                    onItemRemoved?.Invoke( item );
                    return result;
                }
            return default;
        }

        public InventorySlot GetSlot( int index ) => _slots.IsInBounds( index ) ? _slots[ index ] : null;

        public void Clear() {
            if (!IsInitialized) return;
            _slots.ForEach( slot => slot.Clear() );
            onClear?.Invoke();
            IsInitialized = false;
        }

        public void Dispose() {
            Holder = null;
        }

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

        public InventorySlot( Inventory source ) => _source = source;

        public bool AddOrSet( ItemData data, int count ) {
            if (data == null) {
                Item = null;
                Count = 0;
                return true;
            }
            if (!Item) {
                Item = data;
                Count = count;
                return true;
            }
            if (Item == data) {
                if (!Item.Stackable || Count >= Item.MaxStack) return false;
                Count = Mathf.Min( Count + count, Item.MaxStack );
                return true;
            }
            return false;
        }

        public void Add( int count ) => Count = Mathf.Min( Count + count, Item.MaxStack );

        public void Set( ItemData data, int count ) => (Item, Count) = (data, count);

        public (ItemData data, int amount) Remove( int count = 1 ) {
            if (!Item || count < 1) return default;
            Count -= count;
            var item = Item;
            if (Count <= 0) {
                Item = null;
                count += Count;
                Count = 0;
            }
            return (item, count);
        }

        public bool ValidateUse() => true;

        public bool Use() {
            if (!_source) return false;
            if (!Item || !Item.Use( _source )) return false;
            if (Item.ConsumedWhenUsed) Remove();
            return true;
        }

        public void Clear() => (Item, Count) = (null, 0);

    }

    public interface IInventoryHolder {

        void EquipWeapon( WeaponData weapon );

    }

}
