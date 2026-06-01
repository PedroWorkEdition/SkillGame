using UnityEngine;

namespace HaroLibs {
    public class SceneObject : ScriptableObject {

        [field: SerializeField] public SceneData SceneData { get; private set; }

        public static implicit operator SceneData(SceneObject sceneData) => sceneData.SceneData;

    }
}
