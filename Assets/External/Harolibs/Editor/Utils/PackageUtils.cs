using UnityEditor;

namespace HaroLibsEditor {

    public static class PackageUtils {

        const string PACKAGE_PATH = "Packages/com.emptytile.harolibs/";

        /// <summary> Loads asset within package </summary>
        /// <param name="relativePath">Path within 'Packages/com.emptytile.harolibs/'</param>
        internal static T LoadAsset<T>( string relativePath ) where T : UnityEngine.Object => 
                                        AssetDatabase.LoadAssetAtPath<T>( PACKAGE_PATH + relativePath );

        internal static T[] LoadAllAssets<T>( string relativePath ) where T : UnityEngine.Object =>
                                              AssetDBUtil.GetAllAssetsOfType<T>( new[] { PACKAGE_PATH + relativePath } );
    }

}
