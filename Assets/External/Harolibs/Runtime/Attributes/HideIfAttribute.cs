using UnityEngine;

namespace HaroLibs {

    public class HideIfAttribute : PropertyAttribute {

        public readonly string TargetMember;
        public readonly bool Reverse;

        public HideIfAttribute( string member, bool reverse = false ) => (TargetMember, Reverse) = (member, reverse);

    }

}
