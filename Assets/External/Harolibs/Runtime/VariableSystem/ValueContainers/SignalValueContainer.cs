using UnityEngine;

namespace HaroLibs {
    [CreateAssetMenu( fileName = nameof( SignalValueContainer ), menuName = HaroLibsConstPaths.SO_VARIABLE + nameof( SignalValueContainer ) )]
    public class SignalValueContainer : ValueContainerBase<EmptyEventSignal> {
        public override EmptyEventSignal Value { get => _value; set => _value = value; }

        public void Emmit() => Value.Raise();

    }
}
