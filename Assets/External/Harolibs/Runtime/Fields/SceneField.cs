using System;
using UnityEngine;

namespace HaroLibs {

    [Serializable]
    public class SceneField {

        [SerializeField] string sceneName;
        [SerializeField] string sceneGUID;
        public string SceneName => sceneName;

        public static implicit operator string( SceneField scene ) => scene?.SceneName;

    }
}
