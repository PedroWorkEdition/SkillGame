using System;
using System.Collections.Generic;

namespace HaroLibs {
    public static class TypeExtensions {

        public static IEnumerable<Type> GetInheritanceHierarchy( this Type type, Type cutOff = null ) {
            Func<Type, bool> check = cutOff.IsInterface ?
                                     cur => cutOff.IsAssignableFrom( cur ) :
                                     cur => cur != cutOff;
            var current = type;
            while (true) {
                yield return current;
                current = current.BaseType;
                if (!check( current ))
                    yield break;
            }
            /*
            for (var current = type; check( current ); current = current.BaseType) {
                if ( !check( current ) ) yield break;
                yield return current;
            }
            */
        }

    }
}
