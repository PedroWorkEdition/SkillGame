using UnityEngine;

namespace SkillGame.Data {
    [CreateAssetMenu( fileName = nameof( WeaponData ), menuName = "Data/" + nameof( WeaponData ) )]
    public class WeaponData : ScriptableObject {

        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public AttackData[] Attacks { get; private set; }

    }

}
