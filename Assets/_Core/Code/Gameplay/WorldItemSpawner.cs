using SkillGame.Data;
using UnityEngine;

namespace SkillGame {

    public class WorldItemSpawner : MonoBehaviour {

        [SerializeField] WorldItem itemPrefab;

        public void SpawnItem( ItemDropContext ctx, Transform source ) {
            if (!itemPrefab) return;
            var item = Instantiate( itemPrefab, transform );
            item.transform.position = ( Vector2 )source.position + Vector2.one;
            item.SetData( ctx.Data, ctx.Count );
        }

    }

}
