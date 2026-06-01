using UnityEngine;

namespace HaroLibs {
    public static class GameObjectExtensions {

        public static void TrySetActive( this GameObject target, bool active ) {
            if (!target) return;
            target.SetActive( active );
        }

        public static string GetPath( this GameObject current ) {
            if (current.transform.parent == null)
                return "/" + current.name;
            return current.transform.parent.gameObject.GetPath() + "/" + current.name;
        }

        public static bool IsInLayerMask( this GameObject target, LayerMask mask ) => ( mask.value & ( 1 << target.layer ) ) != 0;

    }
}
