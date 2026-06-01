using System;
using UnityEngine;
using HaroLibs.Cursed.Extensions;

namespace HaroLibs {

    [Serializable]
    public sealed class InterfaceField<T> {

        [field: SerializeField] public bool UseObjectReference { get; private set; }
        [SerializeReference, ReferenceSelector] T reference;
        [SerializeField, InterfaceType( nameof( GetInterfaceType ) )] UnityEngine.Object obj;

        public T Value => UseObjectReference ? obj.AsGeneric<T>() : reference;

        Type GetInterfaceType() => typeof( T );

        public static implicit operator T( InterfaceField<T> field ) => field.Value;

    }

}
