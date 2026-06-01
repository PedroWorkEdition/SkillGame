using System;

namespace HaroLibs.Cursed.Extensions {
    public static class GenericExtensions {

        public static int Int<T>( this T source ) => Convert.ToInt32( source );
        public static float Float<T>( this T source ) => Convert.ToSingle( source );
        public static T AsGeneric<T>( this object source ) => ( T )source;

    }
}
