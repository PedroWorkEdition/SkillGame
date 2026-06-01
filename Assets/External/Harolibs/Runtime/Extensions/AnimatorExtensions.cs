using UnityEngine;

namespace HaroLibs {
    public static class AnimatorExtensions {

        public static bool IsNameState( this Animator animator, string name, int layer = 0 ) => animator.GetCurrentAnimatorStateInfo( layer ).IsName( name );
        public static float CurrentStateTime( this Animator animator, int layer = 0 ) => animator.GetCurrentAnimatorStateInfo( layer ).normalizedTime;

    }
}
