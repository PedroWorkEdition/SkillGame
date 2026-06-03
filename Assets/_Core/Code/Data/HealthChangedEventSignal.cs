using HaroLibs;
using SkillGame.Utils;
using UnityEngine;

namespace SkillGame.Data {
    [CreateAssetMenu( fileName = nameof( HealthChangedEventSignal ), menuName = SkillGameGlobalsConstants.SOPath + nameof( HealthChangedEventSignal ) )]
    public class HealthChangedEventSignal : EventSignal<HealthChangeData> { }

}
