using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HaroLibs {
    [DefaultExecutionOrder( -500 )]
    public class SceneController : PrivateUniqueSingleton<SceneController> {

        [SerializeField] SceneObject startingScene;
        [SerializeField] SceneField[] filter;

        string _currentActiveScene;

        Dictionary<string, string[]> _dependencyMap;
        List<string> _otherScenes;
        SceneData _currentData;

        protected override void Awake() {
            base.Awake();
            _dependencyMap = new();
            _otherScenes = new();
        }

        private void Start() {
#if UNITY_EDITOR
            if (SceneManager.loadedSceneCount > 1) {
                List<Scene> loadedScenes = new();
                for (var i = 0; i < SceneManager.sceneCount; i++)
                    loadedScenes.Add( SceneManager.GetSceneAt( i ) );
                foreach (var f in filter)
                    loadedScenes.Remove( loadedScenes.Find( s => s.name == f ) );
                foreach (var scene in loadedScenes)
                    if (scene.isLoaded)
                        _currentActiveScene = scene.name;
            } else
#endif
                OpenScene( startingScene );
        }

        public static bool IsSceneOpen( SceneData data ) => data == Instance._currentData;

        public static void ReOpenCurrentScene() => OpenScene( Instance._currentData );

        public static void OpenScene( SceneData data, LoadingScreen overrideLoadingScreen = null ) {
            Optional<LoadingScreen> ls = new( overrideLoadingScreen );
            if (data.UseLoadingScreen)
                LoadingScreenController.StartLoadScreen( () => Instance.OpenTargetScene( data,
                                                         () => Instance.StartCoroutine( Instance.CheckDependencies() ) ),
                                                         ls.GetValue( data.LoadingScreen ) );
            else
                Instance.OpenTargetScene( data, null );
        }

        void OpenTargetScene( SceneData data, Action onFinishedopen ) {
            string sceneName = data.Scene;
            bool isActive = data.IsActive;
            string[] dependencies = data.DependentScenes.Select( s => s.SceneName ).ToArray();

            if (!isActive) {
                StartCoroutine( LoadScene( sceneName, isActive, dependencies, onFinishedopen ) );
                _otherScenes.Add( sceneName );
                return;
            }
            Instance._currentData = data;
            if (!string.IsNullOrEmpty( _currentActiveScene ))
                StartCoroutine( UnloadScene( _currentActiveScene, () => StartCoroutine( LoadScene( sceneName, isActive, dependencies, onFinishedopen ) ) ) );
            else
                StartCoroutine( LoadScene( sceneName, isActive, dependencies, onFinishedopen ) );
        }

        public static void CloseScene( string sceneName ) {
            if (!Instance) return;
            Instance.StartCoroutine( Instance.UnloadScene( sceneName, null ) );
        }

        IEnumerator CheckDependencies() {
            var dependencies = FindObjectsByType<MonoBehaviour>( FindObjectsSortMode.None ).OfType<ILoadingDependency>();
            if (dependencies.Count() > 0)
                yield return new WaitWhile( () => dependencies.Any( d => d.IsLoading() ) );
            LoadingScreenController.FinishLoadingScreen();
        }

        IEnumerator LoadScene( string sceneName, bool isActive, string[] dependencies, Action onLoaded ) {
            var op = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Additive );
            op.allowSceneActivation = false;
            yield return new WaitWhile( () => LoadingScreenController.Alive );
            op.allowSceneActivation = true;
            while (!op.isDone)
                yield return null;
            _dependencyMap.Add( sceneName, dependencies );
            foreach ( var dep in dependencies) {
                var depOp = SceneManager.LoadSceneAsync( dep, LoadSceneMode.Additive );
                while (!depOp.isDone)
                    yield return null;
                depOp.allowSceneActivation = true;
                _otherScenes.Add( dep );
            }
            if (isActive) _currentActiveScene = sceneName;
            onLoaded?.Invoke();
        }

        IEnumerator UnloadScene( string sceneName, Action onUnload ) {
            var op = SceneManager.UnloadSceneAsync( sceneName );
            if (op == null) yield break;
            while (!op.isDone)
                yield return null;
            if( _dependencyMap.ContainsKey( sceneName ) ) {
                foreach (var dep in _dependencyMap[sceneName]) {
                    var depOp = SceneManager.UnloadSceneAsync( dep );
                    while (!depOp.isDone)
                        yield return null;
                }
                _dependencyMap.Remove( sceneName );
            }
            onUnload?.Invoke();
        }
    }

}
