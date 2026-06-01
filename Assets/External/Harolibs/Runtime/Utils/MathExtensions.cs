using UnityEngine;

namespace HaroLibs {
    public static class MathExtensions {

        public static int ToInt( this float target ) => Mathf.RoundToInt( target );
        public static float Clamp01( this float target ) => Mathf.Clamp01( target );
        public static float Clamp( this float target, float min, float max ) => Mathf.Clamp( target, min, max );
        public static float Abs( this float target ) => Mathf.Abs( target );
        public static float Sin( this float target ) => Mathf.Sin( target );
        public static float Cos( this float target ) => Mathf.Cos( target );
        public static float Tan( this float target ) => Mathf.Tan( target );
        public static float Round( this float target ) => Mathf.Round( target );

        public static Vector2 InvertAxis( this Vector2 target ) => new( target.y, target.x );
        public static Vector2 Clamp( this Vector2 target, float min, float max ) =>
                                    new( target.x.Clamp( min, max ), target.y.Clamp( min, max ) );

        // temp
        public static bool Compare( this Color32 target, Color32 other, byte margin = 5 ) {
            var r = Mathf.Abs( target.r - other.r ) < margin;
            var g = Mathf.Abs( target.g - other.g ) < margin;
            var b = Mathf.Abs( target.b - other.b ) < margin;
            var a = Mathf.Abs( target.a - other.a ) < margin;
            return r && g & b && a;
        }

        public static int SparseBitcount( int n ) {
            int count = 0;
            while (n != 0) {
                count++;
                n &= ( n - 1 );
            }
            return count;
        }

    }
}
