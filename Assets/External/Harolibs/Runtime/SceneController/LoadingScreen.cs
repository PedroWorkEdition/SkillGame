using System;
using UnityEngine;

namespace HaroLibs {
    public abstract class LoadingScreen : MonoBehaviour {

        [SerializeField] float minAliveTime = 1;

        Action _onShow, _onHide;
        float _timer;

        bool _showing;
        public bool Showing => _showing;

        private void Update() {
            if (!_showing) return;
            _timer -= Time.deltaTime;
            if (_timer <= 0) _showing = false;
        }

        public virtual void ShowScreen( Action onShow ) => _onShow = onShow;
        public virtual void HideScreen( Action onHidden ) => _onHide = onHidden;

        protected void OnShown() {
            _onShow?.Invoke();
            _showing = true;
            _timer = minAliveTime;
        }

        protected void OnHide() {
            _showing = false;
            _onHide?.Invoke();
            LoadingScreenController.EndLoadingScreen();
        }

    }
}
