using UnityEngine;

namespace HaroLibs {
    public static class Vector2Extensions {

        public static Vector2 RestrictToRange( this Vector2 source, Vector2 range ) {
            range = range.Abs();
            return new Vector2( source.x.Clamp( -range.x, range.x ), source.y.Clamp( -range.y, range.y ) );
        }

        public static Vector2 Abs( this Vector2 source ) => new ( source.x.Abs(), source.y.Abs() );


        public static Vector2 RandomRange( this Vector2 source, Vector2 max ) =>
            new ( Random.Range( source.x, max.x ), Random.Range( source.y, max.y ) );

        public static float GetRandomValueWithin( this Vector2 source ) => Random.Range( source.x, source.y );

    }
}
