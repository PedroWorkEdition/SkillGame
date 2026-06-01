using System;
using System.Reflection;
using HaroLibs.Cursed.Extensions;
using UnityEngine;

namespace HaroLibs {
    public static class MemberInfoExtensions {
        public static Type GetMemberType( this MemberInfo memberInfo ) =>
            memberInfo switch {
                FieldInfo field => field.FieldType,
                PropertyInfo prop => prop.PropertyType,
                MethodInfo mtd => mtd.ReturnType,
                _ => null
            };

        public static object GetMemberValue( this MemberInfo memberInfo, object source, params object[] args ) =>
            memberInfo switch {
                FieldInfo field => field.GetValue( source ),
                PropertyInfo prop => prop.GetValue( source ),
                MethodInfo mtd => mtd.Invoke( source, args ),
                _ => null
            };

        public static T GetMemberValue<T>( this MemberInfo memberInfo, object source, params object[] args ) {
            if (memberInfo == null || memberInfo.GetMemberType() != typeof( T )) return default;
            return (memberInfo switch {
                FieldInfo field => field.GetValue( source ),
                PropertyInfo prop => prop.GetValue( source ),
                MethodInfo mtd => mtd.Invoke( source, args ),
                _ => null
            }).AsGeneric<T>();
        }

        public static void SetMemberValue( this MemberInfo memberInfo, object source, object val ) {
            try {
                switch (memberInfo) {
                    case FieldInfo field: field.SetValue( source, val ); break;
                    case PropertyInfo prop: prop.SetValue( source, val ); break;
                }
            } catch( Exception e ) {
                Debug.LogError( $"{memberInfo.Name}: {e.Message}" );
            }
        }
    }
}
