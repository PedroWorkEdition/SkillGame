using SkillGame.Data;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;

namespace SkillGame.UI {

    public class UI_InventoryView : MonoBehaviour {

        [SerializeField] Inventory target;
        [SerializeField] Button useButton, removeButton;
        [SerializeField] UI_InventorySlot[] inventorySlots;
        [SerializeField] UI_InventorySlot previewSlot;
        [SerializeField] UltEvent<ItemData> onCurrentSelected;
        [SerializeField] bool initializeOnAwake;

        UI_InventorySlot _currentSelected;
        UI_InventorySlot _hover;

        private void Awake() {
            if (!initializeOnAwake) return;
            target.Initialize();
        }

        void Start () {
            if (!target || !target.IsInitialized) return;
            for (int i = 0; i < inventorySlots.Length; i++)
                inventorySlots[ i ].SetData( target.GetSlot( i ) );
        }

        public void CurrentHover( UI_InventorySlot slot ) => _hover = slot;
        public void RemoveHover( UI_InventorySlot slot ) {
            if (slot != _hover) return;
            _hover = null;
        }

        public void ApplyToCurrentHover( UI_InventorySlot slot ) {
            if (!_hover) {
                _currentSelected.SetData( previewSlot );
                previewSlot.Clear();
                previewSlot.gameObject.SetActive( false );
                return;
            }
            _currentSelected.SetData( _hover );
            _hover.SetData( previewSlot );
            previewSlot.gameObject.SetActive( false );
            previewSlot.Clear();
            _currentSelected = null;
            SetSelected( _hover );
        }

        public void SetSelected( UI_InventorySlot slot ) {
            if (slot == _currentSelected) {
                slot.Use();
                return;
            }
            _currentSelected = slot;
            onCurrentSelected?.Invoke( _currentSelected.Data );
            useButton.interactable = _currentSelected;
            removeButton.interactable = _currentSelected;
        }

        public void UseCurrentSelected() {
            if (!_currentSelected) return;
            _currentSelected.Use();
        }

        public void RemoveCurrentSelected() {
            if (!_currentSelected) return;
            onCurrentSelected?.Invoke( null );
            _currentSelected.RemoveAll();
        }

    }

}
