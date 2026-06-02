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

        internal void SetData( InventorySlot slot ) {
            if (slot == null) {
                gameObject.SetActive( false );
                return;
            }
            _slot = slot;
            _slot.CountChanged += OnAmountChanged;
            _slot.ItemChanged += ItemChanged;
            gameObject.SetActive( true );
        }

        private void ItemChanged( ItemData data ) { 
            icon.gameObject.SetActive( data );
            amount.gameObject.SetActive( data && data.Stackable );
            if (data)
                icon.sprite = data.Icon;
        }

        void OnAmountChanged( int val ) => amount.text = string.Format( k_countFormat, val );

        public void LogBeginDrag() => Debug.Log( $"Drag begun at: {name}" );
        public void LogEndDrag() => Debug.Log( $"Drag ended at: {name}" );

    }

}
