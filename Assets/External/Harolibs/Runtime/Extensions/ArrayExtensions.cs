using System;
using HaroLibs.Cursed.Extensions;

namespace HaroLibs {
    public static class ArrayExtensions {

        public static void ForEachObject<T>( this T[] values, Action<T> action ) where T : class { foreach (var val in values) action( val ); }
        public static T[] ForEachStruct<T>( this T[] values, Action<T> action ) where T : struct { foreach (var val in values) action( val ); return values; }

        // Somehow it does not work
        [Obsolete( "Never really worked" )]
        public static T[] CreateMany<T>( int size, params object[] args ) =>
                                        CreateMany( size, () => Activator.CreateInstance( typeof( T ), args ).AsGeneric<T>() );

        public static T[] CreateMany<T>( int size, Func<T> create ) {
            var arr = new T[ size ];
            for (var i = 0; i < size; i++)
                arr[ i ] = create();
            return arr;
        }

        public static T TakeRandom<T>( this T[] source ) => source[ UnityEngine.Random.Range( 0, source.Length ) ];

        public static T TryIndex<T>( this T[] source, int index, T defaultValue = default ) =>
                                                                source.IsInBounds( index ) ? source[ index ] : defaultValue;

        public static bool TryIndex<T>( this T[] source, int index, out T result ) {
            bool isInBounds = source.IsInBounds( index );
            result = isInBounds ? source[ index ] : default;
            return isInBounds;
        }

        public static int IndexOf<T>( this T[] source, T item ) => Array.IndexOf( source, item );
        public static int IndexOf<T>( this T[] source, Func<T, bool> validation ) {
            for (var i = 0; i < source.Length; i++) {
                if (validation( source[ i ] ))
                    return i;
            }
            return -1;
        }

        public static bool IsInBounds<T>( this T[] source, int index ) => index >= 0 && index < source.Length;

        public static void ActOnElement<T>( this T[] source, int index, Action<T> callback ) {
            if (!source.IsInBounds( index )) return;
            callback( source[ index ] );
        }

    }
}
