using System;

namespace HaroLibs {
    [Serializable]
    public class SceneData : IContainableData {
        public SceneField Scene;
        public SceneField[] DependentScenes;
        public bool UseLoadingScreen, IsActive;
        public LoadingScreen LoadingScreen;

        public static implicit operator string( SceneData data ) => data?.Scene;
    }
}
