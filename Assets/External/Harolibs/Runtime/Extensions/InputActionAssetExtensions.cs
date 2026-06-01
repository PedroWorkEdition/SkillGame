using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

namespace HaroLibs {

    public static class InputActionAssetExtensions {

        public static InputActionReference[] GetAllActionReferences( this InputActionAsset targetAsset ) {
#if UNITY_EDITOR
            var subAssets = AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( targetAsset ) );
            var inputActionReferences = new List<InputActionReference>();
            foreach (var obj in subAssets) {
                // there are 2 InputActionReference returned for each InputAction in the asset, need to filter to not add the hidden one generated for backward compatibility
                if (obj is InputActionReference inputActionReference && ( inputActionReference.hideFlags & HideFlags.HideInHierarchy ) == 0)
                    inputActionReferences.Add( inputActionReference );
            }
            return inputActionReferences.ToArray();
#else
            return System.Array.Empty<InputActionReference>();
#endif
        }

    }

}
