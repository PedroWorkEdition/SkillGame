using System;
using System.Collections.Generic;

namespace HaroLibs {
    public static class IEnumerableExtensions {

        public static void ForEach<T>( this IEnumerable<T> target, Action<T> action ) {
            if (action == null) return;
            foreach ( var item in target ) action( item );
        }

    }
}
