using System;
using UnityEngine;

namespace HaroLibs {
    public class InterfaceTypeAttribute : PropertyAttribute {

        public readonly Type TargetType;
        public readonly string MethodName;

        public InterfaceTypeAttribute( Type targetType ) => TargetType = targetType;
        public InterfaceTypeAttribute( string methodName ) => MethodName = methodName;

    }

}
