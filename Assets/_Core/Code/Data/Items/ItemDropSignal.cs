using HaroLibs;
using SkillGame.Utils;
using UnityEngine;

namespace SkillGame.Data {
    [CreateAssetMenu( fileName = nameof( ItemDropContext ), menuName = SkillGameGlobalsConstants.SOPath + nameof( ItemDropContext ) )]
    public class ItemDropSignal : EventSignal<ItemDropContext> { }

}
