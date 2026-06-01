using UnityEngine;

namespace HaroLibs {
    public class SideButtonAttribute : PropertyAttribute {

        public readonly string MethodName;

        public SideButtonAttribute(string methodName) => MethodName = methodName;

    }
}
