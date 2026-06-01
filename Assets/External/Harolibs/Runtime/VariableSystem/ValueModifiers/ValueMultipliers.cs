using UnityEngine;

namespace HaroLibs {

    public class ValueMultipliers {

        public static int MutipleInt( int origin, float multiplier ) => Mathf.RoundToInt( origin * multiplier );
        public static int MutipleInt( int origin, int multiplier ) => origin * multiplier;
        public static int MutipleInt( int origin, FloatValueContainer multiplier ) => Mathf.RoundToInt( origin * multiplier );
        public static int MutipleInt( int origin, IntValueContainer multiplier ) => origin * multiplier;

        public static float MutipleFloat( float origin, float multiplier ) => origin * multiplier;
        public static float MutipleFloat( float origin, int multiplier ) => origin * multiplier;
        public static float MutipleFloat( float origin, FloatValueContainer multiplier ) => origin * multiplier;
        public static float MutipleFloat( float origin, IntValueContainer multiplier ) => origin * multiplier;

        public static Vector2 MutipleVector2( Vector2 origin, float multiplier ) => origin * multiplier;
        public static Vector2 MutipleVector2( Vector2 origin, int multiplier ) => origin * multiplier;
        public static Vector2 MutipleVector2( Vector2 origin, FloatValueContainer multiplier ) => origin * multiplier;
        public static Vector2 MutipleVector2( Vector2 origin, IntValueContainer multiplier ) => origin * multiplier;

    }

}
