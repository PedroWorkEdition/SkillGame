using System;

namespace HaroLibs {
    public static class ObjectExtensions {
        public static bool IsNumeric( this object x ) => x != null && IsNumeric( x.GetType() );
        public static bool IsNumeric( this Type type ) => IsNumeric( type, Type.GetTypeCode( type ) );
        static bool IsNumeric( Type type, TypeCode typeCode ) => typeCode == TypeCode.Decimal ||
                                                                ( type.IsPrimitive && typeCode != TypeCode.Object &&
                                                                  typeCode != TypeCode.Boolean &&
                                                                  typeCode != TypeCode.Char );

        public static object ParseToNumeric( this string target ) {
            if ( string.IsNullOrEmpty( target ) ) return null;
            return target.Contains( '.' ) ? float.Parse( target ) : int.Parse( target );
        }
        public static object ParseToNumeric( this string target, Type numericType ) => numericType switch {
                var t when t == typeof( int ) => int.Parse( target ),
                var t when t == typeof( uint ) => uint.Parse( target ),
                var t when t == typeof( byte ) => byte.Parse( target ),
                var t when t == typeof( sbyte ) => sbyte.Parse( target ),
                var t when t == typeof( short ) => short.Parse( target ),
                var t when t == typeof( ushort ) => ushort.Parse( target ),
                var t when t == typeof( long ) => long.Parse( target ),
                var t when t == typeof( ulong ) => ulong.Parse( target ),
                var t when t == typeof( float ) => float.Parse( target ),
                var t when t == typeof( double ) => double.Parse( target ),
                _ => null
            };

    }
}
