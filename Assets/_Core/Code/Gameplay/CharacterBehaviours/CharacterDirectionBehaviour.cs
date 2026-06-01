using DG.Tweening;
using UnityEngine;

namespace SkillGame {

    public class CharacterDirectionBehaviour : CharacterBehaviourBase {

        [SerializeField] float turnSpeed = .25f;
        [SerializeField] Transform targetModel;

        Vector2 _direction = Vector2.right;
        Tweener _turnTween;
        float _scale;

        internal override void Initialize( Character source ) {
            base.Initialize( source );
            _scale = character.model.transform.localScale.x;
            source.DirectionTriggered += ChangeDirection;
            source.DirectionForced += ForceDirection;
        }

        private void ForceDirection( Vector2 dir ) {
            var prevDir = _direction;
            _direction = character.LastValidDirection;
            var requestFlip = _direction.x > 0 && prevDir.x <= 0 || _direction.x < 0 && prevDir.x >= 0;
            if (!requestFlip) return;
            var model = targetModel ? targetModel : character.model;
            _turnTween?.Kill();
            _turnTween = model.DOScale( new Vector3( _scale * ( _direction.x < 0 ? -1 : 1 ), model.localScale.y, model.localScale.z ), turnSpeed );
        }

        void ChangeDirection( InputBuffer _, Vector2 dir ) {
            if (!Active) return;
            ForceDirection( dir );
        }

    }

}
