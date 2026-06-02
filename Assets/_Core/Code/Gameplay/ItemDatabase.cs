using SkillGame.Data;
using UnityEngine;
using System.Collections.Generic;
using HaroLibs;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SkillGame {

    public class ItemDatabase : PrivateUniqueSingleton<ItemDatabase> {

        [SerializeField, ContextMenuItem( "Load Items", nameof( LoadItems ) )] ItemData[] items;

        void LoadItems() {
#if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets( $"t: {nameof( ItemData )}" );
            List<ItemData> data = new();
            foreach (var guid in guids) 
                data.Add( AssetDatabase.LoadAssetAtPath<ItemData>( AssetDatabase.GUIDToAssetPath( guid ) ) );
            items = data.ToArray();
            EditorUtility.SetDirty( this );
#endif
        }

        public static ItemData LoadItem( string id ) => Instance.items.FirstOrDefault( item => item.ID == id );

    }

}
