using SkillGame.Data;
using UnityEngine;

namespace SkillGame.UI {

    public class UI_InventoryView : MonoBehaviour {

        [SerializeField] Inventory target;
        [SerializeField] UI_InventorySlot[] inventorySlots;
        [SerializeField] bool initializeOnAwake;

        private void Awake() {
            if (initializeOnAwake)
                target.Initialize();
        }

        void Start () {
            if (!target || !target.IsInitialized) return;
            for (int i = 0; i < inventorySlots.Length; i++)
                inventorySlots[ i ].SetData( target.GetSlot( i ) );
        }

    }

}
