using UnityEngine;

namespace HaroLibs {
    [CreateAssetMenu( fileName = nameof( IntClampedValeContainer ), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof( IntClampedValeContainer ) )]
    public class IntClampedValeContainer : IntValueContainer {

        [SerializeField] ValueField<int> min, max;

        public override int Value { get => Mathf.Clamp( _value, min, max ); 
                                    set { _value = Mathf.Clamp( value, min, max ); ValueChanged( _value ); onChanged?.Invoke( _value ); } }

    }

}
