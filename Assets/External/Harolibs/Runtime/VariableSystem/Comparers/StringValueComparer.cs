using UnityEngine;

namespace HaroLibs {
    public class StringValueComparer : ValueComparerBase<string> {

        [SerializeField] System.StringComparison comparison = System.StringComparison.Ordinal;
        protected override bool Validate() => origin.Value.Equals( compare.Value, comparison );
    
    }
}
