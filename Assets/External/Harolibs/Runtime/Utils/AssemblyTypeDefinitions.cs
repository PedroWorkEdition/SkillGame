using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HaroLibs {
    public static class AssemblyTypeDefinitions {

        const string k_genericToken = "`1";

        public static List<Type> GetAllTypes<T>() => GetAllTypes( typeof( T ) );
        public static List<Type> GetAllInterfaces<T>() => GetAllInterfaces( typeof( T ) );

        public static List<Type> GetAllTypes( Type targetType ) =>
             AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany( s => s.GetTypes() )
                        .Where( p => targetType.IsGenericType ? targetType.GenericTypeFilter( p ) : targetType.NormalTypeFilter( p ) ).ToList();

        public static List<Type> GetAllInterfaces( Type targetType ) =>
            AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany( s => s.GetTypes() )
                        .Where( p => targetType.IsAssignableFrom( p ) && p.IsInterface && p != targetType ).ToList();

        public static Type AsType( this string source, Assembly sourceAsm = null ) {
            if ( source.Contains( "<" ) ) {
                source = source.Replace( "<", k_genericToken ).Replace( ">", "" );
                source = source[ ..( source.IndexOf( k_genericToken ) + k_genericToken.Length ) ];
            }
            if ( sourceAsm != null ) return sourceAsm.GetType( source );
            var asmblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach( var asm in asmblies ) {
                var type = asm.GetType( source );
                if ( type != null ) return type;
            }
            return null;
        }

        static bool GenericTypeFilter( this Type targetType, Type current ) =>
            !targetType.ContainsGenericParameters ? targetType.IsAssignableFrom( current ) :
            current.BaseType != null && current.BaseType.IsGenericType && current.BaseType.GetGenericTypeDefinition() == targetType;

        static bool NormalTypeFilter( this Type targetType, Type current ) => targetType.IsAssignableFrom( current ) && !current.IsAbstract && !current.IsInterface;

        public static (Type type, object source) GetTypeByPath( object origin, string propertyPath ) {
            string[] pathParts = propertyPath.Split( '.' )[ ..^1 ];
            Type currentType = origin.GetType();
            MemberInfo memberInfo = null;
            foreach (string part in pathParts) {
                if (part == "Array")
                    continue;
                if (part.Contains( "[" )) {
                    var index = int.Parse( part.Replace( "data[", string.Empty )[ ..^1 ].ToString() );
                    origin = ( ( Array )origin ).GetValue( index );
                    currentType = currentType.GetElementType();
                    continue;
                }
                memberInfo = currentType.GetMember( part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ).FirstOrDefault();
                if (memberInfo == null) {
                    UnityEngine.Debug.LogError( $"Failed to find member from path: {propertyPath} - last reached: {part}" );
                    return default;
                }
                origin = GetMemberInfoValue( memberInfo, origin );
                currentType = GetMemberInfoType( memberInfo );
            }
            return new( currentType, origin );
        }

        static Type GetMemberInfoType( MemberInfo memberInfo ) {
            if (memberInfo is FieldInfo fieldInfo)
                return fieldInfo.FieldType;
            else if (memberInfo is PropertyInfo propertyInfo)
                return propertyInfo.PropertyType;
            return default;
        }

        static object GetMemberInfoValue( MemberInfo memberInfo, object source ) {
            if (memberInfo is FieldInfo fieldInfo)
                return fieldInfo.GetValue( source );
            else if (memberInfo is PropertyInfo propertyInfo)
                return propertyInfo.GetValue( source );
            return default;
        }

    }
}
