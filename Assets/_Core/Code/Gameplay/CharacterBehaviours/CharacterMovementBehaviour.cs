using SkillGame.StateMachineLogic;
using UnityEngine;

namespace SkillGame {

    public class CharacterMovementBehaviour : CharacterBehaviourBase {

        [SerializeField] float speed;
        [SerializeField] string targetAnimState;

        Vector2 _moveInput;

        internal override void Initialize( Character source ) {
            base.Initialize( source );
            source.DirectionTriggered += SetMovement;
            RegisterState();
        }

        void SetMovement( InputBuffer buffer, Vector2 direction ) {
            if (!Active) return;
            buffer.Consume();
            _moveInput = direction;
        }

        void RegisterState() {
            var moveStateContext = new StateContext( Character.STATE_MOVE, OnEnterState, () => _moveInput = Vector2.zero );
            moveStateContext = moveStateContext.AddLink( character.GetIdleState(), () => _moveInput.magnitude == 0 );
            var moveState = character.RegisterState( moveStateContext );
            character.AddLinkToIdleState( moveState, () => _moveInput.magnitude > 0 );
        }

        void OnEnterState() {
            character.animator.Play( targetAnimState );
        }

        void FixedUpdate() {
            if (!Active) return;
            Vector3 dir = ( character.Cam.transform.right * _moveInput.x ) + ( character.Cam.transform.forward * _moveInput.y );
            character.rb.linearVelocity = character.Stats.MoveSpeedMultiplier * speed * ( new Vector2( dir.x, dir.z ) ).normalized;
        }

    }

}
