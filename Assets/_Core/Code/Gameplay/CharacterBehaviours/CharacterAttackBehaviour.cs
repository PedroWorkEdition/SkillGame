using Cysharp.Threading.Tasks;
using SkillGame.Data;
using SkillGame.StateMachineLogic;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SkillGame {

    public class CharacterAttackBehaviour : CharacterBehaviourBase {// this is pretty bad

        const string k_attackSpeedParameter = "AttackSpeed";

        enum AttackState { WaitingInput, Attacking, Locked, Finished }

        [SerializeField] DamageColliderController defaultCollider;
        [SerializeField] float defaultRange = 1f;
        [SerializeField] float moveSpeed = 10f;

        AttackState _attackState = AttackState.WaitingInput;
        int _attackIndex = -1;
        float _lastDir = 1;
        DamageColliderController _collider;
        CancellationTokenSource _source;
        Dictionary<GameObject, DamageColliderController> _colliderMap;

        AnimatorStateInfo AnimState => character.animator ? character.animator.GetCurrentAnimatorStateInfo( 0 ) : default;

        private void Awake() {
            _colliderMap = new();
        }

        internal override void Initialize( Character source ) {
            base.Initialize( source );
            character.AttackTriggered += Attack;
            character.DirectionTriggered += DirectionTriggered; ;
            defaultCollider.Initilize( character );
            var ctx = new StateContext( Character.STATE_ATTACK, EnterState, ExitState );
            ctx = ctx.AddLink( Link.Create( character.GetIdleState(), () => _attackState == AttackState.Finished ) );
            var state = character.RegisterState( ctx );
            character.AddLinkToIdleState( Link.Create( state, () => _attackState == AttackState.Attacking ) );
        }

        private void DirectionTriggered( InputBuffer _, Vector2 dir ) {
            if (dir.magnitude == 0 || dir.x == 0) return;
            _lastDir = character.LastValidDirection.x;
        }

        internal override void PostInitilize() {
            var state = character.GetState( Character.STATE_ATTACK );
            var moveState = character.GetState( Character.STATE_MOVE );
            moveState.AddLink( state, () => _attackState == AttackState.Attacking );
        }

        void EnterState() => character.rb.linearVelocity = Vector3.zero;

        void ExitState() {
            _attackIndex = -1;
            _collider.SetColliderEnable( false );
            DisposeToken();
            _attackState = AttackState.WaitingInput;
        }

        void Attack( InputBuffer buffer ) {
            if (!Active || _attackState != AttackState.WaitingInput) return;
            buffer.Consume();
            _attackIndex++;
            var attacksSize = character.Weapon ? Mathf.Min( character.Stats.MaxCombo, character.Weapon.Attacks.Length ) : 0;
            if (_attackIndex >= attacksSize) return;
            _attackState = AttackState.Attacking;
            DisposeToken();
            _source = new();
            AttackSequence().Forget();
        }

        async UniTaskVoid AttackSequence() {
            try {
                var currentAttack = character.Weapon.Attacks[ _attackIndex ];
                character.animator.SetFloat( k_attackSpeedParameter, character.Stats.AttackSpeed );
                character.animator.Play( currentAttack.State );
                await UniTask.Yield();
                if (!await ValidateState( currentAttack.State ).AttachExternalCancellation( _source.Token )) {
                    _attackState = AttackState.Finished;
                    DisposeToken();
                    return;
                }
                await ProcessAttack( currentAttack ).AttachExternalCancellation( _source.Token );
            } catch { }
            DisposeToken();
        }

        async UniTask<bool> ValidateState( string state ) {
            var currentState = AnimState;
            float stateCheckTimeout = .2f;
            while (!currentState.IsName( state )) {
                if (stateCheckTimeout < 0)
                    return false;
                await UniTask.Yield( cancellationToken: _source.Token );
                stateCheckTimeout -= Time.deltaTime;
                currentState = character.animator.GetCurrentAnimatorStateInfo( 0 );
            }
            return true;
        }

        async UniTask ProcessAttack( AttackData data ) {
            _collider = GetCollider( data );
            _collider.SetAttackData( data );
            var attackSide = _lastDir > 0 ? 1 : -1;
            character.ForceDirection();
            while (AnimState.normalizedTime < 1) {
                var time = AnimState.normalizedTime;
                _collider.SetColliderEnable( data.ColliderIsEnabled( time ) );
                _attackState = time >= data.FinishedTime ? AttackState.Locked :
                               ( data.ComboIsEnabled( time ) ? AttackState.WaitingInput : AttackState.Attacking );
                float speed = data.SpeedMultiplier.Evaluate( time ) * moveSpeed;
                _collider.SetColliderDirection( new Vector2( ( data.OverrideRange ? data.CustomRange : defaultRange ) * attackSide, 0 ) );
                character.rb.linearVelocity = attackSide * speed * character.Cam.transform.right;
                await UniTask.Yield( cancellationToken: _source.Token );
            }
            _attackState = AttackState.Finished;
        }

        DamageColliderController GetCollider( AttackData data ) {
            if (!data.OverrideCollider)
                return defaultCollider;
            if (_colliderMap.TryGetValue( data.OverrideCollider, out var result )) return result;
            _colliderMap.Add( data.OverrideCollider, defaultCollider );
            if (!data.OverrideCollider.GetComponent<DamageColliderController>()) return defaultCollider;
            var go = Instantiate( data.OverrideCollider, defaultCollider.transform.parent );
            go.transform.localScale = Vector3.zero;
            var controller = go.GetComponent<DamageColliderController>();
            controller.Initilize( character );
            return _colliderMap[ data.OverrideCollider ] = controller;
        }

        void DisposeToken() {
            _source?.Cancel();
            _source?.Dispose();
            _source = null;
        }

    }

}
