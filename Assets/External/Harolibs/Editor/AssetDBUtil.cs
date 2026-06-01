using UnityEditor;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace HaroLibsEditor {
    public static class AssetDBUtil {

        public static string GetFullPath( this UObj obj ) => AssetDatabase.GetAssetPath( obj ).Replace( "Assets", Application.dataPath );
        public static bool DeleteAsset( UObj obj ) => AssetDatabase.DeleteAsset( AssetDatabase.GetAssetPath( obj ) );

        public static T[] GetAllAssetsOfType<T>( params string[] searchInFolders ) where T : UObj {
            var guids = AssetDatabase.FindAssets( $"t:{typeof( T )}", searchInFolders );
            var vals = new T[ guids.Length ];
            for (int i = 0; i < guids.Length; i++)
                vals[ i ] = ( T )AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( guids[ i ] ), typeof( T ) );
            return vals;
        }

    }
}
