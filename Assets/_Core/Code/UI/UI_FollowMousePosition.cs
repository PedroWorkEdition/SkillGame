using UnityEngine;
using UnityEngine.InputSystem;

namespace SkillGame.UI {

    public class UI_FollowMousePosition : MonoBehaviour {

        [SerializeField] Vector2 offset;

        RectTransform _rect;

        private void Awake() => _rect = transform as RectTransform;

        private void Update() {
            _rect.position = Mouse.current.position.ReadValue() + offset;
        }

    }

}
