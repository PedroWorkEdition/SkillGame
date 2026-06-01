using UnityEngine;
using UnityEngine.EventSystems;

namespace SkillGame.Utils {

    public static class UltEventStaticHelper {

        public static Transform GetComponentTransform( Component source ) => source.transform;
        public static Transform GetGOTransform( GameObject source ) => source.transform;
        public static void SelectSelectable( GameObject source ) => EventSystem.current.SetSelectedGameObject( source );

    }

}
