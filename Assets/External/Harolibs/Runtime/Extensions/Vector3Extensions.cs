using UnityEngine;

namespace HaroLibs {
    public static class Vector3Extensions {

        public static Vector3 Clamp( this Vector3 target, float min, float max ) =>
            new ( ClampAxis( target.x, min, max ),
                  ClampAxis( target.y, min, max ),
                  ClampAxis( target.z, min, max ) );

        static float ClampAxis( float current, float min, float max ) {
            min = Mathf.Abs( min );
            max = Mathf.Abs( max );
            return current < 0 ? Mathf.Clamp( current, -max, -min ) : Mathf.Clamp( current, min, max );
        }

        public static Vector3 Set( this Vector3 target, float? x = null, float? y = null, float? z = null ) => new( x ?? target.x, y ?? target.y, z ?? target.z );

        public static Vector3 RestrictToRange( this Vector3 source, Vector3 range ) {
            range = range.Abs();
            return new Vector3( source.x.Clamp( -range.x, range.x ), source.y.Clamp( -range.y, range.y ), source.z.Clamp( -range.z, range.z ) );
        }

        public static Vector3 Abs( this Vector3 source ) => new ( source.x.Abs(), source.y.Abs(), source.z.Abs() );

    }
}
