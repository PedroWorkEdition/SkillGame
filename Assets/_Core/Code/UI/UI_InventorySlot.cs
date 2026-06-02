using SkillGame.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SkillGame.UI {

    public class UI_InventorySlot : MonoBehaviour {

        const string k_countFormat = "x{0}";

        [SerializeField] Image icon;
        [SerializeField] TMP_Text amount;

        InventorySlot _slot;

        public ItemData Data => _slot == null ? null : _slot.Item;

        internal void SetData( InventorySlot slot ) {
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

        public void SetData( UI_InventorySlot slot ) => SetData( slot._slot );

        private void ItemChanged( ItemData data ) {
            icon.gameObject.SetActive( data );
            amount.gameObject.SetActive( data && data.Stackable );
            if (data)
                icon.sprite = data.Icon;
        }

        void OnAmountChanged( int val ) => amount.text = string.Format( k_countFormat, val );

        public void LogBeginDrag() => Debug.Log( $"Drag begun at: {name}" );
        public void LogEndDrag() => Debug.Log( $"Drag ended at: {name}" );

        public void Clear() {
            if (_slot == null) return;
            _slot.CountChanged -= OnAmountChanged;
            _slot.ItemChanged -= ItemChanged;
            _slot = null;
        }

        public void Use() {
            if (_slot == null) return;
            _slot.Use();
        }

        public void RemoveAll() {
            if (_slot == null) return;
            _slot.Clear();
        }

    }

}
