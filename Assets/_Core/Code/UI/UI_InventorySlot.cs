using SkillGame.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SkillGame.UI {

    public class UI_InventorySlot : MonoBehaviour {

        const string k_countFormat = "x{0}";

        [SerializeField] Image icon;
        [SerializeField] TMP_Text amount;
        [SerializeField] bool localSlot;

        InventorySlot _slot;

        public ItemData Data => _slot?.Item;
        public int Count => _slot?.Count ?? 0;
        public InventorySlot BindedSlot => _slot;

        private void OnDestroy() {
            if (_slot == null) return;
            _slot.CountChanged -= OnAmountChanged;
            _slot.ItemChanged -= ItemChanged;
        }

        public void SetData( ItemData data, int amount ) {
            if (_slot == null && localSlot)
                BindSlot( new( null ) );
            _slot.Set( data, amount );
            gameObject.SetActive( true );
        }

        public void AddData( int amount ) {
            if (_slot == null && localSlot)
                BindSlot( new( null ) );
            _slot.Add( amount );
            gameObject.SetActive( true );
        }

        internal void BindSlot( InventorySlot slot ) {
            if (slot == null) {
                gameObject.SetActive( false );
                Clear();
                return;
            }
            _slot = slot;
            _slot.CountChanged += OnAmountChanged;
            _slot.ItemChanged += ItemChanged;
            ItemChanged( _slot.Item );
            OnAmountChanged( _slot.Count );
            gameObject.SetActive( true );
        }

        public void SetData( UI_InventorySlot slot ) => SetData( slot._slot.Item, slot._slot.Count );
        public void AddData( UI_InventorySlot slot ) => AddData( slot._slot.Count );

        private void ItemChanged( ItemData data ) {
            icon.gameObject.SetActive( data );
            amount.gameObject.SetActive( data && data.Stackable );
            if (data)
                icon.sprite = data.Icon;
        }

        void OnAmountChanged( int val ) => amount.text = string.Format( k_countFormat, val );

        public void Clear() {
            if (_slot == null) return;
            RemoveAll();
        }

        public void Use() {
            if (_slot == null) return;
            _slot.Use();
        }

        public void RemoveAmount( int amount ) {
            _slot.Remove( amount );
        }

        public void RemoveAll() {
            if (_slot == null) return;
            _slot.Clear();
        }
    }

}
