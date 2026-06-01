using UnityEngine;

namespace HaroLibs {
    [ExecuteInEditMode]
    public class SpriteBillboarding : MonoBehaviour {

        Camera _cam;

        private void Awake() => _cam = Camera.main;

        void Update() => transform.forward = _cam.transform.forward;
    }
}
