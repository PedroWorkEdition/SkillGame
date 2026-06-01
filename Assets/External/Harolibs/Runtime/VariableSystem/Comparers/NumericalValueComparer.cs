using System;
using UnityEngine;

namespace HaroLibs {
    public class NumericalValueComparer<T> : ValueComparerBase<T> where T : IComparable {

        [SerializeField] CompareType compareType;

        protected override bool Validate() =>
            compareType switch {
                CompareType.Equal => base.Validate(),
                CompareType.NotEqual => !base.Validate(),
                CompareType.GreaterThan => origin.Value.CompareTo( compare.Value ) > 0,
                CompareType.LowerThan => origin.Value.CompareTo( compare.Value ) < 0,
                CompareType.GreaterOrEqualThan => origin.Value.CompareTo( compare.Value ) >= 0,
                CompareType.LowerOrEqualThan => origin.Value.CompareTo( compare.Value ) <= 0,
                _ => false,
            };

    }
}
