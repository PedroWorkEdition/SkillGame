using System.Collections.Generic;
using UnityEngine;

namespace HaroLibs {
    [DefaultExecutionOrder( -1000 )]
    public class ScriptableInitializer : MonoBehaviour {

        [SerializeField]
#if UNITY_EDITOR
        [ContextMenuItem( "Get objects", nameof( GetInitializables ) )]
#endif
        ScriptableObject[] targets;

        private void Awake() {
            foreach (var target in targets)
                ( ( IScriptableInitializer )target ).Initialize();
        }

        private void OnDestroy() {
            foreach (var target in targets)
                ( ( IScriptableInitializer )target ).Dispose();
        }

#if UNITY_EDITOR
        void GetInitializables() {
            var assets = UnityEditor.AssetDatabase.FindAssets( "t:ScriptableObject" );
            List<ScriptableObject> initializers = new();
            foreach (var asset in assets) {
                var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>( UnityEditor.AssetDatabase.GUIDToAssetPath( asset ) );
                if (typeof( IScriptableInitializer ).IsAssignableFrom( obj.GetType() ))
                    initializers.Add( obj );
            }
            targets = initializers.ToArray();
        }
#endif
    }
}
