using UnityEngine; 

namespace HaroLibs { 

    public class DropdownAttribute : PropertyAttribute {

        public readonly string MethodName;
        public readonly bool UseEmpty;

        public DropdownAttribute( string methodName, bool useEmpty = true ) => (MethodName, UseEmpty) = (methodName, useEmpty);

    } 

}
