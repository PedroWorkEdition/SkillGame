using SkillGame.Data;
using SkillGame.Utils;
using UnityEngine;

namespace SkillGame {

    [DefaultExecutionOrder( 100 )]
    public class InventoryLoader : MonoBehaviour, ISaveDataProvider<InventoryData> {

        [SerializeField] Inventory target;

        public string ID => target.ID;

        public InventoryData GetSaveData() => new( target );

        public void LoadData( object data ) {
            target.Clear();
            var loadedData = ( InventoryData )data;
            for (int i = 0; i < loadedData.Slots.Length; i++) 
                target.GetSlot( i ).AddOrSet( ItemDatabase.LoadItem( loadedData.Slots[ i ].ItemID ), loadedData.Slots[ i ].Count );
        }

        private void Start() {
            PersistentSystem.RegisterData( this );
            PersistentSystem.ApplyLoadedData( ID );
        }

    }

}
