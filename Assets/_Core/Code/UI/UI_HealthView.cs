using SkillGame.Data;
using UnityEngine;

namespace SkillGame.UI {

    public class UI_HealthView : MonoBehaviour {

        [SerializeField] GameObject[] healthContainers;

        public void UpdateHealth( HealthChangeData data ) {
            for (int i = 0; i < healthContainers.Length; i++)
                healthContainers[ i ].SetActive( data.Current > i );
        }

    }

}
