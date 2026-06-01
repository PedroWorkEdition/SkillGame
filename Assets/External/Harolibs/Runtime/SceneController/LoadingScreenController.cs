using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HaroLibs {
    [DefaultExecutionOrder( -900 )]
    public class LoadingScreenController : PrivateUniqueSingleton<LoadingScreenController> {

        [SerializeField] GameObject blocker;
        [SerializeField] LoadingScreen defaultScreen;

        public static bool Alive => Instance._currentScreen.Showing;

        Dictionary<LoadingScreen, LoadingScreen> _screenMap;
        LoadingScreen _currentScreen;

        bool _unloading;

        protected override void Awake() {
            base.Awake();
            blocker.SetActive( false );
            _screenMap = new();
        }

        public void ShowLoadScreen( Action onScreenShown, LoadingScreen prefab ) {
            if (blocker.activeSelf) return;
            blocker.SetActive( true );
            if (!prefab) prefab = defaultScreen;
            if (!_screenMap.ContainsKey( prefab ))
                _screenMap.Add( prefab, Instantiate( prefab, transform ) );
            _currentScreen = _screenMap[ prefab ];
            _currentScreen.ShowScreen( onScreenShown );
        }

        public void HideLoadScreen( Action onScreenHidden ) {
            if (_currentScreen && !_unloading)
                StartCoroutine( TryHideLoadingScreen( onScreenHidden ) );
        }

        IEnumerator TryHideLoadingScreen( Action onScreenHidden ) {
            _unloading = true;
            while (_currentScreen.Showing)
                yield return null;
            _currentScreen.HideScreen( onScreenHidden );
            _unloading = false;
        }

        public static void StartLoadScreen( Action onScreenShown, LoadingScreen prefab ) => Instance.ShowLoadScreen( onScreenShown, prefab );
        public static void FinishLoadingScreen( Action onScreenHidden = null ) => Instance.HideLoadScreen( onScreenHidden );
        internal static void EndLoadingScreen() => Instance.blocker.SetActive( false );

    }
}
