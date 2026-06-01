namespace HaroLibs {
    public static class BoolExtensions {

        /// <summary> true self is inverse of other | ( target && !other ) || ( !target && other ) | </summary>
        public static bool InverseOf( this bool target, bool other ) => ( target && !other ) || ( !target && other );

    }
}
