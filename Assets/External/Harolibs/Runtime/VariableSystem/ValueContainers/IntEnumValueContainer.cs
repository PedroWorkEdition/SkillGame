using System;
using System.Linq;
using UnityEngine;

namespace HaroLibs {

    [CreateAssetMenu( fileName = nameof(IntEnumValueContainer), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof(IntEnumValueContainer) )]
    public class IntEnumValueContainer : ContainerBase<int[]> {

        [SerializeField] ValueEntry[] entries;

        public override int[] Value { get => entries.Select( entry => entry.Value ).ToArray(); set { } }

        public int GetValue( string name ) => entries.FirstOrDefault( entry => entry.Name == name ).Value;

        [Serializable]
        public struct ValueEntry {

            [field: SerializeField] public ValueField<string> Name { get; private set; }
            [field: SerializeField] public int Value { get; private set; }

        }
    }

}
