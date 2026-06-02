using SkillGame.Utils;
using UnityEngine;

namespace SkillGame {
    public class GameManager : MonoBehaviour {

        private void Awake() {
            Application.quitting += SaveGame;
        }

        private void SaveGame() => PersistentSystem.Save();
    }

}
