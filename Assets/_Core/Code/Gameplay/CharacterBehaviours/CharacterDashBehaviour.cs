using SkillGame.StateMachineLogic;
using System;
using UnityEngine;

namespace SkillGame {
    public class CharacterDashBehaviour : CharacterBehaviourBase {

        [SerializeField] float speed;
        [SerializeField] float duration;

        bool _trigger;

        internal override void Initialize( Character source ) {
            base.Initialize( source );
            character.SubActionTriggered += TriggerDash;
            var ctx = new StateContext( Character.STATE_DASH, typeof( TimedState ), OnEnterState, OnExitState, ( Func<float> )( () => duration ) );
            var idleState = character.GetIdleState();
            ctx = ctx.AddLink( Link.Create( idleState, () => true ) );
            var state = character.RegisterState( ctx );
            var link = Link.Create( state, () => _trigger );
            idleState.AddLink( link );
        }

        internal override void PostInitilize() {
            var moveState = character.GetState( Character.STATE_MOVE );
            moveState.AddLink( Link.Create( character.GetState( Character.STATE_DASH ), () => _trigger ) );
        }

        void TriggerDash( InputBuffer buffer ) {
            if (!Active) return;
            buffer.Consume();
            _trigger = true;
        }

        void OnEnterState() {
            _trigger = false;
            character.animator.Play( "Dash" );
            var charDir = character.LastValidDirection;
            Vector3 dir = ( character.Cam.transform.right * charDir.x ) + ( character.Cam.transform.forward * charDir.y );
            character.rb.linearVelocity = speed * new Vector3( dir.x, 0, dir.z ).normalized;
        }

        void OnExitState() { 
            character.rb.linearVelocity = Vector3.zero;
        }
    }

}
