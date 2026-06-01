using System;
using System.Reflection;

namespace HaroLibsEditor {
    public static class MemberInfoExtensions {

        public static Type GetMemberType( this MemberInfo target ) =>
            target switch {
                PropertyInfo prop => prop.PropertyType,
                FieldInfo field => field.FieldType,
                MethodInfo mtd => mtd.ReturnType,
                _ => null
            };

    }

}
